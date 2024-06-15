using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the EntityTypeCode parameter is not a valid integer
    /// </summary>
    public class EntityTypeCodeNotValidException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityTypeCodeNotValidException() : base("When executing ExportPdfDocument request, EntityTypeCode parameter is required but is not valid")
        {
            
        }
    }
}