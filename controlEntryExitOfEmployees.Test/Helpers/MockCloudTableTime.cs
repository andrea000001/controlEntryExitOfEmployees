using controlEntryExitOfEmployees.Functions.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace controlEntryExitOfEmployees.Test.Helpers
{
    public class MockCloudTableTime : CloudTable
    {
        public MockCloudTableTime(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableTime(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableTime(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetEmployeeEntity()
            });
        }

        public override async Task<TableQuerySegment<EmployeeEntity>> ExecuteQuerySegmentedAsync<EmployeeEntity>(
            TableQuery<EmployeeEntity> query, 
            TableContinuationToken token)
        {
            var ctor = typeof(TableQuerySegment<EmployeeEntity>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);

            TableQuerySegment<EmployeeEntity> mock = ctor.Invoke(new object[] { TestFactory.GetListEmployeeEntity() }) as TableQuerySegment<EmployeeEntity>;

            return await Task.Run(() => mock);
        }        
    }
}
