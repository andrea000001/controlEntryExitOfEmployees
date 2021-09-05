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
    public class MockCloudTableConsolidatedTime : CloudTable
    {
        public MockCloudTableConsolidatedTime(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableConsolidatedTime(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableConsolidatedTime(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        List<ConsolidatedTimeEntity> results = new List<ConsolidatedTimeEntity>() { TestFactoryConsolidatedTime.GetConsolidatedTimeEntity() };
        public override async Task<TableQuerySegment<ConsolidatedTimeEntity>> ExecuteQuerySegmentedAsync<ConsolidatedTimeEntity>(
            TableQuery<ConsolidatedTimeEntity> query,
            TableContinuationToken token)
        {
            var ctor = typeof(TableQuerySegment<ConsolidatedTimeEntity>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);

            TableQuerySegment<ConsolidatedTimeEntity> mock = ctor.Invoke(new object[] { results }) as TableQuerySegment<ConsolidatedTimeEntity>;

            return await Task.Run(() => mock);
        }
    }
}
