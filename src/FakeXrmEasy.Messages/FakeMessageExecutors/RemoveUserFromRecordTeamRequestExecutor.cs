#if FAKE_XRM_EASY_2013 || FAKE_XRM_EASY_2015 || FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_9
using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Permissions;
using FakeXrmEasy.Abstractions.Permissions;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RemoveUserFromRecordTeamRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RemoveUserFromRecordTeamRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RemoveUserFromRecordTeamResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            RemoveUserFromRecordTeamRequest remReq = (RemoveUserFromRecordTeamRequest)request;

            EntityReference target = remReq.Record;
            Guid systemuserId = remReq.SystemUserId;
            Guid teamTemplateId = remReq.TeamTemplateId;

            if (target == null)
            {
                throw FakeOrganizationServiceFaultFactory.New( "Can not remove from team without target");
            }

            if (systemuserId == Guid.Empty)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not remove from team without user");
            }

            if (teamTemplateId == Guid.Empty)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not remove from team without team");
            }

            Entity teamTemplate = ctx.CreateQuery("teamtemplate").FirstOrDefault(p => p.Id == teamTemplateId);
            if (teamTemplate == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("Team template with id=" + teamTemplateId + " does not exist");
            }

            Entity user = ctx.CreateQuery("systemuser").FirstOrDefault(p => p.Id == systemuserId);
            if (user == null)
            {
                throw FakeOrganizationServiceFaultFactory.New("User with id=" + teamTemplateId + " does not exist");
            }

            IOrganizationService service = ctx.GetOrganizationService();

            ctx.GetProperty<IAccessRightsRepository>().RevokeAccessTo(target, user.ToEntityReference());
            Entity team = ctx.CreateQuery("team").FirstOrDefault(p => ((EntityReference)p["teamtemplateid"]).Id == teamTemplateId);
            if (team == null)
            {
                return new RemoveUserFromRecordTeamResponse
                {
                    ResponseName = "RemoveUserFromRecordTeam"
                };
            }
                
            Entity tm = ctx.CreateQuery("teammembership").FirstOrDefault(p => (Guid)p["teamid"] == team.Id);
            if (tm != null)
            {
                service.Delete(tm.LogicalName, tm.Id);
            }

            return new RemoveUserFromRecordTeamResponse
            {
                ResponseName = "RemoveUserFromRecordTeam"
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RemoveUserFromRecordTeamRequestExecutor);
        }
    }
}
#endif