using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Provides a fake implementation for SendTemplateRequest
    /// </summary>
    public class SendTemplateRequestExecutor: IFakeMessageExecutor
    {
        private const string MISSING_ARGUMENT = "Required field '{0}' is missing for RequestName='SendTemplate'";
        
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is SendTemplateRequest;
        }

        private void ValidateRecipients(IXrmFakedContext ctx, Guid[] ids, string recipientType)
        {
            foreach (var id in ids)
            {
                if (!ctx.ContainsEntity(recipientType, id))
                {
                    throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.unManagedidscommunicationsnopartyaddress,
                        "Object address not found on party or party is marked as non-emailable");
                }
            }
        }

        private void SendEmailFromTemplate(IXrmFakedContext ctx, SendTemplateRequest request)
        {
            var ids = request.RecipientIds;
            foreach (var id in ids)
            {
                var instantiateEmailReq = new InstantiateTemplateRequest()
                {
                    TemplateId = request.TemplateId,
                    ObjectId = id,
                    ObjectType = request.RecipientType
                };

                var service = ctx.GetOrganizationService();
                var instantiateResponse = service.Execute(instantiateEmailReq) as InstantiateTemplateResponse;
                var instantiatedEmail = instantiateResponse.EntityCollection.Entities[0];

                var email = ctx.NewEntityRecord("email");
                email["subject"] = instantiatedEmail["subject"];
                email["description"] = instantiatedEmail["description"];
                email["regardingobjectid"] = new EntityReference(request.RegardingType, request.RegardingId);
            
                var emailId = ctx.CreateEntity(email);

                var sendEmailRequest = new SendEmailRequest()
                {
                    EmailId = emailId
                };
                service.Execute(sendEmailRequest);
            }
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>SendEmailResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as SendTemplateRequest;

            if (req.Sender == null)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    string.Format(MISSING_ARGUMENT, "Sender"));
            }
            
            if (req.RecipientType == null)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    string.Format(MISSING_ARGUMENT, "RecipientType"));
            }
            
            if (req.RecipientIds == null)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    string.Format(MISSING_ARGUMENT, "RecipientIds"));
            }
            
            if (req.RecipientIds.Length == 0)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument,
                    "Recipients should be set");
            }

            ValidateRecipients(ctx, req.RecipientIds, req.RecipientType);
            
            if (!ctx.ContainsEntity("template", req.TemplateId))
            {
                return new SendTemplateResponse();
            }

            var template = ctx.GetEntityById("template", req.TemplateId);
            if (!req.RegardingType.Equals(template.GetAttributeValue<string>("templatetypecode")) && !req.RecipientType.Equals(template.GetAttributeValue<string>("templatetypecode")))
            {
                return new SendTemplateResponse();
            }

            if (!ctx.ContainsEntity(req.RegardingType, req.RegardingId))
            {
                return new SendTemplateResponse();
            }
            
            SendEmailFromTemplate(ctx, req);

            return new SendTemplateResponse();
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(SendTemplateRequest);
        }
    }
}