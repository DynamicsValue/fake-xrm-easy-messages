using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class AddToQueueRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is AddToQueueRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>AddToQueueResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var addToQueueRequest = (AddToQueueRequest)request;

            var target = addToQueueRequest.Target;
            var destinationQueueId = addToQueueRequest.DestinationQueueId;
            var queueItemProperties = addToQueueRequest.QueueItemProperties;

            if (target == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not add to queue without target");
            }

            if (destinationQueueId == Guid.Empty)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not add to queue without destination queue");
            }

            var service = ctx.GetOrganizationService();

            // CRM updates existing queue item if one already exists for a given objectid
            var existingQueueItem = service.RetrieveMultiple(new QueryExpression
            {
                EntityName = "queueitem",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression("objectid", ConditionOperator.Equal, target.Id)
                        }
                }
            }).Entities.FirstOrDefault();

            var createQueueItem = existingQueueItem ?? new Entity
            {
                LogicalName = "queueitem",
                // QueueItemProperties are used for initializing new queueitems
                Attributes = queueItemProperties?.Attributes
            };

            createQueueItem["queueid"] = new EntityReference("queue", destinationQueueId);
            createQueueItem["objectid"] = target;

            var guid = service.Create(createQueueItem);

            return new AddToQueueResponse()
            {
                ResponseName = "AddToQueue",
                Results = new ParameterCollection { { "QueueItemId", guid } }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(AddToQueueRequest);
        }
    }
}