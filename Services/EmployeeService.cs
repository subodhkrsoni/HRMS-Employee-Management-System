using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IWebHostEnvironment _environment;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IWebHostEnvironment environment)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _environment = environment;
        }

        // ===============================
        // Employee List
        // ===============================
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(string? searchString)
        {
            var employees = await _employeeRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                employees = employees.Where(e =>
                    (!string.IsNullOrEmpty(e.EmployeeCode) &&
                     e.EmployeeCode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrEmpty(e.Name) &&
                     e.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrEmpty(e.Email) &&
                     e.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    ||
                    (e.Department != null &&
                     !string.IsNullOrEmpty(e.Department.DepartmentName) &&
                     e.Department.DepartmentName.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }

            return employees;
        }

        // ===============================
        // Details
        // ===============================
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        // ===============================
        // Edit
        // ===============================
        public async Task<Employee?> GetEmployeeForEditAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        // ===============================
        // Department List
        // ===============================
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return (await _departmentRepository.GetAllAsync()).ToList();
        }

        // ===============================
        // Add Employee
        // ===============================
        public async Task AddEmployeeAsync(EmployeeViewModel vm)
        {
            var photoPath = await UploadPhotoAsync(vm.Photo);

            if (photoPath != null)
            {
                vm.Employee.PhotoPath = photoPath;
            }

            vm.Employee.CreatedDate = DateTime.Now;

            await _employeeRepository.AddAsync(vm.Employee);

            await _employeeRepository.SaveAsync();
        }

        // ===============================
        // Update Employee
        // ===============================
        public async Task<Employee?> UpdateEmployeeAsync(EmployeeViewModel vm)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(vm.Employee.EmployeeId);

            if (employee == null)
                return null;

            employee.EmployeeCode = vm.Employee.EmployeeCode;
            employee.Name = vm.Employee.Name;
            employee.Email = vm.Employee.Email;
            employee.Phone = vm.Employee.Phone;
            employee.Gender = vm.Employee.Gender;
            employee.Designation = vm.Employee.Designation;
            employee.DepartmentId = vm.Employee.DepartmentId;
            employee.Salary = vm.Employee.Salary;
            employee.JoiningDate = vm.Employee.JoiningDate;
            employee.Address = vm.Employee.Address;
            employee.IsActive = vm.Employee.IsActive;

            var photoPath = await UploadPhotoAsync(vm.Photo);

            if (photoPath != null)
            {
                employee.PhotoPath = photoPath;
            }

            await _employeeRepository.UpdateAsync(employee);

            await _employeeRepository.SaveAsync();

            return employee;
        }

        // ===============================
        // Delete
        // ===============================
        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeeRepository.DeleteAsync(id);

            await _employeeRepository.SaveAsync();
        }

        private async Task<string?> UploadPhotoAsync(IFormFile? photo)
        {
            if (photo == null)
                return null;

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(photo.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using FileStream stream = new(filePath, FileMode.Create);

            await photo.CopyToAsync(stream);

            return "/uploads/" + fileName;
        }
    }
}