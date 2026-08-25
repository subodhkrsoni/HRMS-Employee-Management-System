using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // ============================
        // Employee List
        // Admin + HR + Employee
        // ============================
        public async Task<IActionResult> Index(string? searchString)
        {
            var employees =
                await _employeeService.GetAllEmployeesAsync(searchString);

            ViewBag.SearchString = searchString;

            return View(employees);
        }

        // ============================
        // Employee Details
        // Admin + HR + Employee
        // ============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var employee =
                await _employeeService.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // ============================
        // Create Employee - GET
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            EmployeeViewModel vm = new EmployeeViewModel();

            var departments =
                await _employeeService.GetDepartmentsAsync();

            vm.Departments = departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();

            vm.Employee.JoiningDate = DateTime.Today;
            vm.Employee.IsActive = true;

            return View(vm);
        }

        // ============================
        // Create Employee - POST
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.AddEmployeeAsync(vm);

                TempData["Success"] =
                    "Employee Added Successfully.";

                return RedirectToAction(nameof(Index));
            }

            var departments =
                await _employeeService.GetDepartmentsAsync();

            vm.Departments = departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();

            return View(vm);
        }

        // ============================
        // Edit Employee - GET
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var employee =
                await _employeeService.GetEmployeeForEditAsync(id.Value);

            if (employee == null)
                return NotFound();

            EmployeeViewModel vm = new EmployeeViewModel
            {
                Employee = employee
            };

            var departments =
                await _employeeService.GetDepartmentsAsync();

            vm.Departments = departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();

            return View(vm);
        }

        // ============================
        // Edit Employee - POST
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            EmployeeViewModel vm)
        {
            if (id != vm.Employee.EmployeeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var employee =
                    await _employeeService.UpdateEmployeeAsync(vm);

                if (employee == null)
                    return NotFound();

                TempData["Success"] =
                    "Employee Updated Successfully.";

                return RedirectToAction(nameof(Index));
            }

            var departments =
                await _employeeService.GetDepartmentsAsync();

            vm.Departments = departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();

            return View(vm);
        }

        // ============================
        // Delete Employee - GET
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee =
                await _employeeService.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // ============================
        // Delete Employee - POST
        // Admin + HR only
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);

            TempData["Success"] =
                "Employee Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}