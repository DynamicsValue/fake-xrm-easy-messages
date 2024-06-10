using System;

namespace FakeXrmEasy.Messages.Exceptions.NavigateToNextEntityOrganizationRequest
{
    /// <summary>
    /// Exception raised when the entity record of the current stage associated to a given business process does not exist
    /// </summary>
    public class CurrentEntityNotFoundException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="id"></param>
        public CurrentEntityNotFoundException(string logicalName, Guid id): 
            base($"There is no current entity record with logical name '{logicalName}' and Id '{id}'")
        {
            
        }
    }
}