using System;
using FakeXrmEasy.Core.FileStorage.Db;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.CommitFileBlocksUploadRequestTests
{
    public class CommitFileBlocksUploadRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        
        private readonly InitializeFileBlocksUploadRequest _initFileUploadRequest;
        private readonly CommitFileBlocksUploadRequest _commitFileBlocksUploadRequest;
        private readonly UploadBlockRequest _uploadBlockRequest;
        
        public CommitFileBlocksUploadRequestTests()
        {
            _fileDb = (_context as XrmFakedContext).FileDb;
            
            _entity = new Entity("dv_test") { Id = Guid.NewGuid() };
                
            _initFileUploadRequest = new InitializeFileBlocksUploadRequest()
            {
                FileName = "Test.pdf",
                Target = _entity.ToEntityReference(),
                FileAttributeName = "dv_file"
            };
            
            _uploadBlockRequest = new UploadBlockRequest()
            {
                BlockData = new byte[] { 1, 2, 3, 4 },
                BlockId = new Guid().ToString(),
            };

            _commitFileBlocksUploadRequest = new CommitFileBlocksUploadRequest()
            {
                FileName = "Test.pdf",
                BlockList = new[] { _uploadBlockRequest.BlockId },
                MimeType = "application/pdf"
            };
        }

        [Fact]
        public void Should_commit_file_block_upload()
        {
            _context.Initialize(_entity);
            
            var initFileUploadResponse = _service.Execute(_initFileUploadRequest) as InitializeFileBlocksUploadResponse;
            
            _uploadBlockRequest.FileContinuationToken = initFileUploadResponse.FileContinuationToken;
            _service.Execute(_uploadBlockRequest);

            _commitFileBlocksUploadRequest.FileContinuationToken = initFileUploadResponse.FileContinuationToken;
            var response = _service.Execute(_commitFileBlocksUploadRequest);
            Assert.NotNull(response);
            Assert.IsType<CommitFileBlocksUploadResponse>(response);

            var commitResponse = response as CommitFileBlocksUploadResponse;
            Assert.Equal(4L, commitResponse.FileSizeInBytes);
            Assert.NotEqual(Guid.Empty, commitResponse.FileId);

            var file = _fileDb.GetFileById(commitResponse.FileId.ToString());
            Assert.NotNull(file);
            
            Assert.Equal(_commitFileBlocksUploadRequest.MimeType, file.MimeType);
            Assert.Equal(_uploadBlockRequest.BlockData, file.Content);
        }
    }
}