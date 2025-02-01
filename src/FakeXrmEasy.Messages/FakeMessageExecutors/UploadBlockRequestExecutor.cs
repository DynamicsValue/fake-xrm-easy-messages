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
    /// Implements the UploadBlockRequest message https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest?view=dataverse-sdk-latest
    /// </summary>
    public class UploadBlockRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true if the request to execute is an UploadBlockRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is UploadBlockRequest;
        }

        /// <summary>
        /// Executes the current request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var uploadBlockRequest = request as UploadBlockRequest;
            var fileDb = (ctx as XrmFakedContext).FileDb;

            var fileUploadSession = fileDb.GetFileUploadSession(uploadBlockRequest.FileContinuationToken);
            fileUploadSession.AddFileBlock(new UploadBlockProperties()
            {
                FileContinuationToken = uploadBlockRequest.FileContinuationToken,
                BlockContents = uploadBlockRequest.BlockData,
                BlockId = uploadBlockRequest.BlockId
            });

            return new UploadBlockResponse();
        }

        /// <summary>
        /// Returns the type of UploadBlockRequest
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Type GetResponsibleRequestType()
        {
            return typeof(UploadBlockRequest);
        }
    }
}
#endif