using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedRecords parameter is valid but not found
    /// </summary>
    public class SelectedRecordsNotFoundException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedRecordsNotFoundException() : base("When executing ExportPdfDocument request, SelectedRecords parameter was specified but at least one record was not found")
        {
            
        }
    }
}