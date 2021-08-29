using System;

namespace controlEntryExitOfEmployees.Common.Models
{
    public class Employee
    {
        public int EmployeId { get; set; } = -1;

        public DateTime Date { get; set; }

        public int Type { get; set; } = -1;

        public bool IsConsolidated { get; set; }
    }
}
