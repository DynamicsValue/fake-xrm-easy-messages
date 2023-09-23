using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class ExecuteMultipleRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is ExecuteMultipleRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>ExecuteMultipleResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var executeMultipleRequest = (ExecuteMultipleRequest)request;

            if (executeMultipleRequest.Settings == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("You need to pass a value for 'Settings' in execute multiple request");
            }

            if (executeMultipleRequest.Requests == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("You need to pass a value for 'Requests' in execute multiple request");
            }

            var service = ctx.GetOrganizationService();

            var response = new ExecuteMultipleResponse();
            response.Results["Responses"] = new ExecuteMultipleResponseItemCollection();

            for (var i = 0; i < executeMultipleRequest.Requests.Count; i++)
            {
                var executeRequest = executeMultipleRequest.Requests[i];

                try
                {
                    OrganizationResponse resp = service.Execute(executeRequest);

                    if (executeMultipleRequest.Settings.ReturnResponses)
                    {
                        response.Responses.Add(new ExecuteMultipleResponseItem
                        {
                            RequestIndex = i,
                            Response = resp
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (!response.IsFaulted)
                    {
                        response.Results["IsFaulted"] = true;
                    }

                    response.Responses.Add(new ExecuteMultipleResponseItem
                    {
                        Fault = new OrganizationServiceFault { Message = ex.Message },
                        RequestIndex = i
                    });

                    if (!executeMultipleRequest.Settings.ContinueOnError)
                    {
                        break;
                    }
                }
            }

            // Implement response behaviour as in https://msdn.microsoft.com/en-us/library/jj863631.aspx
            if (executeMultipleRequest.Settings.ReturnResponses)
            {
                response.Results["response.Responses"] = response.Responses;
            }
            else if (response.Responses.Any(resp => resp.Fault != null))
            {
                var failures = new ExecuteMultipleResponseItemCollection();

                failures.AddRange(response.Responses.Where(resp => resp.Fault != null));

                response.Results["response.Responses"] = failures;
            }

            return response;
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(ExecuteMultipleRequest);
        }
    }
}