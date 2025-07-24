using System;
using System.Linq;
using DataverseEntities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.SendTemplateRequestTests
{
    public class SendTemplateRequestTests: FakeXrmEasyTestsBase
    {
        protected SendTemplateRequest _request;

        private const string ACCOUNT_NAME = "SendTemplateTest Organisation";

        private const string DUMMY_EMAIL = "jordi.montana+test@gmail.com";
        private const string DUMMY_EMAIL_2 = "jordi.montana+test2@gmail.com";

        private const string EMAIL_TEMPLATE_SUBJECT_XSLT =
            "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\" /><xsl:template match=\"/data\"><![CDATA[Thank you for registering with us]]></xsl:template></xsl:stylesheet>";
        private const string EMAIL_TEMPLATE_BODY_XSLT = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[<P>Dear ]]><xsl:choose><xsl:when test=\"contact/salutation\"><xsl:value-of select=\"contact/salutation\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ ]]><xsl:choose><xsl:when test=\"contact/lastname\"><xsl:value-of select=\"contact/lastname\" /></xsl:when><xsl:otherwise>Valued Customer</xsl:otherwise></xsl:choose><![CDATA[  ,</P>\r\n     <P>Thank you for registering with us. We now have the following registration information on file:</P><P>Name: ]]><xsl:choose><xsl:when test=\"systemuser/fullname\"><xsl:value-of select=\"systemuser/fullname\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Street Address: ]]><xsl:choose><xsl:when test=\"contact/address1_line1\"><xsl:value-of select=\"contact/address1_line1\" /></xsl:when><xsl:when test=\"contact/address1_line2\"><xsl:value-of select=\"contact/address1_line2\" /></xsl:when><xsl:when test=\"contact/address1_line3\"><xsl:value-of select=\"contact/address1_line3\" /></xsl:when><xsl:otherwise>No Address Provided</xsl:otherwise></xsl:choose><![CDATA[ <BR>City: ]]><xsl:choose><xsl:when test=\"contact/address1_city\"><xsl:value-of select=\"contact/address1_city\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>State or Province: ]]><xsl:choose><xsl:when test=\"contact/address1_stateorprovince\"><xsl:value-of select=\"contact/address1_stateorprovince\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Country or Region: ]]><xsl:choose><xsl:when test=\"contact/address1_country\"><xsl:value-of select=\"contact/address1_country\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>Postal Code: ]]><xsl:choose><xsl:when test=\"contact/address1_postalcode\"><xsl:value-of select=\"contact/address1_postalcode\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<BR>E-mail Address: ]]><xsl:choose><xsl:when test=\"contact/emailaddress1\"><xsl:value-of select=\"contact/emailaddress1\" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</P><P>If you would like to change or add additional information to your customer profile please visit our Web site. While there you can take advantage of the many self-service features of our site including scheduling, appointment notifications, knowledge base look-up, and service request management. </P><P>We look forward to serving you in the future.</P>\r\n     <P>Thank you.</P>]]></xsl:template></xsl:stylesheet>";
        private const string EMAIL_TEMPLATE_TITLE = "Thank you for registering with us";

        private const string EMAIL_RENDERED_BODY = @"<P>Dear Mr.Montana  ,</P>
     <P>Thank you for registering with us. We now have the following registration information on file:</P><P>Name: <BR>Street Address: No Address Provided <BR>City: <BR>State or Province: <BR>Country or Region: <BR>Postal Code: <BR>E-mail Address: fake.email@gmail.com</P><P>If you would like to change or add additional information to your customer profile please visit our Web site. While there you can take advantage of the many self-service features of our site including scheduling, appointment notifications, knowledge base look-up, and service request management. </P><P>We look forward to serving you in the future.</P>
     <P>Thank you.</P>";
        
        private readonly Template _template;
        private readonly Contact _contact;
        private readonly Contact _contact2;
        private readonly Account _account;
        
        public SendTemplateRequestTests()
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
                EMailAddress1 = DUMMY_EMAIL
            };

            _contact2 = new Contact()
            {
                Id = Guid.NewGuid(),
                EMailAddress1 = DUMMY_EMAIL_2
            };

            _account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = ACCOUNT_NAME
            };
            
        }
        private Contact CreateContactIfNotExists(string emailAddress)
        {
            using (var ctx = new XrmServiceContext(_service))
            {
                Contact contact = (from c in ctx.CreateQuery<Contact>()
                                    where c.EMailAddress1 == emailAddress
                                   select c).FirstOrDefault();

                if(contact != null)
                {
                    return contact;
                }

                contact = new Contact()
                {
                    EMailAddress1 = emailAddress
                };

                var id = _service.Create(contact);
                contact.Id = id;
                return contact;
            }
        }

        private Account CreateAccountIfNotExists(string accountName)
        {
            using (var ctx = new XrmServiceContext(_service))
            {
                var account = (from a in ctx.CreateQuery<Account>()
                                   where a.Name == accountName
                                   select a).FirstOrDefault();

                if (account != null)
                {
                    return account;
                }

                account = new Account()
                {
                    Name = accountName
                };

                var id = _service.Create(account);
                account.Id = id;
                return account;
            }
        }

        private EntityReference GetSender()
        {
            var systemUserResponse = (WhoAmIResponse)_service.Execute(new WhoAmIRequest());
            var userId = systemUserResponse.UserId;

            return new EntityReference(SystemUser.EntityLogicalName, userId);

        }

        /// <summary>
        /// Assumes LoggerPlugin is registered against Update message of Email
        /// </summary>
        [Fact]
        public void Should_trigger_and_send_bulk_email_to_two_contacts()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new[] { _contact.Id, _contact2.Id, }
            };

            var response = _service.Execute(_request);
            Assert.NotNull(response);
            Assert.IsType<SendTemplateResponse>(response);

            var emails = _context.CreateQuery<Email>().ToList();
            Assert.Equal(2, emails.Count);
            
        }

        [Fact]
        public void Should_fail_when_sender_is_null() 
        { 
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = null,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new[] { _contact.Id, _contact2.Id, }
            };

            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Required field 'Sender' is missing for RequestName='SendTemplate'", ex.Message);

        }


        [Fact]
        public void Should_throw_exception_when_recipient_ids_is_null()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = null
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Required field 'RecipientIds' is missing for RequestName='SendTemplate'", ex.Message);
        }

        [Fact]
        public void Should_throw_exception_when_recipient_type_is_null()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = null,
                RecipientIds = new[] { _contact.Id, _contact2.Id, }
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Required field 'RecipientType' is missing for RequestName='SendTemplate'", ex.Message);
        }
        
        [Fact]
        public void Should_throw_exception_when_recipient_ids_is_empty()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new Guid[] { }
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.InvalidArgument, () => _service.Execute(_request));
            Assert.Contains("Recipients should be set", ex.Message);
        }

        [Fact]
        public void Should_throw_exception_when_recipient_does_not_exist()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = _account.Id,
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new Guid[] { Guid.NewGuid() }
            };


            var ex = XAssert.ThrowsFaultCode(Abstractions.ErrorCodes.unManagedidscommunicationsnopartyaddress, () => _service.Execute(_request));
            Assert.Contains("Object address not found on party or party is marked as non-emailable", ex.Message);
        }
        
        [Fact]
        public void Should_not_throw_exception_when_template_id_does_not_exist_but_it_wont_send_any_emails_either()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });

            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = Guid.NewGuid(),
                RegardingId = _account.Id,
                RegardingType = Contact.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new[] { _contact.Id, _contact2.Id, }
            };

            var response = _service.Execute(_request);
            Assert.NotNull(response);
            Assert.IsType<SendTemplateResponse>(response);
            
            var emails = _context.CreateQuery<Email>().ToList();
            Assert.Empty(emails);
        }

        [Fact]
        public void Should_not_throw_exception_when_regarding_id_does_not_exist_but_wont_send_emails_either()
        {
            _context.Initialize(new List<Entity>()
            {
                _template, _contact, _contact2, _account
            });
            
            var sender = GetSender();

            _request = new SendTemplateRequest()
            {
                Sender = sender,
                TemplateId = _template.Id,
                RegardingId = Guid.NewGuid(),
                RegardingType = Account.EntityLogicalName,
                RecipientType = Contact.EntityLogicalName,
                RecipientIds = new[] { _contact.Id, _contact2.Id, }
            };


            var response = _service.Execute(_request);
            Assert.NotNull(response);
            Assert.IsType<SendTemplateResponse>(response);
            
            var emails = _context.CreateQuery<Email>().ToList();
            Assert.Empty(emails);
        }
    }
}