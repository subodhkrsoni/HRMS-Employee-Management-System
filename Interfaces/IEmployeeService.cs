using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.ViewModels;

namespace EmployeeManagementSystem.Interfaces
{
    public interface IEmployeeService
    {
        // Employee List
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(string? searchString);

        // Employee Details
        Task<Employee?> GetEmployeeByIdAsync(int id);

        // Edit Employee
        Task<Employee?> GetEmployeeForEditAsync(int id);

        // Department List
        Task<List<Department>> GetDepartmentsAsync();

        // Add Employee
        Task AddEmployeeAsync(EmployeeViewModel vm);

        // Update Employee
        Task<Employee?> UpdateEmployeeAsync(EmployeeViewModel vm);

        // Delete Employee
        Task DeleteEmployeeAsync(int id);
    }
}