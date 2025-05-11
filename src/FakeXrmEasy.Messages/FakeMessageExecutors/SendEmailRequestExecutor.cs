using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.EmailSettings;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class SendEmailRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is SendEmailRequest;
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
            var req = request as SendEmailRequest;
#if FAKE_XRM_EASY || FAKE_XRM_EASY_2013
            var entity = new Entity("email");
            entity.Id = req.EmailId;
#else
            var entity = new Entity("email", req.EmailId);
#endif

            var emailExists = ctx.ContainsEntity("email", req.EmailId);
            if (!emailExists)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist,
                    $"Entity 'email' With Id {req.EmailId} Does Not Exist");
            }
            
            var existingEmail = ctx.GetEntityById("email", req.EmailId);
            
            entity["statecode"] = new OptionSetValue(1); //Completed
            entity["statuscode"] = new OptionSetValue(3); //Sent

            string trackingToken = req.TrackingToken;
            if (string.IsNullOrWhiteSpace(trackingToken))
            {
                var emailTrackingSettings = ctx.GetProperty<IEmailTrackingSettings>();
                trackingToken = emailTrackingSettings.GenerateNewTrackingTokenValue();
            }
            entity["trackingtoken"] = trackingToken;
            entity["actualend"] = DateTime.UtcNow;
            entity["deliveryattempts"] = 0;

            string newSubject = $"{existingEmail["subject"]} {trackingToken}";
            entity["subject"] = newSubject;
            
            ctx.GetOrganizationService().Update(entity);

            return new SendEmailResponse()
            {
                Results =  new ParameterCollection()
                    {
                        { "Subject", newSubject }
                    }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(SendEmailRequest);
        }
    }
}
