using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class AssignRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is AssignRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>AssignResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var assignRequest = (AssignRequest)request;

            var target = assignRequest.Target;
            var assignee = assignRequest.Assignee;

            if (target == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not assign without target");
            }

            if (assignee == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not assign without assignee");
            }

            var service = ctx.GetOrganizationService();

            KeyValuePair<string, object> owningX = new KeyValuePair<string, object>();
            if (assignee.LogicalName == "systemuser")
                owningX = new KeyValuePair<string, object>("owninguser", assignee);
            else if (assignee.LogicalName == "team")
                owningX = new KeyValuePair<string, object>("owningteam", assignee);

            var assignment = new Entity
            {
                LogicalName = target.LogicalName,
                Id = target.Id,
                Attributes = new AttributeCollection
                {
                    { "ownerid", assignee },
                    owningX
                }
            };

            service.Update(assignment);

            return new AssignResponse();
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(AssignRequest);
        }
    }
}