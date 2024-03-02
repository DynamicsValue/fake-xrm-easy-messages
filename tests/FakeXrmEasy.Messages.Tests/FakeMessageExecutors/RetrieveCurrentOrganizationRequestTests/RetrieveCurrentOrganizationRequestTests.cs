#if FAKE_XRM_EASY_9 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_2015

using System;
using FakeXrmEasy.Messages.ContextProperties;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Organization;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.RetrieveCurrentOrganizationRequestTests
{
    public class RetrieveCurrentOrganizationRequestTests: FakeXrmEasyTestsBase
    {
        private readonly OrganizationDetail _organizationDetail;

        public RetrieveCurrentOrganizationRequestTests() : base()
        {
            _organizationDetail = new OrganizationDetail()
            {
                #if FAKE_XRM_EASY_9
                Geo = "Geo",
                EnvironmentId = Guid.NewGuid().ToString(),
                TenantId = Guid.NewGuid().ToString(),
                #endif
                State = OrganizationState.Enabled,
                FriendlyName = "OrgFriendlyName",
                UniqueName = "UniqueName",
                UrlName = "UrlName",
                OrganizationId = Guid.NewGuid(),
                OrganizationVersion = "0.0.0.0",
                Endpoints =
                {
                    { EndpointType.OrganizationDataService, "http://localhost/XrmService/2011/OrganizationData.svc" }
                }
            };
        }
        [Fact]
        public void Should_retrieve_current_organization_details_if_none_was_set_by_default()
        {
            var response = _service.Execute(new RetrieveCurrentOrganizationRequest());
            Assert.NotNull(response);

            var orgDetail = (response as RetrieveCurrentOrganizationResponse).Detail;
            Assert.NotNull(orgDetail);
        }
        
        [Fact]
        public void Should_retrieve_current_organization_details()
        {
            var currentOrgDetails = new CurrentOrganizationDetails()
            {
                Details = _organizationDetail
            };
            
            _context.SetProperty(currentOrgDetails);
            
            var response = _service.Execute(new RetrieveCurrentOrganizationRequest());
            Assert.NotNull(response);

            var orgDetail = (response as RetrieveCurrentOrganizationResponse).Detail;
            #if FAKE_XRM_EASY_9
            Assert.Equal(currentOrgDetails.Details.EnvironmentId, orgDetail.EnvironmentId);
            Assert.Equal(currentOrgDetails.Details.Geo, orgDetail.Geo);
            Assert.Equal(currentOrgDetails.Details.TenantId, orgDetail.TenantId);
            #endif
            Assert.Equal(currentOrgDetails.Details.State, orgDetail.State);
            Assert.Equal(currentOrgDetails.Details.FriendlyName, orgDetail.FriendlyName);
            Assert.Equal(currentOrgDetails.Details.UniqueName, orgDetail.UniqueName);
            Assert.Equal(currentOrgDetails.Details.UrlName, orgDetail.UrlName);
            Assert.Equal(currentOrgDetails.Details.OrganizationId, orgDetail.OrganizationId);
            Assert.Equal(currentOrgDetails.Details.OrganizationVersion, orgDetail.OrganizationVersion);
        }
    }
}

#endif