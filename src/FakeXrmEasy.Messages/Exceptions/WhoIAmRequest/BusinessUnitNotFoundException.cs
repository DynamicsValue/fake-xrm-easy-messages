using System;

namespace FakeXrmEasy.FakeMessageExecutors.Exceptions.WhoIAmRequest
{
    public class BusinessUnitNotFoundException: Exception
    {
        public BusinessUnitNotFoundException()
        {
            
        }
        public BusinessUnitNotFoundException(Guid businessUnitId) 
            : base($"The current user 'callerId' has a business unit property id value '{businessUnitId.ToString()}' that was not initialized in the context.")
        {
            
        }
    }
}