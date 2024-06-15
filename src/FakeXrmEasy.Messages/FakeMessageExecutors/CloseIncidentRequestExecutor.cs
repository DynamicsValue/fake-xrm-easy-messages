using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class CloseIncidentRequestExecutor : IFakeMessageExecutor
    {
        private const string AttributeIncidentId = "incidentid";
        private const string AttributeSubject = "subject";
        private const string IncidentLogicalName = "incident";
        private const string IncidentResolutionLogicalName = "incidentresolution";
        private const int StateResolved = 1;

        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is CloseIncidentRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>CloseIncidentResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var service = ctx.GetOrganizationService();
            var closeIncidentRequest = (CloseIncidentRequest)request;

            var incidentResolution = closeIncidentRequest.IncidentResolution;
            if (incidentResolution == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Cannot close incident without incident resolution.");
            }

            var status = closeIncidentRequest.Status;
            if (status == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Cannot close incident without status.");
            }

            var incidentId = (EntityReference)incidentResolution[AttributeIncidentId];
            if (!ctx.ContainsEntity(IncidentLogicalName,incidentId.Id))
            {
                throw FakeOrganizationServiceFaultFactory.New(string.Format("Incident with id {0} not found.", incidentId.Id));
            }

            var newIncidentResolution = new Entity
            {
                LogicalName = IncidentResolutionLogicalName,
                Attributes = new AttributeCollection
                {
                    { "description", incidentResolution[AttributeSubject] },
                    { AttributeSubject, incidentResolution[AttributeSubject] },
                    { AttributeIncidentId, incidentId }
                }
            };
            service.Create(newIncidentResolution);

            var setState = new SetStateRequest
            {
                EntityMoniker = incidentId,
                Status = status,
                State = new OptionSetValue(StateResolved)
            };

            service.Execute(setState);

            return new CloseIncidentResponse();
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(CloseIncidentRequest);
        }
    }
}