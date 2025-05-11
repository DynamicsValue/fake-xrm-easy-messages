using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Provides a fake implementation for SendEmailFromTemplateRequest
    /// </summary>
    public class SendEmailFromTemplateRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is SendEmailFromTemplateRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>SendEmailFromTemplateResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as SendEmailFromTemplateRequest;

            if (req.Target == null)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    "Required field 'Target' is missing for RequestName='SendEmailFromTemplate'");
            }
            
            var instantiateEmailReq = new InstantiateTemplateRequest()
            {
                TemplateId = req.TemplateId,
                ObjectId = req.RegardingId,
                ObjectType = req.RegardingType
            };

            var service = ctx.GetOrganizationService();
            var instantiateResponse = service.Execute(instantiateEmailReq) as InstantiateTemplateResponse;
            var instantiatedEmail = instantiateResponse.EntityCollection.Entities[0];

            var email = req.Target;
            email["subject"] = instantiatedEmail["subject"];
            email["description"] = instantiatedEmail["description"];
            email["regardingobjectid"] = new EntityReference(req.RegardingType, req.RegardingId);
            
            var emailId = ctx.CreateEntity(email);

            var sendEmailRequest = new SendEmailRequest()
            {
                EmailId = emailId
            };
            service.Execute(sendEmailRequest);
            
            return new SendEmailFromTemplateResponse()
            {
                Results =  new ParameterCollection()
                    {
                        { "Id", emailId }
                    }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(SendEmailFromTemplateRequest);
        }
    }
}