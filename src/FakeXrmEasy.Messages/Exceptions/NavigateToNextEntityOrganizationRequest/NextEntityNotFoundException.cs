using System;

namespace FakeXrmEasy.Messages.Exceptions.NavigateToNextEntityOrganizationRequest
{
    /// <summary>
    /// Exception raised when the record of the the next stage in a given business process does not exist
    /// </summary>
    public class NextEntityNotFoundException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="id"></param>
        public NextEntityNotFoundException(string logicalName, Guid id): 
            base($"There is no next entity record with logical name '{logicalName}' and Id '{id}'")
        {
            
        }
    }
}