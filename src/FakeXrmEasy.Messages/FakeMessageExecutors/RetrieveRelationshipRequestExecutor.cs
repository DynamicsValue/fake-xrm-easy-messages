using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveRelationshipRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveRelationshipRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveRelationshipResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var retrieveRequest = request as RetrieveRelationshipRequest;
            if (retrieveRequest == null)
            {
                throw new Exception("Only RetrieveRelationshipRequest can be processed!");
            }

            var fakeRelationShip = ctx.GetRelationship(retrieveRequest.Name);
            if (fakeRelationShip == null)
            {
                throw new Exception(string.Format("Relationship {0} does not exist in the metadata cache", retrieveRequest.Name));
            }

            
            var response = new RetrieveRelationshipResponse();
            response.Results = new ParameterCollection();
            response.Results.Add("RelationshipMetadata", GetRelationshipMetadata(fakeRelationShip));
            response.ResponseName = "RetrieveRelationship";

            return response;
        }

        private static object GetRelationshipMetadata(XrmFakedRelationship fakeRelationShip)
        {
            if (fakeRelationShip.RelationshipType == XrmFakedRelationship.FakeRelationshipType.ManyToMany)
            {
                var mtm = new Microsoft.Xrm.Sdk.Metadata.ManyToManyRelationshipMetadata();
                mtm.Entity1LogicalName = fakeRelationShip.Entity1LogicalName;
                mtm.Entity1IntersectAttribute = fakeRelationShip.Entity1Attribute;
                mtm.Entity2LogicalName = fakeRelationShip.Entity2LogicalName;
                mtm.Entity2IntersectAttribute = fakeRelationShip.Entity2Attribute;
                mtm.SchemaName = fakeRelationShip.IntersectEntity;
                mtm.IntersectEntityName = fakeRelationShip.IntersectEntity.ToLower();
                return mtm;
            } else {

                var otm = new Microsoft.Xrm.Sdk.Metadata.OneToManyRelationshipMetadata();
#if FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_9
                otm.ReferencedEntityNavigationPropertyName = fakeRelationShip.IntersectEntity;
#endif
                otm.ReferencingAttribute = fakeRelationShip.Entity1Attribute;
                otm.ReferencingEntity = fakeRelationShip.Entity1LogicalName;
                otm.ReferencedAttribute = fakeRelationShip.Entity2Attribute;
                otm.ReferencedEntity = fakeRelationShip.Entity2LogicalName;
                otm.SchemaName = fakeRelationShip.IntersectEntity;
                return otm;
            }
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveRelationshipRequest);
        }
    }
}