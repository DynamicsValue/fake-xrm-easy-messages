using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;

namespace FakeXrmEasy.FakeMessageExecutors
{
    public class RetrieveExchangeRateRequestExecutor : IFakeMessageExecutor
    {
        /// <summary>
        /// Determines if the given request can be executed by this executor
        /// </summary>
        /// <param name="request">The OrganizationRequest that is currently executing</param>
        /// <returns></returns>
        public bool CanExecute(OrganizationRequest request)
        {
            return request is RetrieveExchangeRateRequest;
        }

        /// <summary>
        /// Implements the execution of the current request with this executor against a particular XrmFakedContext
        /// </summary>
        /// <param name="request">The current request that is being executed</param>
        /// <param name="ctx">The instance of an XrmFakedContext that the request will be executed against</param>
        /// <returns>RetrieveExchangeRateResponse</returns>
        /// <exception cref="Exception"></exception>
        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            var retrieveExchangeRateRequest = (RetrieveExchangeRateRequest)request;

            var currencyId = retrieveExchangeRateRequest.TransactionCurrencyId;

            if (currencyId == Guid.Empty)
            {
                throw FakeOrganizationServiceFaultFactory.New("Can not retrieve Exchange Rate without Transaction Currency Guid");
            }

            var service = ctx.GetOrganizationService();

            var result = service.RetrieveMultiple(new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("exchangerate"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("transactioncurrencyid", ConditionOperator.Equal, currencyId)
                    }
                }
            }).Entities;

            if (!result.Any())
            {
                throw FakeOrganizationServiceFaultFactory.New("Transaction Currency not found");
            }

            var exchangeRate = result.First().GetAttributeValue<decimal>("exchangerate");

            return new RetrieveExchangeRateResponse
            {
                Results = new ParameterCollection
                {
                    {"ExchangeRate", exchangeRate}
                }
            };
        }

        /// <summary>
        /// Returns the type of the concrete OrganizationRequest that this executor implements
        /// </summary>
        /// <returns></returns>
        public Type GetResponsibleRequestType()
        {
            return typeof(RetrieveExchangeRateRequest);
        }
    }
}