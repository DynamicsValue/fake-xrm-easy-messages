using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class CloseQuoteRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is CloseQuoteRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>CloseQuoteResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var closeRequest = request as CloseQuoteRequest;

            if (closeRequest == null)
            {
                throw new Exception("You did not pass a CloseQuoteRequest");
            }

            var quoteClose = closeRequest.QuoteClose;

            if (quoteClose == null)
            {
                throw new Exception("QuoteClose is mandatory");
            }

            var quoteId = quoteClose.GetAttributeValue<EntityReference>("quoteid");

            if (quoteId == null)
            {
                throw new Exception("Quote ID is not set on QuoteClose, but is required");
            }

            var update = new Entity
            {
                Id = quoteId.Id,
                LogicalName = "quote",
                Attributes = new AttributeCollection
                {
                    { "statuscode", closeRequest.Status }
                }
            };

            var service = ctx.GetOrganizationService();

            service.Update(update);

            return new CloseQuoteResponse();
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(CloseQuoteRequest);
        }
    }
}