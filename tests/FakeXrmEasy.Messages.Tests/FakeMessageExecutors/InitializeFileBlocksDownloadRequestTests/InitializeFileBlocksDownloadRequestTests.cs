#if FAKE_XRM_EASY_9
using System;
using System.Collections.Generic;
using FakeXrmEasy.Core.FileStorage.Db;
using FakeXrmEasy.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.InitializeFileBlocksDownloadRequestTests
{
    public class InitializeFileBlocksDownloadRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        private readonly InitializeFileBlocksDownloadRequest _request;
        
        private readonly FileAttachment _file;
        private readonly EntityMetadata _entityMetadata;

        private const string FILE_ENTITY_NAME = "dv_test";
        private const string FILE_ATTRIBUTE_NAME = "dv_file";
        
        public InitializeFileBlocksDownloadRequestTests()
        {
            _fileDb = (_context as XrmFakedContext).FileDb;
            
            _entity = new Entity(FILE_ENTITY_NAME) { Id = Guid.NewGuid() };
                
            _request = new InitializeFileBlocksDownloadRequest()
            {
                Target = _entity.ToEntityReference(),
                FileAttributeName = FILE_ATTRIBUTE_NAME
            };
            
            _file = new FileAttachment()
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = "application/pdf",
                FileName = "TestFile.pdf",
                Target = _entity.ToEntityReference(),
                AttributeName = FILE_ATTRIBUTE_NAME,
                Content = new byte[] { 1, 2, 3, 4 }
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
        public void Should_initiate_file_download_request_with_correct_parameters()
        {
            _context.InitializeMetadata(_entityMetadata);
            _context.Initialize(_entity);
            _context.InitializeFiles(new [] { _file });

            var response = _service.Execute(_request);
            
            Assert.NotNull(response);
            Assert.IsType<InitializeFileBlocksDownloadResponse>(response);

            var downloadResponse = response as InitializeFileBlocksDownloadResponse;
            Assert.Equal(_file.FileName, downloadResponse.FileName);
            Assert.Equal((long) _file.Content.Length, downloadResponse.FileSizeInBytes);
            Assert.True(downloadResponse.IsChunkingSupported);
            Assert.NotNull(downloadResponse.FileContinuationToken);

            var fileDownloadSession = _fileDb.GetFileDownloadSession(downloadResponse.FileContinuationToken);
            Assert.NotNull(fileDownloadSession);
        }
    }
}
#endif