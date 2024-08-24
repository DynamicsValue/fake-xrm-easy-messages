#if FAKE_XRM_EASY_9
using System;
using System.Linq;
using FakeXrmEasy.Core.FileStorage.Db;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.UploadBlockRequestTests
{
    public class UploadBlockRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        private readonly InitializeFileBlocksUploadRequest _request;
        
        public UploadBlockRequestTests()
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
        public void Should_call_upload_blob_request_with_correct_parameters()
        {
            _context.Initialize(_entity);
            
            var initFileUploadResponse = _service.Execute(_request) as InitializeFileBlocksUploadResponse;

            var request = new UploadBlockRequest()
            {
                FileContinuationToken = initFileUploadResponse.FileContinuationToken,
                BlockData = new byte[] { 1, 2, 3, 4 },
                BlockId = new Guid().ToString(),
            };

            var response = _service.Execute(request);
            Assert.NotNull(response);
            Assert.IsType<UploadBlockResponse>(response);
            
            //Check the blob was added
            var fileUploadSession = _fileDb.GetFileUploadSession(initFileUploadResponse.FileContinuationToken);
            var fileBlocks = fileUploadSession.GetAllBlocks();

            Assert.Single(fileBlocks);

            var uploadedFileBlock = fileBlocks.FirstOrDefault();
            Assert.Equal(request.BlockData, uploadedFileBlock.Content);
            Assert.Equal(request.BlockId, uploadedFileBlock.BlockId);
        }
    }
}
#endif