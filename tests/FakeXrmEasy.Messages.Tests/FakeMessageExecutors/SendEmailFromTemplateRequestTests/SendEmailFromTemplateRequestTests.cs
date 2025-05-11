
using System;
using System.Collections.Generic;
using System.Linq;
using DataverseEntities;
using FakeXrmEasy.FakeMessageExecutors;
using FakeXrmEasy.Messages.Exceptions.InstantiateTemplateRequest;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.SendEmailFromTemplateRequestTests
{
    public class SendEmailFromTemplateRequestTests: FakeXrmEasyTestsBase
    {
        private const string DUMMY_EMAIL = "fake.email@gmail.com";
        
        private const string EMAIL_TEMPLATE_SUBJECT_XSLT =
            "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\" /><xsl:template match=\"/data\"><![CDATA[Thank you for registering with us]]></xsl:template></xsl:stylesheet>";
        private const string EMAIL_TEMPLATE_BODY_XSLT = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[<P>Dear ]]><xsl:choose><xsl:when test=\"contact/salutation\"><xsl:value-of select=\"contact/salutation\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ ]]><xsl:choose><xsl:when test=\"contact/lastname\"><xsl:value-of select=\"contact/lastname\" /></xsl:when><xsl:otherwise>Valued Customer</xsl:otherwise></xsl:choose><![CDATA[  ,</P>\r\n     <P>Thank you for registering with us. We now have the following registration information on file:</P><P>Name: ]]><xsl:choose><xsl:when test=\"systemuser/fullname\"><xsl:value-of select=\"systemuser/fullname\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Street Address: ]]><xsl:choose><xsl:when test=\"contact/address1_line1\"><xsl:value-of select=\"contact/address1_line1\" /></xsl:when><xsl:when test=\"contact/address1_line2\"><xsl:value-of select=\"contact/address1_line2\" /></xsl:when><xsl:when test=\"contact/address1_line3\"><xsl:value-of select=\"contact/address1_line3\" /></xsl:when><xsl:otherwise>No Address Provided</xsl:otherwise></xsl:choose><![CDATA[ <BR>City: ]]><xsl:choose><xsl:when test=\"contact/address1_city\"><xsl:value-of select=\"contact/address1_city\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>State or Province: ]]><xsl:choose><xsl:when test=\"contact/address1_stateorprovince\"><xsl:value-of select=\"contact/address1_stateorprovince\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Country or Region: ]]><xsl:choose><xsl:when test=\"contact/address1_country\"><xsl:value-of select=\"contact/address1_country\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Postal Code: ]]><xsl:choose><xsl:when test=\"contact/address1_postalcode\"><xsl:value-of select=\"contact/address1_postalcode\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>E-mail Address: ]]><xsl:choose><xsl:when test=\"contact/emailaddress1\"><xsl:value-of select=\"contact/emailaddress1\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</P><P>If you would like to change or add additional information to your customer profile please visit our Web site. While there you can take advantage of the many self-service features of our site including scheduling, appointment notifications, knowledge base look-up, and service request management. </P><P>We look forward to serving you in the future.</P>\r\n     <P>Thank you.</P>]]></xsl:template></xsl:stylesheet>";
        private const string EMAIL_TEMPLATE_TITLE = "Thank you for registering with us";

        private const string EMAIL_RENDERED_BODY = @"<P>Dear Mr.Montana  ,</P>
     <P>Thank you for registering with us. We now have the following registration information on file:</P><P>Name: <BR>Street Address: No Address Provided <BR>City: <BR>State or Province: <BR>Country or Region: <BR>Postal Code: <BR>E-mail Address: fake.email@gmail.com</P><P>If you would like to change or add additional information to your customer profile please visit our Web site. While there you can take advantage of the many self-service features of our site including scheduling, appointment notifications, knowledge base look-up, and service request management. </P><P>We look forward to serving you in the future.</P>
     <P>Thank you.</P>";
        
        private SendEmailFromTemplateRequest _request;
        private readonly Contact _contact;
        private readonly Template _template;
        private readonly SystemUser _systemUser;
        private readonly Email _target;
        
        public SendEmailFromTemplateRequestTests()
        {
            _template = new Template()
            {
                Id = Guid.NewGuid(),
                TemplateTypeCode = Contact.EntityLogicalName,
                Subject = EMAIL_TEMPLATE_SUBJECT_XSLT,
                Body = EMAIL_TEMPLATE_BODY_XSLT,
                Title = EMAIL_TEMPLATE_TITLE
            };
            
            _contact = new Contact()
            {
                Id = Guid.NewGuid(),
                Salutation = "Mr.",
                LastName = "Montana",
                EMailAddress1 = DUMMY_EMAIL
            };

            _systemUser = new SystemUser()
            {
                Id = Guid.NewGuid(),
                LastName = "User LastName"
            };

            _target = new Email()
            {
                From = new [] {new ActivityParty() { PartyId = _systemUser.ToEntityReference() }},
                To = new [] {new ActivityParty() { PartyId = _contact.ToEntityReference() }},
                DirectionCode = true
            };
        }
        
        [Fact]
        public void Should_throw_exception_when_template_id_does_not_exist()
        {
            _context.Initialize(new List<Entity>()
            {
                _contact, _systemUser
            });

            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = Guid.NewGuid(),
                RegardingId = _contact.Id,
                RegardingType = Contact.EntityLogicalName
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.ObjectDoesNotExist, () => _service.Execute(_request));
            Assert.Contains($"Entity 'template' With Id = {_request.TemplateId} Does Not Exist", ex.Message);
        }
        
        [Fact]
        public void Should_throw_exception_when_target_is_null()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _systemUser
            });
            
            _request = new SendEmailFromTemplateRequest()
            {
                Target = null,
                TemplateId = _template.Id, //is a contact template
                RegardingId = _contact.Id,
                RegardingType = Account.EntityLogicalName
            };
            
            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Required field 'Target' is missing for RequestName='SendEmailFromTemplate'", ex.Message);
        }
        
        [Fact]
        public void Should_throw_exception_when_regarding_type_does_not_match()
        {
            var account = new Account()
            {
                Id = Guid.NewGuid()
            };
            
            _context.Initialize(new List<Entity>()
            {
                _template, account, _systemUser
            });
            
            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = _template.Id, //is a contact template
                RegardingId = account.Id,
                RegardingType = Account.EntityLogicalName
            };
            
            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Template type is incorrect for given objectType and the current template's templatetypecode", ex.Message);
        }
        
        [Fact]
        public void Should_throw_exception_when_regarding_id_does_not_exist()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _systemUser
            });
            
            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = _template.Id,
                RegardingId = _contact.Id,
                RegardingType = Contact.EntityLogicalName
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.ObjectDoesNotExist, () => _service.Execute(_request));
            Assert.Contains($"Entity 'contact' With Id = {_request.RegardingId} Does Not Exist", ex.Message);
        }

        [Fact]
        public void Should_send_email_from_template_with_relevant_properties()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _systemUser
            });

            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = _template.Id,
                RegardingId = _contact.Id,
                RegardingType = Contact.EntityLogicalName
            };

            var response = _service.Execute(_request);
            Assert.IsType<SendEmailFromTemplateResponse>(response);

            var emails = _context.CreateQuery<Email>().ToList();
            Assert.Single(emails); //the email is actually created now
            var createdEmail = emails[0];
            
            var sendEmailFromTemplateResponse = (response as SendEmailFromTemplateResponse);
            Assert.Equal(emails[0].Id, sendEmailFromTemplateResponse.Id);
            
            Assert.NotNull(createdEmail["subject"]);
            Assert.NotNull(createdEmail["description"]);

            var regarding = createdEmail["regardingobjectid"] as EntityReference;
            Assert.Equal(_contact.Id, regarding.Id);
            Assert.Equal(Contact.EntityLogicalName, regarding.LogicalName);

            Assert.Equal(_contact.Id, createdEmail.To.First().PartyId.Id);
            Assert.Equal(_systemUser.Id, createdEmail.From.First().PartyId.Id);
            Assert.Equal("Thank you for registering with us CRM:0235001", createdEmail["subject"]);
            Assert.StartsWith(InstantiateTemplateRequestExecutor.HTML_BODY_PREFIX, (string) createdEmail["description"]);
            Assert.Contains(EMAIL_RENDERED_BODY, (string)createdEmail["description"]);
            Assert.EndsWith(InstantiateTemplateRequestExecutor.HTML_BODY_SUFFIX, (string) createdEmail["description"]);
        }
        
        [Fact]
        public void Should_return_invalid_xslt_exception_if_subject_is_not_valid()
        {
            _template.Subject = "asdasdasd";
            
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _systemUser
            });

            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = _template.Id,
                RegardingId = _contact.Id,
                RegardingType = Contact.EntityLogicalName
            };

            Assert.Throws<InvalidXsltAttributeValueException>(() => _service.Execute(_request));
        }
        
        [Fact]
        public void Should_return_invalid_xslt_exception_if_body_is_not_valid()
        {
            _template.Body = "asdasdasdsad";
            
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _systemUser
            });

            _request = new SendEmailFromTemplateRequest()
            {
                Target = _target,
                TemplateId = _template.Id,
                RegardingId = _contact.Id,
                RegardingType = Contact.EntityLogicalName
            };

            Assert.Throws<InvalidXsltAttributeValueException>(() => _service.Execute(_request));
        }
    }
}
