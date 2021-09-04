using controlEntryExitOfEmployees.Common.Models;
using controlEntryExitOfEmployees.Common.Responses;
using controlEntryExitOfEmployees.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace controlEntryExitOfEmployees.Functions.Functions
{
    public static class TimeApi
    {
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new time.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);

                if (employee == null || employee?.EmployeId == null || employee?.Type == null || employee?.Date == null)
                {
                    return new BadRequestObjectResult(new Response
                    {
                        IsSuccess = false,
                        Message = "the EmployeId, Type and Date fields are required."
                    });
                }

            EmployeeEntity employeeEntity = new EmployeeEntity
            {
                EmployeId = (int)employee.EmployeId,
                Date = (DateTime)employee.Date,
                Type = (int)employee.Type,
                IsConsolidated = false,
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString()
            };

            TableOperation addOperation = TableOperation.Insert(employeeEntity);
            await timeTable.ExecuteAsync(addOperation);

            string message = "New time stored in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeEntity
            });
        }

        [FunctionName(nameof(UpdateTime))]
        public static async Task<IActionResult> UpdateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "time/{id}")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for time: {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Employee employee = JsonConvert.DeserializeObject<Employee>(requestBody);

            //validate time id
            TableOperation findOperation = TableOperation.Retrieve<EmployeeEntity>("TIME", id);
            TableResult findResult = await timeTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "time not found."
                });
            }

            //Update time
            EmployeeEntity employeeEntity = (EmployeeEntity)findResult.Result;
            employeeEntity.IsConsolidated = employee.IsConsolidated;

            if (employee != null || employee?.EmployeId != null || employee?.Type != null || employee?.Date != null)
            {
                employeeEntity.EmployeId = (int)employee.EmployeId;
                employeeEntity.Type = (int)employee.Type;
                employeeEntity.Date = (DateTime)employee.Date;
            }

            TableOperation addOperation = TableOperation.Replace(employeeEntity);
            await timeTable.ExecuteAsync(addOperation);

            string message = $"Time: {id}, update in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeEntity
            });
        }

        [FunctionName(nameof(GetAllTimes))]
        public static async Task<IActionResult> GetAllTimes(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time")] HttpRequest req,
           [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
           ILogger log)
        {
            log.LogInformation("Get all timed received.");

            TableQuery<EmployeeEntity> query = new TableQuery<EmployeeEntity>();
            TableQuerySegment<EmployeeEntity> times = await timeTable.ExecuteQuerySegmentedAsync(query, null);
            
            string message = "Retrieved all times.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = times
            });
        }

        [FunctionName(nameof(GetTimeById))]
        public static IActionResult GetTimeById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time/{id}")] HttpRequest req,
           [Table("time","TIME","{id}",Connection = "AzureWebJobsStorage")] EmployeeEntity employeeEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get time by id: {id}, received.");

            if (employeeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Time not found."
                });
            }

            string message = $"Time: {id}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeEntity
            });
        }

        [FunctionName(nameof(DeleteTime))]
        public static async Task<IActionResult> DeleteTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "time/{id}")] HttpRequest req,
            [Table("time", "TIME", "{id}", Connection = "AzureWebJobsStorage")] EmployeeEntity employeeEntity,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete time: {id}, received.");

            if (employeeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Time not found."
                });
            }

            await timeTable.ExecuteAsync(TableOperation.Delete(employeeEntity));
            string message = $"Time: {id}, deleted.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeEntity
            });
        }
    }
}
