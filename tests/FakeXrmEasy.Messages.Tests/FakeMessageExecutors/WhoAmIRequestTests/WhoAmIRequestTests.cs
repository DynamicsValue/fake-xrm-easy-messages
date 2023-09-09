using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Crm;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.WhoAmIRequestTests
{
    public class WhoAmIRequestTests : FakeXrmEasyTestsBase
    {
        private readonly Entity _organization;

        public WhoAmIRequestTests() : base()
        {
            _organization = new Organization() { Id = Guid.NewGuid() };
        }
        
        [Fact]
        public void When_a_who_am_i_request_is_invoked_the_caller_id_is_returned()
        {
            _context.CallerProperties.CallerId = new EntityReference() { Id = Guid.NewGuid(), Name = "Super Faked User" };

            WhoAmIRequest req = new WhoAmIRequest();

            var response = _service.Execute(req) as WhoAmIResponse;
            Assert.Equal(response.UserId, _context.CallerProperties.CallerId.Id);
        }

        [Fact]
        public void When_a_who_am_i_request_is_invoked_the_organization_is_returned_when_the_user_belongs_to_an_organization()
        {
            var user = new SystemUser()
            {
                Id = Guid.NewGuid(),
                ["organizationid"] = (Guid?)_organization.Id
            };

            var dbContent = new List<Entity> {
              user,
              _organization
            };

            _context.CallerProperties.CallerId = new EntityReference() { Id = user.Id, Name = "Super Faked User" };
            _context.Initialize(dbContent);

            var req = new WhoAmIRequest();
            var response = _service.Execute(req) as WhoAmIResponse;

            Assert.Equal(user.Id, response.UserId);
            Assert.Equal(_organization.Id, response.OrganizationId);
        }

        [Fact]
        public void When_a_who_am_i_request_is_invoked_the_business_unit_is_returned_when_the_user_belongs_to_a_business_unit()
        {
            var businessUnit = new BusinessUnit() { Id = Guid.NewGuid() };

            var user = new SystemUser()
            {
                Id = Guid.NewGuid(),
                ["businessunitid"] = businessUnit.ToEntityReference()
            };

            var dbContent = new List<Entity> {
              user,
              businessUnit
            };

            _context.CallerProperties.CallerId = new EntityReference() { Id = user.Id, Name = "Super Faked User" };
            _context.Initialize(dbContent);

            var req = new WhoAmIRequest();
            var response = _service.Execute(req) as WhoAmIResponse;

            Assert.Equal(user.Id, response.UserId);
            Assert.Equal(businessUnit.Id, response.BusinessUnitId);
        }

        [Fact]
        public void When_a_who_am_i_request_is_invoked_the_business_unit_and_organisation_are_returned_when_the_user_belongs_to__business_unit_and_organisation()
        {
            var businessUnit = new BusinessUnit() { Id = Guid.NewGuid() };
            
            var user = new SystemUser()
            {
                Id = Guid.NewGuid(),
                ["businessunitid"] = businessUnit.ToEntityReference(),
                ["organizationid"] = (Guid?)_organization.Id
            };

            var dbContent = new List<Entity> {
              user,
              businessUnit,
              _organization
            };

            _context.CallerProperties.CallerId = new EntityReference() { Id = user.Id, Name = "Super Faked User" };
            _context.Initialize(dbContent);

            var req = new WhoAmIRequest();
            var response = _service.Execute(req) as WhoAmIResponse;

            Assert.Equal(user.Id, response.UserId);
            Assert.Equal(businessUnit.Id, response.BusinessUnitId);
            Assert.Equal(_organization.Id, response.OrganizationId);
        }

        [Fact]
        public void When_a_who_am_i_request_is_invoked_the_business_unit_and_organisation_are_returned_when_the_user_belongs_to_a_business_unit_and_the_business_unit_has_an_organisation()
        {

            var businessUnit = new BusinessUnit()
            {
                Id = Guid.NewGuid(),
                ["organizationid"] = _organization.ToEntityReference()
            };

            var user = new SystemUser()
            {
                Id = Guid.NewGuid(),
                ["businessunitid"] = businessUnit.ToEntityReference(),
            };

            var dbContent = new List<Entity> {
              user,
              businessUnit,
              _organization
            };

            _context.CallerProperties.CallerId = new EntityReference() { Id = user.Id, Name = "Super Faked User" };
            _context.Initialize(dbContent);

            var req = new WhoAmIRequest();
            var response = _service.Execute(req) as WhoAmIResponse;

            Assert.Equal(user.Id, response.UserId);
            Assert.Equal(businessUnit.Id, response.BusinessUnitId);
            Assert.Equal(_organization.Id, response.OrganizationId);
        }
    }
}