using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class WhoAmIRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is WhoAmIRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>WhoAmIResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var callerId = ctx.CallerProperties.CallerId.Id;

            var results = new ParameterCollection {
                            { "UserId", callerId }
                            };

            var user = ctx.CreateQuery("systemuser")
                          .Where(u => u.Id == callerId)
                          .SingleOrDefault();

            if (user != null)
            {
                var buId = GetBusinessUnitId(user);
                results.Add("BusinessUnitId", buId);

                var orgId = GetOrganizationId(ctx, user, buId);
                results.Add("OrganizationId", orgId);
            }

            var response = new WhoAmIResponse
            {
                Results = results
            };
            return response;

        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(WhoAmIRequest);
        }

        private static Guid GetBusinessUnitId(Entity user)
        {
            var buRef = user.GetAttributeValue<EntityReference>("businessunitid");
            var buId = buRef != null ? buRef.Id : Guid.Empty;
            return buId;
        }

        private static Guid GetOrganizationId(IXrmFakedContext ctx, Entity user, Guid buId)
        {
            var orgId = user.GetAttributeValue<Guid?>("organizationid") ?? Guid.Empty;
            if (orgId == Guid.Empty)
            {
                var bu = ctx.CreateQuery("businessunit")
                            .Where(b => b.Id == buId)
                            .SingleOrDefault();
                var orgRef = bu.GetAttributeValue<EntityReference>("organizationid");
                orgId = orgRef?.Id ?? Guid.Empty;
            }

            return orgId;
        }
    }
}