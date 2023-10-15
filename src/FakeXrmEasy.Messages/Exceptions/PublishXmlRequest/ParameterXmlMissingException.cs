using System;

namespace FakeXrmEasy.FakeMessageExecutors.Exceptions.PublishXmlRequest
{
    /// <summary>
    /// Exception raised when ParameterXml is missing from the PublishXmlRequest
    /// </summary>
    public class ParameterXmlMissingException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterXmlMissingException() : base ("ParameterXml is missing from the PublishXmlRequest")
        {
            
        }
    }
}