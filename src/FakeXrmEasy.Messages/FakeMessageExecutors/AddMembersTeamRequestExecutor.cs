using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;

namespace FakeXrmEasy.FakeMessageExecutors
{
	/// <summary>
	/// Implements a fake OrganizationRequest for AddMembersTeamRequest
	/// </summary>
	public class AddMembersTeamRequestExecutor : IFakeMessageExecutor
	{
		/// <summary>
		/// Determines if the given request can be executed by this executor
		/// </summary>
		/// <param name="request">The OrganizationRequest that is currently executing</param>
		/// <returns></returns>
		public bool CanExecute(OrganizationRequest request)
		{
			return request is AddMembersTeamRequest;
		}

		/// <summary>
		/// Implements the execution of the current request with this executor against a particular XrmFakedContext
		/// </summary>
		/// <param name="request">The current request that is being executed</param>
		/// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
		/// <returns>AddMembersTeamRequestResponse</returns>
		/// <exception cref="Exception"></exception>
		public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
		{
			var req = (AddMembersTeamRequest)request;

			if (req.MemberIds == null)
			{
				throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument, "MemberIds parameter is required");
			}

			if (req.TeamId == Guid.Empty)
			{
				throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.InvalidArgument, "TeamId parameter is required");
			}

			var service = ctx.GetOrganizationService();

			// Find the list
			var team = ctx.CreateQuery("team").FirstOrDefault(e => e.Id == req.TeamId);

			if (team == null)
			{
				throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist, string.Format("Team with Id {0} wasn't found", req.TeamId.ToString()));
			}

			if (team.GetAttributeValue<bool?>("isdefault") == true)
			{
				throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.CannotAddMembersToDefaultTeam, "You cannot join one or more of the teams selected. The membership of default teams cannot be modified.");
			}
			
			foreach (var memberId in req.MemberIds)
			{
				var user = ctx.CreateQuery("systemuser").FirstOrDefault(e => e.Id == memberId);
				if (user == null)
				{
					throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.ObjectDoesNotExist, string.Format("SystemUser with Id {0} wasn't found", memberId.ToString()));
				}

				// Create teammembership
				var teammembership = new Entity("teammembership");
				teammembership["teamid"] = team.Id;
				teammembership["systemuserid"] = memberId;
				service.Create(teammembership);
			}

			return new AddMembersTeamResponse();
		}

		/// <summary>
		/// Returns the type of the concrete OrganizationRequest that this executor implements
		/// </summary>
		/// <returns></returns>
		public Type GetResponsibleRequestType()
		{
			return typeof(AddMembersTeamRequest);
		}
	}
}