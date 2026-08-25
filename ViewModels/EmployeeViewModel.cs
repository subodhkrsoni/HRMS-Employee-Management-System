using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; } = new();

        public IFormFile? Photo { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();
    }
}