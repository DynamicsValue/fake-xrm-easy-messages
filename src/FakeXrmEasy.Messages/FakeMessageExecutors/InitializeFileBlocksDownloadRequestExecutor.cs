using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.FileStorage.Download;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements the InitializeFileBlocksDownloadRequest message https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.initializefileblocksdownloadrequest?view=dataverse-sdk-latest
    /// </summary>
    public class InitializeFileBlocksDownloadRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true if InitializeFileBlocksDownloadRequest can be executed
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is InitializeFileBlocksDownloadRequest;
        }

        /// <summary>
        /// Executes a fake implementation of InitializeFileBlocksDownloadRequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as InitializeFileBlocksDownloadRequest;
            
            var fileDb = (ctx as XrmFakedContext).FileDb;

            var fileContinuationToken = fileDb.InitFileDownloadSession(new FileDownloadProperties()
            {
                Target = req.Target,
                FileAttributeName = req.FileAttributeName
            });

            var fileDownloadSession = fileDb.GetFileDownloadSession(fileContinuationToken);
            
            return new InitializeFileBlocksDownloadResponse()
            {
                Results = new ParameterCollection()
                {
                    { "FileContinuationToken", fileContinuationToken },
                    { "FileSizeInBytes",  (long) fileDownloadSession.File.Content.Length },
                    { "FileName", fileDownloadSession.File.FileName },
                    { "IsChunkingSupported", true },
                }
            };
        }

        /// <summary>
        /// Returns the type of InitializeFileBlocksDownloadRequest
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Type GetResponsibleRequestType()
        {
            return typeof(InitializeFileBlocksDownloadRequest);
        }
    }
}