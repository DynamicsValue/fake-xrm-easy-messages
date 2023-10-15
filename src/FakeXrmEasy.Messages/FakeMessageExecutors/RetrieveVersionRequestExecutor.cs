using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveVersionRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveVersionRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveVersionResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            string version =  "";

#if FAKE_XRM_EASY
           version = "5.0.0.0";
#elif FAKE_XRM_EASY_2013
           version = "6.0.0.0";
#elif FAKE_XRM_EASY_2015
           version = "7.0.0.0"; 
#elif FAKE_XRM_EASY_2016
           version = "8.0.0.0"; 
#elif FAKE_XRM_EASY_365
           version = "8.2.0.0"; 
#elif FAKE_XRM_EASY_9
           version = "9.0.0.0"; 
#endif

            return new RetrieveVersionResponse
            {
                Results = new ParameterCollection
                {
                    { "Version", version }
                }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveVersionRequest);
        }
    }
}
