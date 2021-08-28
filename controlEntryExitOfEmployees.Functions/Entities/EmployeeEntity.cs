using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace controlEntryExitOfEmployees.Functions.Entities
{
    public class EmployeeEntity : TableEntity
    {
        public int EmployeId { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
