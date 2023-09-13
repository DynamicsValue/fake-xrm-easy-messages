#if FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_9

using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class ExecuteTransactionExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is ExecuteTransactionRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>ExecuteTransactionResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var executeTransactionRequest = (ExecuteTransactionRequest)request;
            var response = new ExecuteTransactionResponse { ["Responses"] = new OrganizationResponseCollection() };

            var service = ctx.GetOrganizationService();

            foreach (var r in executeTransactionRequest.Requests)
            {
                var result = service.Execute(r);

                if (executeTransactionRequest.ReturnResponses.HasValue && executeTransactionRequest.ReturnResponses.Value)
                {
                    response.Responses.Add(result);
                }
            }
            return response;
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(ExecuteTransactionRequest);
        }
    }
}
#endif