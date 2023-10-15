using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveEntityRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveEntityRequest;
        }
        
        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveEntityResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as RetrieveEntityRequest;

            if (string.IsNullOrWhiteSpace(req.LogicalName))
            {
                throw new Exception("A logical name property must be specified in the request");
            }

            // HasFlag -> used to verify flag matches --> to verify EntityFilters.Entity | EntityFilters.Attributes
            if (req.EntityFilters.HasFlag(Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity) ||
                req.EntityFilters.HasFlag(Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes))
            {
                if(ctx.GetEntityMetadataByName(req.LogicalName) == null)
                {
                    throw new Exception($"Entity '{req.LogicalName}' is not found in the metadata cache");
                }

                var entityMetadata = ctx.GetEntityMetadataByName(req.LogicalName);

                var response = new RetrieveEntityResponse()
                {
                    Results = new ParameterCollection
                        {
                            { "EntityMetadata", entityMetadata }
                        }
                };

                return response;
            }

            throw new Exception("At least EntityFilters.Entity or EntityFilters.Attributes must be present on EntityFilters of Request.");
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveEntityRequest);
        }
    }
}
