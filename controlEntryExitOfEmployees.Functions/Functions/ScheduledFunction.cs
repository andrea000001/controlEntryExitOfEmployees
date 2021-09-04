using controlEntryExitOfEmployees.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace controlEntryExitOfEmployees.Functions.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("timeConsolidated", Connection = "AzureWebJobsStorage")] CloudTable timeConsolidatedTable,
            ILogger log)
        {
            log.LogInformation($"Received a new consolidation process, starts at: {DateTime.Now}");

            TableQuery<EmployeeEntity> query = new TableQuery<EmployeeEntity>();

            TableQuerySegment<EmployeeEntity> times = await timeTable.
                ExecuteQuerySegmentedAsync(query.Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "TIME")), null);

            List<EmployeeEntity> employeesTimes = times.Results.GroupBy(x => x.EmployeId).Select(x => x.First()).ToList();

            foreach (EmployeeEntity employee in employeesTimes)
            {
                CheckIfItIsConsolidated(timeTable, timeConsolidatedTable, employee.EmployeId);
            }
        }

        public static async Task CheckIfItIsConsolidated(
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("timeConsolidated", Connection = "AzureWebJobsStorage")] CloudTable timeConsolidatedTable,
            int EmployeId)
        {
            TableQuery<EmployeeEntity> query = new TableQuery<EmployeeEntity>().Where(
                  TableQuery.CombineFilters(
                     TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.NotEqual, true),
                     TableOperators.And,
                     TableQuery.GenerateFilterConditionForInt("EmployeId", QueryComparisons.Equal, EmployeId)));

            TableQuerySegment<EmployeeEntity> queryResultList = await timeTable.ExecuteQuerySegmentedAsync(query, null);

            queryResultList.Results.Sort((x, y) => x.Date.Hour.CompareTo(y.Date.Hour));

            List<EmployeeEntity> employees = new List<EmployeeEntity>();

            if (queryResultList.Results.Count != 0)
            {
                foreach (EmployeeEntity employee in queryResultList)
                {
                    employees.Add(employee);
                }
            }

            CalculateMinutesWorked(timeConsolidatedTable, employees);

            UpdateConsolidatedTime(timeTable, employees);

        }

        public static async Task SaveTimeConsolidate(
          [Table("timeConsolidated", Connection = "AzureWebJobsStorage")] CloudTable timeConsolidatedTable,
          EmployeeEntity employee,
          int minutesWorked)
        {
            ConsolidatedTimeEntity consolidatedTimeEntity = new ConsolidatedTimeEntity
            {
                EmployeId = employee.EmployeId,
                Date = employee.Date,
                MinutesWorked = minutesWorked,
                ETag = "*",
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString()
            };

            TableOperation addOperation = TableOperation.Insert(consolidatedTimeEntity);
            await timeConsolidatedTable.ExecuteAsync(addOperation);
        }

        public static void CalculateMinutesWorked(
            [Table("timeConsolidated", Connection = "AzureWebJobsStorage")] CloudTable timeConsolidatedTable,
            List<EmployeeEntity> employees)
        {
            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].Type == 1 || (i == employees.Count - 1 && employees[i].Type == 0))
                {
                    continue;
                }

                TimeSpan minutesWorked = employees[i + 1].Date - employees[i].Date;

                SaveTimeConsolidate(timeConsolidatedTable, employees[i], (int)minutesWorked.TotalMinutes);
            }
        }

        public static async Task UpdateConsolidatedTime(
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            List<EmployeeEntity> employees)
        {
            for (int i = 0; i < employees.Count; i++)
            {
                if ((employees[i].Type == 1 && i == 0) || (i == employees.Count - 1 && employees[i].Type == 0))
                {
                    continue;
                }

                employees[i].IsConsolidated = true;

                TableOperation addOperation = TableOperation.Replace(employees[i]);
                await timeTable.ExecuteAsync(addOperation);
            }
        }
    }
}
