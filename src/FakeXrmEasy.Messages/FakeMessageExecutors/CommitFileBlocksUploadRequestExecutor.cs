#if FAKE_XRM_EASY_9
using System;
using System.Linq;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Core.FileStorage.Upload;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements the CommitFileBlocksUploadRequest https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.commitfileblocksuploadrequest?view=dataverse-sdk-latest
    /// </summary>
    public class CommitFileBlocksUploadRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true when the request is a CommitFileBlocksUploadRequest
        /// </summary>
        /// <param name="request">The request to check if it can be executed by this executor</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is CommitFileBlocksUploadRequest;
        }

        /// <summary>
        /// Executes a CommitFileBlocksUploadRequest
        /// </summary>
        /// <param name="request">The request to execute</param>
        /// <param name="ctx">The IXrmFakedContext that this request will be executed against</param>
        /// <returns>CommitFileBlocksUploadResponse</returns>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as CommitFileBlocksUploadRequest;
            
            var fileDb = (ctx as XrmFakedContext).FileDb;

            var fileId = fileDb.CommitFileUploadSession(new CommitFileUploadSessionProperties()
            {
                FileName = req.FileName,
                MimeType = req.MimeType,
                BlockIdsListSequence = req.BlockList,
                FileUploadSessionId = req.FileContinuationToken
            });

            var file = fileDb.GetFileById(fileId);
            
            return new CommitFileBlocksUploadResponse()
            {
                Results = new ParameterCollection()
                {
                    { "FileId" , new Guid(fileId) },
                    { "FileSizeInBytes" , (long) file.Content.Length }
                }
            };
        }

        /// <summary>
        /// The type of CommitFileBlocksUploadRequest
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(CommitFileBlocksUploadRequest);
        }
    }
}
#endif