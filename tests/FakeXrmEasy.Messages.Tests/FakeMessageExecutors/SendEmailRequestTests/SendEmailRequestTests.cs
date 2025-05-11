using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Reflection;
using DataverseEntities;
using FakeXrmEasy.Abstractions;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.SendEmailRequestTests
{
    public class SendEmailRequestTests: FakeXrmEasyTestsBase
    {
        [Fact]
        public void Should_update_email_attributes_when_sending_an_email_with_a_custom_tracking_token()
        {
            _context.EnableProxyTypes(Assembly.GetExecutingAssembly());

            var email = new Email()
            {
                Id = Guid.NewGuid(),
                Subject = "FXE Test"
            };
            var emailId = _service.Create(email);

            var request = new SendEmailRequest
            {
                EmailId = emailId,
                TrackingToken = "CustomTrackingToken"
            };
            var response = (SendEmailResponse)_service.Execute(request) as SendEmailResponse;

            var expectedSubject = $"{email.Subject} {request.TrackingToken}";
            Assert.Equal(expectedSubject, response.Subject);

            var emailAfter = _context.CreateQuery<Email>().FirstOrDefault();
            Assert.Equal(email_statecode.Completed, emailAfter.StateCode);
            Assert.Equal(3, (int) emailAfter.StatusCode.Value);
            Assert.Equal(0, emailAfter.DeliveryAttempts);
            Assert.Equal(DateTime.UtcNow.Date, emailAfter.ActualEnd?.Date);
            Assert.Equal(expectedSubject, emailAfter.Subject);
        }
        
        [Fact]
        public void Should_update_email_attributes_when_sending_an_email_and_generate_a_new_tracking_token()
        {
            _context.EnableProxyTypes(Assembly.GetExecutingAssembly());

            var email = new Email()
            {
                Id = Guid.NewGuid(),
                Subject = "FXE Test"
            };
            var emailId = _service.Create(email);

            var request = new SendEmailRequest
            {
                EmailId = emailId
            };
            var response = (SendEmailResponse)_service.Execute(request) as SendEmailResponse;

            var expectedSubject = $"{email.Subject} CRM:0235001";
            Assert.Equal(expectedSubject, response.Subject);

            var emailAfter = _context.CreateQuery<Email>().FirstOrDefault();
            Assert.Equal(email_statecode.Completed, emailAfter.StateCode);
            Assert.Equal(3, (int) emailAfter.StatusCode.Value);
            Assert.Equal(0, emailAfter.DeliveryAttempts);
            Assert.Equal(DateTime.UtcNow.Date, emailAfter.ActualEnd?.Date);
            Assert.Equal(expectedSubject, emailAfter.Subject);
        }

        [Fact]
        public void Should_throw_exception_when_emailid_does_not_exist()
        {
            _context.EnableProxyTypes(Assembly.GetExecutingAssembly());

            var request = new SendEmailRequest
            {
                EmailId = Guid.NewGuid()
            };
            var ex = XAssert.ThrowsFaultCode(ErrorCodes.ObjectDoesNotExist, () =>  _service.Execute(request));
            Assert.Contains($"Entity 'email' With Id {request.EmailId} Does Not Exist", ex.Message);
        }
    }
}
