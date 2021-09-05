using controlEntryExitOfEmployees.Common.Models;
using controlEntryExitOfEmployees.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace controlEntryExitOfEmployees.Test.Helpers
{
    public class TestFactory
    {
        public static EmployeeEntity GetEmployeeEntity()
        {
            return new EmployeeEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                EmployeId = 1,
                Date = new DateTime(2021, 08, 10, 6, 0, 0),
                Type = 0,
                IsConsolidated = false,
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid rowKey, Employee employeeRequest)
        {
            string request = JsonConvert.SerializeObject(employeeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{rowKey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid rowKey)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{rowKey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Employee employeeRequest)
        {
            string request = JsonConvert.SerializeObject(employeeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Employee GetEmployeeRequest()
        {
            return new Employee
            {
                EmployeId = 1,
                Date = new DateTime(2021, 08, 10, 6, 0, 0),
                Type = 0,
                IsConsolidated = false
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}