#if FAKE_XRM_EASY_9 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_2015

using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Messages.ContextProperties;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Organization;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements an executor for the RetrieveCurrentOrganizationRequest: https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.retrievecurrentorganizationrequest?view=dataverse-sdk-latest
    /// </summary>
    public class RetrieveCurrentOrganizationRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveCurrentOrganizationRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveCurrentOrganizationResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            OrganizationDetail orgDetail = null; 
            if (ctx.HasProperty<CurrentOrganizationDetails>())
            {
                orgDetail = ctx.GetProperty<CurrentOrganizationDetails>().Details;
            }

            if (orgDetail == null)
            {
                orgDetail = new OrganizationDetail()
                {
                    Endpoints = 
                    {
                        { EndpointType.OrganizationDataService, "https://baseUrl/XrmServices/2011/OrganizationDataService.svc" }
                    }
                };
            }
            return new RetrieveCurrentOrganizationResponse()
            {
                Results = new ParameterCollection
                {
                    { "Detail", orgDetail }
                }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveCurrentOrganizationRequest);
        }
    }
}

#endif