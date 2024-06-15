using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedTemplate was specified but not found (it doesn't exists)
    /// </summary>
    public class SelectedTemplateNotFoundException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedTemplateNotFoundException() : base("When executing ExportPdfDocument request, a SelectedTemplate was specified in the request but not found")
        {
            
        }
    }
}