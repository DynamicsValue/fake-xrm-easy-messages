#if FAKE_XRM_EASY_9
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.FileStorage.Download;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements the DownloadBlockRequest message https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest?view=dataverse-sdk-latest
    /// </summary>
    public class DownloadBlockRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true if the request is a DownloadBlockRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is DownloadBlockRequest;
        }

        /// <summary>
        /// Executes a fake implementation of DownloadBlockRequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as DownloadBlockRequest;
            
            var fileDb = (ctx as XrmFakedContext).FileDb;

            var data = fileDb.DownloadFileBlock(new DownloadBlockProperties()
            {
                FileDownloadSessionId = req.FileContinuationToken,
                Offset = req.Offset,
                BlockLength = req.BlockLength
            });
            
            return new DownloadBlockResponse()
            {
                Results = new ParameterCollection()
                {
                    { "Data", data }
                }
            };
        }

        /// <summary>
        /// Returns the DownloadBlockRequest type
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(DownloadBlockRequest);
        }
    }
}
#endif