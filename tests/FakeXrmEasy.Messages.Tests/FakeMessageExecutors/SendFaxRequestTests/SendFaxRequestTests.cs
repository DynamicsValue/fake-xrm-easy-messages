using System;
using System.Reflection;
using DataverseEntities;
using Microsoft.Crm.Sdk.Messages;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.SendFaxRequestTests
{
    public class SendFaxRequestTests: FakeXrmEasyTestsBase
    {
        private SendFaxRequest _request;
        private readonly Fax _fax;
        private const string DUMMY_FAX_NO = "666666666";
        private const string DUMMY_EMAIL = "666666666@dummy-efax.com";
        
        public SendFaxRequestTests()
        {
            _fax = new Fax()
            {
                Id = Guid.NewGuid(),
                FaxNumber = DUMMY_FAX_NO
            };
            
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Fax)));
        }

        [Fact]
        public void Should_throw_unsupported_exception_when_trying_to_send_a_valid_fax_entity_record()
        {
            _context.Initialize(_fax);
            
            _request = new SendFaxRequest()
            {
                FaxId = _fax.Id
            };

            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.NotSupported, () => _service.Execute(_request));
            Assert.Contains($"Sending faxes is not supported", ex.Message);
        }
        
        [Fact]
        public void Should_throw_unsupported_exception_when_trying_to_send_a_non_existing_valid_fax_entity_record()
        {
            _request = new SendFaxRequest()
            {
                FaxId = Guid.NewGuid()
            };

            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.NotSupported, () => _service.Execute(_request));
            Assert.Contains($"Sending faxes is not supported", ex.Message);
        }
    }
}