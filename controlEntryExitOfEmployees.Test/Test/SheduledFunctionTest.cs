using controlEntryExitOfEmployees.Functions.Functions;
using controlEntryExitOfEmployees.Test.Helpers;
using System;
using Xunit;

namespace controlEntryExitOfEmployees.Test.Test
{
    public class SheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTime mockTableTime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidatedTime mockCloudTableConsolidatedTime = new MockCloudTableConsolidatedTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));

            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            ScheduledFunction.Run(null, mockTableTime, mockCloudTableConsolidatedTime, logger);
            string message = logger.Logs[0];

            // Asert
            Assert.Contains("Received a new consolidation process", message);
        }
    }
}
