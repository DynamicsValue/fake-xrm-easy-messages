using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions.Metadata;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveOptionSetRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveOptionSetRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveOptionSetResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var retrieveOptionSetRequest = (RetrieveOptionSetRequest)request;

            if (retrieveOptionSetRequest.MetadataId != Guid.Empty)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist, $"Could not find optionset with optionset id: {retrieveOptionSetRequest.MetadataId}");
            }

            var name = retrieveOptionSetRequest.Name;

            if (string.IsNullOrEmpty(name))
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument, "Name is required when optionSet id is not specified");
            }

            var optionSetMetadataRepository = ctx.GetProperty<IOptionSetMetadataRepository>();

            var optionSetMetadata = optionSetMetadataRepository.GetByName(name);
            if (optionSetMetadata == null)
            {
                throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist, string.Format("An OptionSetMetadata with the name {0} does not exist.", name));
            }

            var response = new RetrieveOptionSetResponse()
            {
                Results = new ParameterCollection
                        {
                            { "OptionSetMetadata", optionSetMetadata }
                        }
            };

            return response;
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveOptionSetRequest);
        }
    }
}