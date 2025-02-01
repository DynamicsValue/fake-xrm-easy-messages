#if FAKE_XRM_EASY_9
using System;
using FakeItEasy;
using FakeXrmEasy.Core.Db;
using FakeXrmEasy.Core.FileStorage.Db;
using FakeXrmEasy.Core.FileStorage.Upload;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.InitializeFileBlocksUploadRequestTests
{
    public class InitializeFileBlocksUploadRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        private readonly InitializeFileBlocksUploadRequest _request;
        
        public InitializeFileBlocksUploadRequestTests(): base()
        {
            _fileDb = (_context as XrmFakedContext).FileDb;
            
            _entity = new Entity("dv_test") { Id = Guid.NewGuid() };
                
            _request = new InitializeFileBlocksUploadRequest()
            {
                FileName = "Test.pdf",
                Target = _entity.ToEntityReference(),
                FileAttributeName = "dv_file"
            };
        }

        [Fact]
        public void Should_call_file_upload_session_with_correct_parameters()
        {
            _context.Initialize(_entity);
            
            var response = _service.Execute(_request) as InitializeFileBlocksUploadResponse;
            Assert.NotNull(response);
            Assert.NotNull(response.FileContinuationToken);

            var fileSession = _fileDb.GetFileUploadSession(response.FileContinuationToken);
            Assert.NotNull(fileSession);
            
            Assert.Equal(_request.FileName, fileSession.Properties.FileName);
            Assert.Equal(_request.Target.LogicalName, fileSession.Properties.Target.LogicalName);
            Assert.Equal(_request.Target.Id, fileSession.Properties.Target.Id);
            Assert.Equal(_request.FileAttributeName, fileSession.Properties.FileAttributeName);

        }
    }
}
#endif