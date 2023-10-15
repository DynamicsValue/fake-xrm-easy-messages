using System;

namespace FakeXrmEasy.FakeMessageExecutors.Exceptions.WhoIAmRequest
{
    /// <summary>
    /// Throws an exception when a business unit id was set in the CallerProperties that wasn't previously initialised
    /// </summary>
    public class BusinessUnitNotFoundException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BusinessUnitNotFoundException()
        {
            
        }
        
        /// <summary>
        /// Exception constructor with the offending business unit id
        /// </summary>
        /// <param name="businessUnitId">The business unit id that was not initialised</param>
        public BusinessUnitNotFoundException(Guid businessUnitId) 
            : base($"The current user 'callerId' has a business unit property id value '{businessUnitId.ToString()}' that was not initialized in the context.")
        {
            
        }
    }
}