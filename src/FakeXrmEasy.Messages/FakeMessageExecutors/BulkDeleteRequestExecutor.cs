using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class BulkDeleteRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is BulkDeleteRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>BulkDeleteResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var bulkDeleteRequest = (BulkDeleteRequest)request;
           
            if (string.IsNullOrEmpty(bulkDeleteRequest.JobName))
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not Bulk delete without JobName");
            }
            if (bulkDeleteRequest.QuerySet == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not Bulk delete without QuerySet");
            }
            if (bulkDeleteRequest.CCRecipients == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not Bulk delete without CCRecipients");
            }
            if (bulkDeleteRequest.ToRecipients == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not Bulk delete without ToRecipients");
            }

            var service = ctx.GetOrganizationService();

            // generate JobId
            var jobId = Guid.NewGuid();

            // create related asyncOperation
            Entity asyncOpertation = new Entity("asyncoperation")
            {
                Id = jobId
            };

            service.Create(asyncOpertation);

            // delete all records from all queries
            foreach (QueryExpression queryExpression in bulkDeleteRequest.QuerySet)
            {
                EntityCollection recordsToDelete = service.RetrieveMultiple(queryExpression);
                foreach (Entity record in recordsToDelete.Entities)
                {
                    service.Delete(record.LogicalName, record.Id);
                }
            }

            // set ayncoperation to completed
            asyncOpertation["statecode"] = new OptionSetValue(3);
            service.Update(asyncOpertation);

            // return result
            return new BulkDeleteResponse { ResponseName = "BulkDeleteResponse", ["JobId"] = jobId};
        }
        
        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(BulkDeleteRequest);
        }
    }
}
