using System;
using FakeXrmEasy.Messages.ContextProperties;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Organization;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.RetrieveCurrentOrganizationRequestTests
{
    public class RetrieveCurrentOrganizationRequestTests: FakeXrmEasyTestsBase
    {
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
                Details = new OrganizationDetail()
                {
                    Geo = "Geo",
                    EnvironmentId = Guid.NewGuid().ToString(),
                    State = OrganizationState.Enabled,
                    FriendlyName = "OrgFriendlyName",
                    UniqueName = "UniqueName",
                    UrlName = "UrlName",
                    OrganizationId = Guid.NewGuid(),
                    TenantId = Guid.NewGuid().ToString(),
                    OrganizationVersion = "0.0.0.0",
                    Endpoints =
                    {
                        { EndpointType.OrganizationDataService, "http://localhost/XrmService/2011/OrganizationData.svc" }
                    }
                }
            };
            
            _context.SetProperty(currentOrgDetails);
            
            var response = _service.Execute(new RetrieveCurrentOrganizationRequest());
            Assert.NotNull(response);

            var orgDetail = (response as RetrieveCurrentOrganizationResponse).Detail;
            Assert.Equal(currentOrgDetails.Details.EnvironmentId, orgDetail.EnvironmentId);
            Assert.Equal(currentOrgDetails.Details.Geo, orgDetail.Geo);
            Assert.Equal(currentOrgDetails.Details.State, orgDetail.State);
            Assert.Equal(currentOrgDetails.Details.FriendlyName, orgDetail.FriendlyName);
            Assert.Equal(currentOrgDetails.Details.UniqueName, orgDetail.UniqueName);
            Assert.Equal(currentOrgDetails.Details.UrlName, orgDetail.UrlName);
            Assert.Equal(currentOrgDetails.Details.OrganizationId, orgDetail.OrganizationId);
            Assert.Equal(currentOrgDetails.Details.TenantId, orgDetail.TenantId);
            Assert.Equal(currentOrgDetails.Details.OrganizationVersion, orgDetail.OrganizationVersion);
        }
    }
}