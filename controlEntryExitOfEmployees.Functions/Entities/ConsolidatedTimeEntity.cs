using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace controlEntryExitOfEmployees.Functions.Entities
{
    public class ConsolidatedTimeEntity : TableEntity
    {
        public int EmployeId { get; set; }

        public DateTime? Date { get; set; }

        public int MinutesWorked { get; set; }
    }
}
