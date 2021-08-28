using System;

namespace controlEntryExitOfEmployees.Common.Models
{
    public class Employee
    {
        public int EmployeId { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
