using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedRecords parameter is missing
    /// </summary>
    public class SelectedRecordsNotSpecifiedException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedRecordsNotSpecifiedException() : base("When executing ExportPdfDocument request, SelectedRecords parameter is required but was not specified")
        {
            
        }
    }
}