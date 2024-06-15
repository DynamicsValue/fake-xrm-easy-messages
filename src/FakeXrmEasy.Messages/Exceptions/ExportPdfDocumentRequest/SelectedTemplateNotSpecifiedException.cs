using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedTemplate parameter is missing
    /// </summary>
    public class SelectedTemplateNotSpecifiedException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedTemplateNotSpecifiedException() : base("When executing ExportPdfDocument request, SelectedTemplate parameter is required but was not specified")
        {
            
        }
    }
}