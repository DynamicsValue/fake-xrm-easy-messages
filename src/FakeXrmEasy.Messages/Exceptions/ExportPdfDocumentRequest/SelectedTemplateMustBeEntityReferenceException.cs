using System;

namespace FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest
{
    /// <summary>
    /// Exception raised when the SelectedTemplate was specified but not a valid EntityReference
    /// </summary>
    public class SelectedTemplateMustBeEntityReferenceException: Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectedTemplateMustBeEntityReferenceException() : base("When executing ExportPdfDocument request, SelectedTemplate must be an EntityReference to a document template record")
        {
            
        }
    }
}