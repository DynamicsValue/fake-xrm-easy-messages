#if FAKE_XRM_EASY_9
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.FileStorage.Upload;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class InitializeFileBlocksUploadRequestExecutor: IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InitializeFileBlocksUploadRequest;
        }

        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            InitializeFileBlocksUploadRequest req = (InitializeFileBlocksUploadRequest)request;

            var fileDb = (ctx as XrmFakedContext).FileDb;

            var fileUploadSession = fileDb.InitFileUploadSession(new FileUploadProperties()
            {
                Target = req.Target,
                FileName = req.FileName,
                FileAttributeName = req.FileAttributeName
            });
            
            return new InitializeFileBlocksUploadResponse()
            {
                Results = new ParameterCollection()
                {
                    { "FileContinuationToken" , fileUploadSession }
                }
            };
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(InitializeFileBlocksUploadRequest);
        }
    }
}
#endif