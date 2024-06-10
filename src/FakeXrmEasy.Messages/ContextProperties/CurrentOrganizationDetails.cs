#if FAKE_XRM_EASY_9 || FAKE_XRM_EASY_365 || FAKE_XRM_EASY_2016 || FAKE_XRM_EASY_2015

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
#endif