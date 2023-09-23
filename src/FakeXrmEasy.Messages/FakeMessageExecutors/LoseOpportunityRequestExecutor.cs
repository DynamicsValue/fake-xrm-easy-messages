using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class LoseOpportunityRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is LoseOpportunityRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>LoseOpportunityResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as LoseOpportunityRequest;

            // Check if OpportunityClose and Status were passed to request
            if (req.OpportunityClose != null &&
                req.Status != null)
            {
                // LoseOpportunityRequest.OpportunityClose.OpportunityId
                var opportunityReference = req.OpportunityClose.GetAttributeValue<EntityReference>("opportunityid");
                var opportunityId = opportunityReference.Id;

                // Get Opportunities (in good scenario, should return 1 record)
                var opportunities = (from op in ctx.CreateQuery("opportunity")
                                     where op.Id == opportunityId
                                     select op);

                // More than one if to check and give better feedback to user
                if (opportunities.Count() < 1) throw new Exception(string.Format("No Opportunity found with Id = {0}", opportunityId));
                else if (opportunities.Count() > 1) throw new Exception(string.Format("More than one Opportunity found with Id = {0}", opportunityId));
                else
                {
                    var opportunity = opportunities.FirstOrDefault();
                    opportunity.Attributes["statuscode"] = req.Status;

                    ctx.GetOrganizationService().Update(opportunity);

                    return new LoseOpportunityResponse();
                }
            }
            else
            {
                throw new Exception("OpportunityClose or Status was not passed to request.");
            }
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(LoseOpportunityRequest);
        }
    }
}