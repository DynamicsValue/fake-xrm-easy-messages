using Microsoft.Xrm.Sdk.Organization;

namespace FakeXrmEasy.Messages.ContextProperties
{
    /// <summary>
    /// Stores information about the current organization details
    /// </summary>
    public class CurrentOrganizationDetails
    {
        /// <summary>
        /// The organization details
        /// </summary>
        public OrganizationDetail Details { get; set; }
    }
}