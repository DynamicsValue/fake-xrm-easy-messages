#if FAKE_XRM_EASY_9
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors
{
    /// <summary>
    /// Implements the DeleteFileRequest message https://learn.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.deletefilerequest?view=dataverse-sdk-latest
    /// </summary>
    public class DeleteFileRequestExecutor: IFakeMessageExecutor
    {
        /// <summary>
        /// Returns true if the request is a DeleteFileRequest message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is DeleteFileRequest;
        }

        /// <summary>
        /// Implements a fake DeleteFileRequest message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var req = request as DeleteFileRequest;
            
            var fileDb = (ctx as XrmFakedContext).FileDb;
            fileDb.DeleteFile(req.FileId.ToString());

            return new DeleteFileResponse();
        }

        /// <summary>
        /// Returns the type of DeleteFileRequest
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Type GetResponsibleRequestType()
        {
            return typeof(DeleteFileRequest);
        }
    }
}
#endif