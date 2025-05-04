using System;

namespace FakeXrmEasy.Messages.Exceptions.InstantiateTemplateRequest
{
    /// <summary>
    /// Exception raised when the XSLT transform is not valid
    /// </summary>
    public class InvalidXsltAttributeValueException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="attributeName">The attribute name in the template record that is not valid</param>
        public InvalidXsltAttributeValueException(string attributeName) : base(
            $"Template attribute '{attributeName}' doesn't appear to have a valid XSLT format")
        {
            
        }      
    }
}