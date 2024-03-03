using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the EntityTypeCode parameter is missing
    /// </summary>
    public class EntityTypeCodeNotSpecifiedException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityTypeCodeNotSpecifiedException() : base("When executing ExportPdfDocument request, EntityTypeCode parameter is required but was not specified")
        {
            
        }
    }
}