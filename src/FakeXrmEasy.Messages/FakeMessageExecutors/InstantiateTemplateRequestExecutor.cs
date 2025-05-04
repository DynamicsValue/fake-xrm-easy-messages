using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Provides a fake implementation for an InstantiateTemplateRequest
    /// </summary>
    public class InstantiateTemplateRequestExecutor: IFakeMessageExecutor
    {
        private const string TEMPLATE_ENTITY_LOGICAL_NAME = "template";
        
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InstantiateTemplateRequest;
        }

        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as InstantiateTemplateRequest;
            
            //Check if template exists
            var template = ctx.GetEntityById(TEMPLATE_ENTITY_LOGICAL_NAME, req.TemplateId);
            if (template == null)
            {
                throw new Exception();
            }

            return new InstantiateTemplateResponse()
            {

            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(InstantiateTemplateRequest);
        }
    }
}