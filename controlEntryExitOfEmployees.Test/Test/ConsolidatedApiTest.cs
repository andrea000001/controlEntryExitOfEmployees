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
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactoryConsolidatedTime.CreateLogger();

        [Fact]
        public async void GetAllConsolidatesByDate_Should_Return_200()
        {
            // Arrenge
            MockCloudTableConsolidatedTime mockCloudTableConsolidatedTime = new MockCloudTableConsolidatedTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            string date = "2021-08-10";
            DefaultHttpRequest request = TestFactoryConsolidatedTime.CreateHttpRequest();

            // Act
            IActionResult response = await ConsolidatedApi.GetAllConsolidatesByDate(request, mockCloudTableConsolidatedTime, date, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
