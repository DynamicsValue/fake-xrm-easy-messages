using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveAttributeRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveAttributeRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveAttributeResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as RetrieveAttributeRequest;

            if (string.IsNullOrWhiteSpace(req.EntityLogicalName))
            {
                throw new Exception("The EntityLogicalName property must be provided in this request");
            }

            if (string.IsNullOrWhiteSpace(req.LogicalName))
            {
                throw new Exception("The LogicalName property must be provided in this request");
            }

            var entityMetadata = ctx.GetEntityMetadataByName(req.EntityLogicalName);
            if(entityMetadata == null)
            {
                throw new Exception(string.Format("The entity metadata with logical name {0} wasn't initialized. Please use .InitializeMetadata", req.EntityLogicalName));
            }

            if(entityMetadata.Attributes == null)
            {
                throw new Exception(string.Format("The attribute {0} wasn't found in entity metadata with logical name {1}. ", req.LogicalName, req.EntityLogicalName));
            }

            var attributeMetadata = entityMetadata.Attributes
                                    .FirstOrDefault(a => a.LogicalName.Equals(req.LogicalName));

            if (attributeMetadata == null)
            {
                throw new Exception(string.Format("The attribute {0} wasn't found in entity metadata with logical name {1}. ", req.LogicalName, req.EntityLogicalName));
            }

            var response = new RetrieveAttributeResponse()
            {
                Results = new ParameterCollection
                {
                    { "AttributeMetadata", attributeMetadata }
                }
            };

            return response;
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveAttributeRequest);
        }
    }
}