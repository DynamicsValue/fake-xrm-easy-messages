using System.Reflection;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.FakeMessageExecutors;
using FakeXrmEasy.FakeMessageExecutors.GenericExecutors;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using Microsoft.Xrm.Sdk;

namespace FakeXrmEasy.Messages.Tests
{
    public class FakeXrmEasyTestsBase
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;
        
        protected FakeXrmEasyTestsBase()
        {
            _context = MiddlewareBuilder
                        .New()
       
                        // Add* -> Middleware configuration
                        .AddCrud()
                        .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(AddListMembersListRequestExecutor)))
                        .AddGenericFakeMessageExecutors(Assembly.GetAssembly(typeof(NavigateToNextEntityOrganizationRequestExecutor)))

                        // Use* -> Defines pipeline sequence
                        .UseCrud()
                        .UseMessages()

                        .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                        .Build();
                        
            _service = _context.GetOrganizationService();
        }
    }
}