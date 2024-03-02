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
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveCurrentOrganizationRequest;
        }

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
                        { EndpointType.OrganizationDataService, "http://baseUrl/XrmServices/2011/OrganizationDataService.svc" }
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

        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveCurrentOrganizationRequest);
        }
    }
}

#endif