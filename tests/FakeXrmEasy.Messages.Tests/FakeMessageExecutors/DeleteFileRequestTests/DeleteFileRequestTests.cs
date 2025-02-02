using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy.Core.FileStorage.Db;
using FakeXrmEasy.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Xunit;

#if FAKE_XRM_EASY_9
namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.DeleteFileRequestTests
{
    public class DeleteFileRequestTests: FakeXrmEasyTestsBase
    {
        private readonly InMemoryFileDb _fileDb;
        private readonly Entity _entity;
        private readonly DeleteFileRequest _request;
        
        private readonly FileAttachment _file;
        private readonly EntityMetadata _entityMetadata;

        private const string FILE_ENTITY_NAME = "dv_test";
        private const string FILE_ATTRIBUTE_NAME = "dv_file";

        public DeleteFileRequestTests()
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
            
            _request = new DeleteFileRequest()
            {
                FileId = new Guid(_file.Id)
            };
        }
        
        [Fact]
        public void Should_delete_existing_file()
        {
            _context.InitializeMetadata(_entityMetadata);
            _context.Initialize(_entity);
            _context.InitializeFiles(new [] { _file });

            var response = _service.Execute(_request);
            
            Assert.NotNull(response);
            Assert.IsType<DeleteFileResponse>(response);

            var deletedFile = _fileDb.CreateQuery()
                .FirstOrDefault(f => _request.FileId != new Guid(f.Id));
            
            Assert.Null(deletedFile);
        }
    }
}
#endif