using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Messages.Exceptions.InstantiateTemplateRequest;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Provides a fake implementation for an InstantiateTemplateRequest
    /// </summary>
    public class InstantiateTemplateRequestExecutor: IFakeMessageExecutor
    {
        private const string TEMPLATE_ENTITY_LOGICAL_NAME = "template";
        private const string TEMPLATE_ENTITY_OBJECT_TYPE_CODE_ATTRIBUTE = "templatetypecode";

        private const string TEMPLATE_BODY_ATTRIBUTE = "body";
        private const string TEMPLATE_SUBJECT_ATTRIBUTE = "subject";

        internal const string HTML_BODY_PREFIX = "<html>\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n</head>\r\n<body>\r\n<div data-wrapper=\"true\" dir=\"ltr\" style=\"font-family:Segoe UI; font-size:9pt\"><div style=\"font-family:Segoe UI; font-size:9pt\">\r\n";
        internal const string HTML_BODY_SUFFIX = "</div>\r\n</div>\r\n</body>\r\n</html>";
        
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InstantiateTemplateRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>InstantiateTemplateResponse with the instantiated email</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as InstantiateTemplateRequest;
            
            //Check if template exists
            var containsTemplate = ctx.ContainsEntity(TEMPLATE_ENTITY_LOGICAL_NAME, req.TemplateId);
            if (!containsTemplate)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist,
                    $"Entity 'template' With Id = {req.TemplateId} Does Not Exist");
            }

            var template = ctx.GetEntityById(TEMPLATE_ENTITY_LOGICAL_NAME, req.TemplateId);
            var objectType = template.GetAttributeValue<string>(TEMPLATE_ENTITY_OBJECT_TYPE_CODE_ATTRIBUTE);
            if (objectType != req.ObjectType)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    $"Template type is incorrect for given objectType and the current template's templatetypecode");
            }
            
            var containsRegarding = ctx.ContainsEntity(req.ObjectType, req.ObjectId);
            if (!containsRegarding)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist,
                    $"Entity '{req.ObjectType}' With Id = {req.ObjectId} Does Not Exist");
            }
            var regarding = ctx.GetEntityById(req.ObjectType, req.ObjectId);

            var userId = ctx.CallerProperties.CallerId.Id;
            var containsUser = ctx.ContainsEntity("systemuser", userId);
            Entity user = null;
            if (containsUser)
            {
                user = ctx.GetEntityById("systemuser", userId);
            }

            var xmlData = GetXmlData(regarding, user);
            
            var email = ctx.NewEntityRecord("email");
            string transformedSubject = template.Contains(TEMPLATE_SUBJECT_ATTRIBUTE) ? GetRenderedXslt(TEMPLATE_SUBJECT_ATTRIBUTE, template.GetAttributeValue<string>(TEMPLATE_SUBJECT_ATTRIBUTE), xmlData) : "";
            string transformedBody = template.Contains(TEMPLATE_BODY_ATTRIBUTE) ? GetRenderedXslt( TEMPLATE_BODY_ATTRIBUTE, template.GetAttributeValue<string>(TEMPLATE_BODY_ATTRIBUTE), xmlData) : "";
            email["subject"] = transformedSubject;
            email["description"] = $"{HTML_BODY_PREFIX}{transformedBody}\r\n{HTML_BODY_SUFFIX}";
            
            return new InstantiateTemplateResponse()
            {
                Results = new ParameterCollection()
                {
                    { "EntityCollection", new EntityCollection(new List<Entity>() { email })}
                }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(InstantiateTemplateRequest);
        }

        private string GetXmlData(Entity regarding, Entity systemUser)
        {
            XElement dataElement = new XElement("data");
            
            dataElement.Add(GetXmlData(regarding));
            if (systemUser != null)
            {
                dataElement.Add(GetXmlData(systemUser));
            }
            
            XDocument doc = new XDocument(
                dataElement
            );

            return doc.ToString();
        }
        
        private XElement GetXmlData(Entity entity)
        {
            XElement entityRecordElement = new XElement(entity.LogicalName);
            
            foreach (var key in entity.Attributes.Keys)
            {
                var value = entity[key];
                
                if (entity.FormattedValues.ContainsKey(key))
                {
                    entityRecordElement.Add(new XElement(key, entity.FormattedValues[key]));
                }
                else if (value is Money money)
                {
                    entityRecordElement.Add(new XElement(key, money.Value));
                }
                else if (value is EntityReference er)
                {
                    entityRecordElement.Add(new XElement(key, er.Name));
                }
                else if (value is OptionSetValue osv)
                {
                    entityRecordElement.Add(new XElement(key, osv.Value));
                }
                else
                {
                    entityRecordElement.Add(new XElement(key, value));
                }
            }

            return entityRecordElement;
        }

        private string GetRenderedXslt(string attributeName, string xsltString, string dataXml)
        {
            
            var xslt = new XslCompiledTransform();
            using (var reader = XmlReader.Create(new StringReader(xsltString)))
            {
                try
                {
                    xslt.Load(reader);
                }
                catch (Exception)
                {
                    throw new InvalidXsltAttributeValueException(attributeName);
                }
            }

            using(var input = XmlReader.Create(new StringReader(dataXml)))
            using (var output = new StringWriter())
            {
                xslt.Transform(input, null, output);
                return output.ToString();
            }
        }
    }
}