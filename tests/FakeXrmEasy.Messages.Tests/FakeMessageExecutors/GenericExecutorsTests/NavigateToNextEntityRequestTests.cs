using Crm;
using FakeXrmEasy.FakeMessageExecutors.GenericExecutors;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using FakeXrmEasy.Messages.Exceptions.NavigateToNextEntityOrganizationRequest;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.GenericExecutors.NavigateToNextEntityRequestTests
{
    public class NavigateToNextEntityRequestTests : FakeXrmEasyTestsBase
    {
        private readonly Workflow _workflow;
        private readonly Contract _contract;
        private readonly Opportunity _opportunity;
        private readonly ProcessStage _currentStage;
        private readonly ProcessStage _nextStage;
        private readonly OrganizationRequest _navigateToNextStageRequest;
        public NavigateToNextEntityRequestTests() : base()
        {
            _workflow = new Workflow() { Id = Guid.NewGuid() };
            _contract = new Contract() { Id = Guid.NewGuid() };
            
            _currentStage = new ProcessStage()
            {
                Id = Guid.NewGuid(),
                ProcessId = _workflow.ToEntityReference()
            };
            _nextStage = new ProcessStage()
            {
                Id = Guid.NewGuid(),
                ProcessId = _workflow.ToEntityReference()
            };
            _opportunity = new Opportunity()
            {
                Id = Guid.NewGuid(),
                StageId = _currentStage.Id
            };
            
            _navigateToNextStageRequest = new OrganizationRequest(NavigateToNextEntityOrganizationRequestExecutor.RequestName);
            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterProcessId, _workflow.Id);
            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterNewActiveStageId, _nextStage.Id);

            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterCurrentEntityLogicalName, _opportunity.LogicalName);
            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterCurrentEntityId, _opportunity.Id);

            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterNextEntityLogicalName, _contract.LogicalName);
            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterNextEntityId, _contract.Id);

            _navigateToNextStageRequest.Parameters.Add(NavigateToNextEntityOrganizationRequestExecutor.ParameterNewTraversedPath, string.Join(",", _currentStage.Id, _nextStage.Id));
        }
        
        [Fact]
        public void Should_move_to_next_stage_in_workflow()
        {
            _context.Initialize(new Entity[] { _workflow, _contract, _opportunity, _currentStage, _nextStage });

            // Execute
            var response = _service.Execute(_navigateToNextStageRequest);
            var traversedPath = response.Results[NavigateToNextEntityOrganizationRequestExecutor.ParameterTraversedPath];

            var oppAfterSet = (from o in _context.CreateQuery("opportunity")
                               where o.Id == _opportunity.Id
                               select o).First();

            Assert.True(response != null);
            Assert.True(traversedPath.ToString() == (_currentStage.Id + "," + _nextStage.Id));
            Assert.True(traversedPath.ToString() == oppAfterSet["traversedpath"].ToString());
        }
        
        [Fact]
        public void Should_throw_exception_if_current_entity_record_does_not_exist()
        {
            _context.Initialize(new Entity[] { _workflow, _contract, _currentStage, _nextStage });
            Assert.Throws<CurrentEntityNotFoundException>( () => _service.Execute(_navigateToNextStageRequest));
        }
        
        [Fact]
        public void Should_throw_exception_if_next_entity_record_does_not_exist()
        {
            _context.Initialize(new Entity[] { _workflow, _opportunity, _currentStage, _nextStage });
            Assert.Throws<NextEntityNotFoundException>( () => _service.Execute(_navigateToNextStageRequest));
        }
    }
}