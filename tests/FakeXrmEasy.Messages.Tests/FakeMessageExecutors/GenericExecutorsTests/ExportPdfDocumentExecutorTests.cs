using System;
using System.Collections.Generic;
using System.ServiceModel;
using Crm;
using FakeXrmEasy.FakeMessageExecutors.GenericExecutors;
using FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.GenericExecutors.NavigateToNextEntityRequestTests
{
    public class ExportPdfDocumentExecutorTests: FakeXrmEasyTestsBase
    {
        private readonly ExportPdfDocumentExecutor _executor;
        private const string _exportPdfDocumentRequestName = "ExportPdfDocument";
        private readonly OrganizationRequest _exportPdfDocumentRequest;
        private readonly Quote _quote;
        private readonly Entity _documentTemplate;
        private const string _documentTemplateLogicalName = "documenttemplate";
            
        public ExportPdfDocumentExecutorTests()
        {
            _quote = new Quote() { Id = Guid.NewGuid() };
            _documentTemplate = new Entity(_documentTemplateLogicalName)
            {
                Id = Guid.NewGuid()
            };
            
            _executor = new ExportPdfDocumentExecutor();
            _exportPdfDocumentRequest = new OrganizationRequest(_exportPdfDocumentRequestName);
            _exportPdfDocumentRequest["EntityTypeCode"] = Quote.EntityTypeCode;
            _exportPdfDocumentRequest["SelectedTemplate"] = _documentTemplate.ToEntityReference();
            _exportPdfDocumentRequest["SelectedRecords"] = "[\"{" + _quote.Id + "}\"]";
            
        }
        
        [Fact]
        public void Should_return_correct_request_name()
        {
            Assert.Equal(_exportPdfDocumentRequestName, _executor.GetRequestName());
        }
        
        [Fact]
        public void Should_execute_a_generic_request_that_matches_the_request_name()
        {
            Assert.True( _executor.CanExecute(_exportPdfDocumentRequest));
        }
        
        [Fact]
        public void Should_return_a_generic_responsible_type()
        {
            Assert.Equal(typeof(OrganizationRequest),_executor.GetResponsibleRequestType());
        }

        [Fact]
        public void Should_return_error_if_object_type_code_was_not_specified()
        {
            _exportPdfDocumentRequest.Parameters.Remove("EntityTypeCode");
            Assert.Throws<EntityTypeCodeNotSpecifiedException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Fact]
        public void Should_return_error_if_selected_template_was_not_specified()
        {
            _exportPdfDocumentRequest.Parameters.Remove("SelectedTemplate");
            Assert.Throws<SelectedTemplateNotSpecifiedException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Fact]
        public void Should_return_error_if_selected_records_was_not_specified()
        {
            _exportPdfDocumentRequest.Parameters.Remove("SelectedRecords");
            Assert.Throws<SelectedRecordsNotSpecifiedException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Fact]
        public void Should_return_error_if_document_template_is_not_an_entity_reference()
        {
            _exportPdfDocumentRequest["SelectedTemplate"] = _documentTemplate;
            Assert.Throws<SelectedTemplateMustBeEntityReferenceException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Fact]
        public void Should_return_error_if_document_template_was_not_found()
        {
            Assert.Throws<SelectedTemplateNotFoundException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Fact]
        public void Should_return_error_if_selected_records_is_not_valid()
        {
            _context.Initialize(_documentTemplate);
            _exportPdfDocumentRequest["SelectedRecords"] = "asdasdasdads";
            Assert.Throws<SelectedRecordsIsNotValidException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("asdasdaa")]
        public void Should_return_error_if_entity_type_code_is_not_valid(object value)
        {
            _context.Initialize(_documentTemplate);
            _exportPdfDocumentRequest["EntityTypeCode"] = value;
            Assert.Throws<EntityTypeCodeNotValidException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        
        [Fact]
        public void Should_return_error_if_selected_records_were_not_found()
        {
            _context.Initialize(new List<Entity>()
            {
                _documentTemplate, _quote 
            });
            
            _exportPdfDocumentRequest["SelectedRecords"] = "[\"{" + Guid.NewGuid() + "}\"]";
            Assert.Throws<SelectedRecordsNotFoundException>(() => _executor.Execute(_exportPdfDocumentRequest, _context));
        }
        
        
        [Fact]
        public void Should_return_dummy_byte_array()
        {
            _context.Initialize(new List<Entity>()
            {
                _documentTemplate, _quote 
            });

            var response = _executor.Execute(_exportPdfDocumentRequest, _context);
            Assert.NotNull(response);
            Assert.Equal("ExportPdfDocumentResponse", response.ResponseName);
            Assert.True(response.Results.ContainsKey("PdfFile"));
        }
    }
}