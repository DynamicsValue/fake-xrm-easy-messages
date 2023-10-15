using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class ExecuteFetchRequestExecutor : IFakeMessageExecutor
    {
        private readonly Dictionary<string, int?> _typeCodes = new Dictionary<string, int?>();

        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is ExecuteFetchRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>ExecuteFetchResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var executeFetchRequest = (ExecuteFetchRequest)request;

            if (executeFetchRequest.FetchXml == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("You need to provide FetchXml value");
            }

            var service = ctx.GetOrganizationService();

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(executeFetchRequest.FetchXml)
            };
            var queryResult = (service.Execute(retrieveMultiple) as RetrieveMultipleResponse).EntityCollection;

            XDocument doc = new XDocument(new XElement("resultset",
                new XAttribute("morerecords", Convert.ToInt16(queryResult.MoreRecords))));
            if (queryResult.PagingCookie != null)
            {
                doc.Root.Add(new XAttribute("paging-cookie", queryResult.PagingCookie));
            }

            var allowedAliases = new string[0];

            var fetchXmlDocument = XDocument.Parse(executeFetchRequest.FetchXml).Root;
            if (fetchXmlDocument != null)
            {
                var linkedEntityName = fetchXmlDocument.Descendants("link-entity").Attributes("name").Select(a => a.Value).Distinct();
                allowedAliases = linkedEntityName.Concat(fetchXmlDocument.Descendants("link-entity").Attributes("alias").Select(a => a.Value).Distinct()).ToArray();
            }

            foreach (var row in queryResult.Entities)
            {
                doc.Root.Add(CreateXmlResult(row, ctx, allowedAliases));
            }

            var response = new ExecuteFetchResponse
            {
                Results = new ParameterCollection
                                 {
                                    { "FetchXmlResult", doc.ToString() }
                                 }
            };

            return response;
        }
        
        private XElement CreateXmlResult(Entity entity, IXrmFakedContext ctx, string[] allowedAliases)
        {
            var row = new XElement("result");
            var formattedValues = entity.FormattedValues;

            foreach (var entAtt in entity.Attributes)
            {
                var attribute = AddAttributeAliases(allowedAliases, entAtt);

                var attributeValueElement = AttributeValueToFetchResult(attribute, formattedValues, ctx);
                if (attributeValueElement == null)
                {
                    continue;
                }

                row.Add(attributeValueElement);
            }

            return row;
        }

        private static KeyValuePair<string, object> AddAttributeAliases(string[] allowedAliases, KeyValuePair<string, object> entAtt)
        {
            var attribute = entAtt;

            // Deprecated ExecuteFetch doesn't use implicitly numbered entity aliases
            if (attribute.Key.Contains("."))
            {
                var alias = attribute.Key.Substring(0, attribute.Key.IndexOf(".", StringComparison.Ordinal));
                if (!allowedAliases.Contains(alias))
                {
                    // The maximum amount of linked entities is 10, 
                    var newAlias = alias.Substring(0, alias.Length - (!alias.EndsWith("10") ? 1 : 2));
                    if (allowedAliases.Contains(newAlias))
                    {
                        var newKey = attribute.Key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        newKey[0] = newAlias;
                        attribute = new KeyValuePair<string, object>(string.Join(".", newKey), attribute.Value);
                    }
                    else
                    {
                        // unknown alias, just leave it
                    }
                }
            }

            return attribute;
        }

        internal XElement AttributeValueToFetchResult(KeyValuePair<string, object> entAtt, FormattedValueCollection formattedValues, IXrmFakedContext ctx)
        {
            XElement attributeValueElement;
            if (entAtt.Value == null)
                return null;
            if (entAtt.Value is DateTime?)
            {
                attributeValueElement = XElement.Parse(String.Format("<{0} date=\"{1:yyyy-MM-dd}\" time=\"{1:hh:mm tt}\">{1:yyyy-MM-ddTHH:mm:sszz:00}</{0}>", entAtt.Key, entAtt.Value));
            }
            else if (entAtt.Value is EntityReference entRef)
            {
                attributeValueElement = EntityReferenceValueToFetchResultValue(entAtt, ctx, entRef);
            }
            else if (entAtt.Value is bool?)
            {
                attributeValueElement = BooleanAttributeValueToFetchResultValue(entAtt, formattedValues);
            }
            else if (entAtt.Value is OptionSetValue osValue)
            {
                var formattedValue = osValue.Value.ToString();
                if (formattedValues.ContainsKey(entAtt.Key))
                    formattedValue = formattedValues[entAtt.Key];
                attributeValueElement = XElement.Parse(String.Format("<{0} name=\"{1}\" formattedvalue=\"{2}\">{2}</{0}>", entAtt.Key, formattedValue, osValue.Value));
            }
            else if (entAtt.Value is Enum enumValue)
            {
                var formattedValue = enumValue.ToString();
                if (formattedValues.ContainsKey(entAtt.Key))
                    formattedValue = formattedValues[entAtt.Key];
                attributeValueElement = XElement.Parse(String.Format("<{0} name=\"{1}\" formattedvalue=\"{2}\">{2}</{0}>", entAtt.Key, formattedValue, enumValue));
            }
            else if (entAtt.Value is Money moneyValue)
            {
                attributeValueElement = MoneyAttributeValueToFetchResultValue(entAtt, formattedValues, moneyValue);
            }
            else if (entAtt.Value is decimal?)
            {
                var decimalVal = (decimal?)entAtt.Value;

                attributeValueElement = XElement.Parse(String.Format("<{0}>{1:0.####}</{0}>", entAtt.Key, decimalVal.Value));
            }
            else if (entAtt.Value is AliasedValue)
            {
                var alliasedVal = entAtt.Value as AliasedValue;
                attributeValueElement = AttributeValueToFetchResult(new KeyValuePair<string, object>(entAtt.Key, alliasedVal.Value), formattedValues, ctx);
            }
            else if (entAtt.Value is Guid)
            {
                attributeValueElement = XElement.Parse(String.Format("<{0}>{1}</{0}>", entAtt.Key, entAtt.Value.ToString().ToUpper()));
            }
#if FAKE_XRM_EASY_9
            else if (entAtt.Value is OptionSetValueCollection optionSetValueCollection)
            {
                var values = String.Join(",", optionSetValueCollection.Select(o => o.Value).OrderBy(v => v));
                var serializedCollection = String.Join(",", "[-1", values, "-1]");
                attributeValueElement = XElement.Parse(String.Format("<{0} name=\"{1}\">{1}</{0}>", entAtt.Key, serializedCollection));
            }
#endif
            else
            {
                attributeValueElement = XElement.Parse(String.Format("<{0}>{1}</{0}>", entAtt.Key, entAtt.Value));
            }
            return attributeValueElement;
        }

        private static XElement MoneyAttributeValueToFetchResultValue(KeyValuePair<string, object> entAtt,
            FormattedValueCollection formattedValues, Money moneyValue)
        {
            XElement attributeValueElement;
            var formattedValue = moneyValue.Value.ToString();
            if (formattedValues.ContainsKey(entAtt.Key))
                formattedValue = formattedValues[entAtt.Key];
            attributeValueElement = XElement.Parse(String.Format("<{0} formattedvalue=\"{1}\">{2:0.##}</{0}>", entAtt.Key,
                formattedValue, moneyValue.Value));
            return attributeValueElement;
        }

        private static XElement BooleanAttributeValueToFetchResultValue(KeyValuePair<string, object> entAtt,
            FormattedValueCollection formattedValues)
        {
            XElement attributeValueElement;
            var boolValue = (bool?)entAtt.Value;

            var formattedValue = boolValue.ToString();
            if (formattedValues.ContainsKey(entAtt.Key))
                formattedValue = formattedValues[entAtt.Key];
            attributeValueElement = XElement.Parse(String.Format("<{0} name=\"{1}\">{2}</{0}>", entAtt.Key, formattedValue,
                Convert.ToInt16(boolValue)));
            return attributeValueElement;
        }

        private XElement EntityReferenceValueToFetchResultValue(KeyValuePair<string, object> entAtt, IXrmFakedContext ctx,
            EntityReference entRef)
        {
            XElement attributeValueElement;
            if (!_typeCodes.ContainsKey(entRef.LogicalName))
            {
                var entType = ctx.FindReflectedType(entRef.LogicalName);
                var typeCode = entType.GetField("EntityTypeCode").GetValue(null);

                _typeCodes.Add(entRef.LogicalName, (int?)typeCode);
            }

            attributeValueElement =
                XElement.Parse(String.Format("<{0} dsc=\"0\" yomi=\"{1}\" name=\"{1}\" type=\"{3}\">{2:D}</{0}>", entAtt.Key,
                    entRef.Name, entRef.Id.ToString().ToUpper(), _typeCodes[entRef.LogicalName]));
            return attributeValueElement;
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(ExecuteFetchRequest);
        }
    }
}