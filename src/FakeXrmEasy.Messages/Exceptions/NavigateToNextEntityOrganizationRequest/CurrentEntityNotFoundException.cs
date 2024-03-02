using System;

namespace FakeXrmEasy.Messages.Exceptions.NavigateToNextEntityOrganizationRequest
{
    public class CurrentEntityNotFoundException: Exception
    {
        public CurrentEntityNotFoundException(string logicalName, Guid id): 
            base($"There is no current entity record with logical name '{logicalName}' and Id '{id}'")
        {
            
        }
    }
}