#if FAKE_XRM_EASY_9
using System;
using System.Collections.Generic;
using FakeXrmEasy.Core.FileStorage.Db;
using FakeXrmEasy.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.DownloadBlockRequestTests
{
    public class DownloadBlockRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        private readonly DownloadBlockRequest _request;
        
        private readonly FileAttachment _file;
        private readonly EntityMetadata _entityMetadata;

        private const string FILE_ENTITY_NAME = "dv_test";
        private const string FILE_ATTRIBUTE_NAME = "dv_file";

        public DownloadBlockRequestTests()
        {
            _fileDb = (_context as XrmFakedContext).FileDb;
            
            _entity = new Entity(FILE_ENTITY_NAME) { Id = Guid.NewGuid() };
                
            _file = new FileAttachment()
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = "application/pdf",
                FileName = "TestFile.pdf",
                Target = _entity.ToEntityReference(),
                AttributeName = FILE_ATTRIBUTE_NAME,
                Content = new byte[] { 1, 2, 3, 4 }
            };
            
            _request = new DownloadBlockRequest()
            {
                Offset = 0,
                BlockLength = _file.Content.Length
            };
            _entity[FILE_ATTRIBUTE_NAME] = _file.Id;
            
            var fileAttributeMetadata = new FileAttributeMetadata()
            {
                LogicalName = FILE_ATTRIBUTE_NAME
            };
            fileAttributeMetadata.MaxSizeInKB = 1; //1 KB
            
            _entityMetadata = new EntityMetadata()
            {
                LogicalName = FILE_ENTITY_NAME
            };
            _entityMetadata.SetAttributeCollection(new List<AttributeMetadata>()
            {
                fileAttributeMetadata  
            });
        }

        [Fact]
        public void Should_download_file_block_when_executing_a_download_block_request()
        {
            _context.InitializeMetadata(_entityMetadata);
            _context.Initialize(_entity);
            _context.InitializeFiles(new [] { _file });

            var initFileDownloadRequest = new InitializeFileBlocksDownloadRequest()
            {
                Target = _entity.ToEntityReference(),
                FileAttributeName = FILE_ATTRIBUTE_NAME
            };

            var initFileDownloadResponse = _service.Execute(initFileDownloadRequest) as InitializeFileBlocksDownloadResponse;
            _request.FileContinuationToken = initFileDownloadResponse.FileContinuationToken;
            
            var response = _service.Execute(_request);
            
            Assert.NotNull(response);
            Assert.IsType<DownloadBlockResponse>(response);

            var downloadBlockResponse = response as DownloadBlockResponse;
            Assert.Equal(_file.Content, downloadBlockResponse.Data);
        }
    }
}
#endif
