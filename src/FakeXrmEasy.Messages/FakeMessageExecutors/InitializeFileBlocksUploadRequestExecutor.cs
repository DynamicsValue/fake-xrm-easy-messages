#if FAKE_XRM_EASY_9
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.FileStorage.Upload;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements the InitializeFileBlocksUploadRequest message: https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.initializefileblocksuploadrequest?view=dataverse-sdk-latest
    /// </summary>
    public class InitializeFileBlocksUploadRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true when asked if it can execute an InitializeFileBlocksUploadRequest message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InitializeFileBlocksUploadRequest;
        }

        /// <summary>
        /// Executes a fake implementation of the request using an In-Memory File Storage mechanism
        /// </summary>
        /// <param name="request">The request to execute</param>
        /// <param name="ctx">The context to store the files against</param>
        /// <returns>InitializeFileBlocksUploadResponse</returns>
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

        /// <summary>
        /// Returns the type of InitializeFileBlocksUploadResponse
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(InitializeFileBlocksUploadResponse);
        }
    }
}
#endif