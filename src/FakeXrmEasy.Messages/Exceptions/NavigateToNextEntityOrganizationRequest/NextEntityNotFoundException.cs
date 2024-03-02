using System;

namespace FakeXrmEasy.Messages.Exceptions.NavigateToNextEntityOrganizationRequest
{
    public class NextEntityNotFoundException: Exception
    {
        public NextEntityNotFoundException(string logicalName, Guid id): 
            base($"There is no next entity record with logical name '{logicalName}' and Id '{id}'")
        {
            
        }
    }
}