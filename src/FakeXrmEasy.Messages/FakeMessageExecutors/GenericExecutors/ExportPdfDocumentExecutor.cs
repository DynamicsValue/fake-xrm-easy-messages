using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Messages.Exceptions.ExportPdfDocumentRequest;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.FakeMessageExecutors.GenericExecutors
{
    /// <summary>
    /// This message is an internal message which is only available for Dynamics 365 for Sales
    /// </summary>
    public class ExportPdfDocumentExecutor: IGenericFakeMessageExecutor
    {
        private const string _requestName = "ExportPdfDocument";
        private const string _entityTypeCodeParameterName = "EntityTypeCode";
        private const string _selectedTemplateParameterName = "SelectedTemplate";
        private const string _selectedRecordsParameterName = "SelectedRecords";
        private const string _documentTemplateLogicalName = "documenttemplate";
        
        /// <summary>
        /// Determines if the current request can be executed by this executor
        /// </summary>
        /// <param name="request">The request being executed</param>
        /// <returns>true if it can be executed, false otherwise</returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return _requestName.Equals(request.RequestName);
        }

        private void ValidateRequest(OrganizationRequest request)
        {
            if (!request.Parameters.ContainsKey(_entityTypeCodeParameterName))
            {
                throw new EntityTypeCodeNotSpecifiedException();
            }
            if (!request.Parameters.ContainsKey(_selectedTemplateParameterName))
            {
                throw new SelectedTemplateNotSpecifiedException();
            }
            
            if (!request.Parameters.ContainsKey(_selectedRecordsParameterName))
            {
                throw new SelectedRecordsNotSpecifiedException();
            }
        }

        private void GetTemplate(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var documentTemplateReference = request.Parameters[_selectedTemplateParameterName] as EntityReference;
            if (documentTemplateReference == null)
            {
                throw new SelectedTemplateMustBeEntityReferenceException();
            }

            var existingTemplate = ctx.CreateQuery(_documentTemplateLogicalName)
                .FirstOrDefault(dt => dt.Id == documentTemplateReference.Id);

            if (existingTemplate == null)
            {
                throw new SelectedTemplateNotFoundException();
            }
        }

        private int GetEntityTypeCode(OrganizationRequest request)
        {
            var entityTypeCode = request.Parameters[_entityTypeCodeParameterName] as int?;
            if (entityTypeCode == null)
            {
                throw new EntityTypeCodeNotValidException();
            }

            return entityTypeCode.Value;
        }

        private List<Guid> GetSelectedRecords(OrganizationRequest request)
        {
            List<Guid> selectedRecords = null;
            
            try
            {
                var selectedRecordsString =
                    JsonStringToObject<List<string>>((string)request.Parameters[_selectedRecordsParameterName]);

                selectedRecords = selectedRecordsString.Select(s => new Guid(s)).ToList();
            }
            catch
            {
                throw new SelectedRecordsIsNotValidException();
            }

            if (!selectedRecords.Any())
            {
                throw new SelectedRecordsIsNotValidException();
            }

            return selectedRecords;
        }
        
        /// <summary>
        /// Executes the current request
        /// </summary>
        /// <param name="request">The request to be executed</param>
        /// <param name="ctx">The current IXrmFakedContext that will be used to execute the request</param>
        /// <returns>A generic OrganizationResponse with a dummy byte array representing the exported Pdf</returns>
        /// <exception cref="SelectedRecordsNotFoundException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            ValidateRequest(request);
            GetTemplate(request, ctx);
            var selectedRecords = GetSelectedRecords(request);
            var entityTypeCode = GetEntityTypeCode(request);
            
            var reflectedType = ctx.FindReflectedType(entityTypeCode);
            if(reflectedType != null)
            {
                var logicalName = reflectedType.GetField("EntityLogicalName").GetValue(null) as string;
                foreach (var selectedRecord in selectedRecords)
                {
                    var record = ctx.CreateQuery(logicalName).FirstOrDefault(r => r.Id.Equals(selectedRecord));
                    if (record == null)
                    {
                        throw new SelectedRecordsNotFoundException();
                    }
                }
            }
            
            return new OrganizationResponse()
            {
                ResponseName = "ExportPdfDocumentResponse",
                Results = new ParameterCollection()
                {
                    { "PdfFile", new byte[] { } }
                }
            };
        }

        /// <summary>
        /// Returns the responsible request type for this request.
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(OrganizationRequest);
        }

        /// <summary>
        /// Returns the request name that this executor implements
        /// </summary>
        /// <returns></returns>
        public string GetRequestName()
        {
            return _requestName;
        }
        
        private T JsonStringToObject<T>(string jsonString)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));

                return (T)dataContractJsonSerializer.ReadObject(memoryStream);
            }
        }
        
    }
}