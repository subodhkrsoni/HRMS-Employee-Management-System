using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }

        public int TotalDepartments { get; set; }

        public int ActiveEmployees { get; set; }

        public int InactiveEmployees { get; set; }

        public decimal HighestSalary { get; set; }

        public decimal AverageSalary { get; set; }

        public List<Employee> RecentEmployees { get; set; } = new();
    }
}