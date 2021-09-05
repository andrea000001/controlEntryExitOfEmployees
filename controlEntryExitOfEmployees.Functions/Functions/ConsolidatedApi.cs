using controlEntryExitOfEmployees.Common.Responses;
using controlEntryExitOfEmployees.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace controlEntryExitOfEmployees.Functions.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName(nameof(GetAllConsolidatesByDate))]
        public static async Task<IActionResult> GetAllConsolidatesByDate(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated/{date}")] HttpRequest req,
           [Table("timeConsolidated", Connection = "AzureWebJobsStorage")] CloudTable timeConsolidatedTable,
           string date,
           ILogger log)
        {
            log.LogInformation($"Get all consolidates by date received.");

            TableQuery<ConsolidatedTimeEntity> query = new TableQuery<ConsolidatedTimeEntity>();
            TableQuerySegment<ConsolidatedTimeEntity> times = await timeConsolidatedTable.ExecuteQuerySegmentedAsync(query, null);

            List<ConsolidatedTimeEntity> consolidatedTimeEntities = new List<ConsolidatedTimeEntity>();

            foreach (ConsolidatedTimeEntity consolidated in times)
            {
                if (consolidated.Date.Value.ToString("yyyy-MM-dd").Equals(date))
                {
                    consolidatedTimeEntities.Add(consolidated);
                }
            }

            if (consolidatedTimeEntities.Count == 0)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Date not found."
                });
            }

            string message = $"Get consolidates by date: {date}, completed.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = consolidatedTimeEntities
            });
        }
    }
}
