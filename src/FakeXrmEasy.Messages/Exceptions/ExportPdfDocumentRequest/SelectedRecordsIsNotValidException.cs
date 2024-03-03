using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedRecords parameter doesn't have a valid format
    /// </summary>
    public class SelectedRecordsIsNotValidException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedRecordsIsNotValidException() : base("When executing ExportPdfDocument request, SelectedRecords parameter is required but not valid. Must be a string representation of an array of Guid's")
        {
            
        }
    }
}