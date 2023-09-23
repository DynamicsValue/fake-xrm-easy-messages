using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Diagnostics;
using System.Linq;
using FakeXrmEasy.Extensions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Metadata;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class InsertStatusValueRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InsertStatusValueRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>InsertStatusValueResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as InsertStatusValueRequest;

            Debug.Assert(req != null, nameof(req) + " != null");
            if (req.Label == null)
                throw new Exception("Label must not be null");

            if (string.IsNullOrWhiteSpace(req.Label.LocalizedLabels[0].Label))
            {
                throw new Exception("Label must not be empty");
            }

            if (string.IsNullOrEmpty(req.OptionSetName)
                && (string.IsNullOrEmpty(req.EntityLogicalName)
                || string.IsNullOrEmpty(req.AttributeLogicalName)))
            {
                throw new Exception("At least OptionSetName or both the EntityName and AttributeName must be provided");
            }

            bool isUsingOptionSet = !string.IsNullOrWhiteSpace(req.OptionSetName);

            var statusAttributeMetadataRepository = ctx.GetProperty<IStatusAttributeMetadataRepository>();
            
            StatusAttributeMetadata statusValuesMetadata = null;
            if(isUsingOptionSet)
            {
                statusValuesMetadata = statusAttributeMetadataRepository.GetByGlobalOptionSetName(req.OptionSetName);
            }
            else 
            {
                statusValuesMetadata = statusAttributeMetadataRepository.GetByAttributeName(req.EntityLogicalName, req.AttributeLogicalName);
            }

            if(statusValuesMetadata == null)
            {
                statusValuesMetadata = new StatusAttributeMetadata();
            }

            if(isUsingOptionSet)
            {
                statusAttributeMetadataRepository.Set(req.OptionSetName, statusValuesMetadata);
            }
            else 
            {
                statusAttributeMetadataRepository.Set(req.EntityLogicalName, req.AttributeLogicalName, statusValuesMetadata);
            }

            //statusValuesMetadata.
            statusValuesMetadata.OptionSet = new OptionSetMetadata();
            statusValuesMetadata.OptionSet.Options.Add(new StatusOptionMetadata()
            {
                MetadataId = Guid.NewGuid(),
                Value = req.Value,
                Label = req.Label,
                State = req.StateCode,
                Description = req.Label
            });
            

            if (!string.IsNullOrEmpty(req.EntityLogicalName))
            {
                var entityMetadata = ctx.GetEntityMetadataByName(req.EntityLogicalName);
                if (entityMetadata != null)
                {
                    var attribute = entityMetadata
                            .Attributes
                            .FirstOrDefault(a => a.LogicalName == req.AttributeLogicalName);

                    if (attribute == null)
                    {
                        throw new Exception($"You are trying to insert an option set value for entity '{req.EntityLogicalName}' with entity metadata associated but the attribute '{req.AttributeLogicalName}' doesn't exist in metadata");
                    }

                    if (!(attribute is EnumAttributeMetadata))
                    {
                        throw new Exception($"You are trying to insert an option set value for entity '{req.EntityLogicalName}' with entity metadata associated but the attribute '{req.AttributeLogicalName}' is not a valid option set field (not a subtype of EnumAttributeMetadata)");
                    }                    

                    var enumAttribute = attribute as EnumAttributeMetadata;

                    var options = enumAttribute.OptionSet == null ? new OptionMetadataCollection() : enumAttribute.OptionSet.Options;
                    
                    options.Add(new StatusOptionMetadata(){Value = req.Value, Label = req.Label, State = req.StateCode, Description = req.Label});

                    enumAttribute.OptionSet = new OptionSetMetadata(options);                    

                    entityMetadata.SetAttribute(enumAttribute);
                    ctx.SetEntityMetadata(entityMetadata);
                }
            }
            return new InsertStatusValueResponse();
        }       

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(InsertStatusValueRequest);
        }
    }
}