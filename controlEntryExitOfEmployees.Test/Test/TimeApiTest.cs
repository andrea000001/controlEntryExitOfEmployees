using controlEntryExitOfEmployees.Common.Models;
using controlEntryExitOfEmployees.Functions.Entities;
using controlEntryExitOfEmployees.Functions.Functions;
using controlEntryExitOfEmployees.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace controlEntryExitOfEmployees.Test.Test
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTime mockTableTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Employee employeeRequest = TestFactory.GetEmployeeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(employeeRequest);

            // Act
            IActionResult response = await TimeApi.CreateTime(request, mockTableTime, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTime mockTableTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Employee employeeRequest = TestFactory.GetEmployeeRequest();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId, employeeRequest);

            // Act
            IActionResult response = await TimeApi.UpdateTime(request, mockTableTime, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllTimes_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTime mockTableTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            // Act
            IActionResult response = await TimeApi.GetAllTimes(request, mockTableTime, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetTimeById_Should_Return_200()
        {
            // Arrenge
            EmployeeEntity employeeEntity = TestFactory.GetEmployeeEntity();

            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId);

            // Act
            IActionResult response = TimeApi.GetTimeById(request, employeeEntity, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void DeleteTime_Should_Return_200()
        {
            // Arrenge
            EmployeeEntity employeeEntity = TestFactory.GetEmployeeEntity();

            MockCloudTableTime mockTableTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId);

            // Act
            IActionResult response = await TimeApi.DeleteTime(request, employeeEntity, mockTableTime, timeId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
