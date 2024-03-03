using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Crm;
using System.ServiceModel;
using FakeXrmEasy.Abstractions;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.AddMembersTeamRequestTests
{
    public class AddMembersTeamRequestTests : FakeXrmEasyTestsBase
    {
        private readonly SystemUser _systemUser;
        private readonly BusinessUnit _businessUnit;
        private readonly Team _team;

        public AddMembersTeamRequestTests(): base()
        {
            _businessUnit = new BusinessUnit() { Id = Guid.NewGuid() };
            _systemUser = new SystemUser() { Id = Guid.NewGuid() };
            _team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Some team"
            };
        }
        
        [Fact]
        public void When_a_member_is_added_to_a_non_existing_team_exception_is_thrown()
        {
            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    _systemUser.Id
                },
                TeamId = _team.Id
            };

            // Execute the request.
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
        }

        [Fact]
        public void When_a_request_is_called_with_an_empty_teamId_parameter_exception_is_thrown()
        {
            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    _systemUser.Id
                },
                TeamId = Guid.Empty
            };

            // Execute the request.
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
        }

        [Fact]
        public void When_a_request_is_called_with_a_null_memberId_parameter_exception_is_thrown()
        {
            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = null,
                TeamId = _team.Id
            };

            // Execute the request.
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
        }

        [Fact]
        public void When_a_request_is_called_with_an_empty_memberId_parameter_exception_is_thrown()
        {
            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    Guid.Empty
                },
                TeamId = _team.Id
            };

            // Execute the request.
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
        }

        [Fact]
        public void When_a_non_existing_member_is_added_to_an_existing_list_exception_is_thrown()
        {
            _context.Initialize(new List<Entity>
            {
                _team
            });

            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    Guid.NewGuid()
                },
                TeamId = _team.Id
            };

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
        }

        [Fact]
        public void When_a_member_is_added_to_an_existing_list_member_is_added_successfully()
        {
            _context.Initialize(new List<Entity>
            {
                _team,
                _systemUser
            });

            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    _systemUser.Id
                },
                TeamId = _team.Id
            };

            _service.Execute(addMembersTeamRequest);

            var member = _context.CreateQuery<TeamMembership>()
                .FirstOrDefault(tm => tm.TeamId == _team.Id && tm.SystemUserId == _systemUser.Id);

            Assert.NotNull(member);
        }
        
        [Fact]
        public void Should_raise_exception_when_a_user_is_added_to_a_default_team()
        {
            _team["isdefault"] = true;
            
            _context.Initialize(new List<Entity>
            {
                _team,
                _systemUser
            });

            AddMembersTeamRequest addMembersTeamRequest = new AddMembersTeamRequest
            {
                MemberIds = new[]
                {
                    _systemUser.Id
                },
                TeamId = _team.Id
            };

            var exception = Assert.Throws<FaultException<OrganizationServiceFault>>(() => _service.Execute(addMembersTeamRequest));
            Assert.Equal((int) ErrorCodes.CannotAddMembersToDefaultTeam, exception.Detail.ErrorCode);
        }
    }
}