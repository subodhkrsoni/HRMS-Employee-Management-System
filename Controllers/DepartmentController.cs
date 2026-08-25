using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // Department List
        // Admin + HR + Employee
        // ============================
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .ToListAsync();

            return View(departments);
        }

        // ============================
        // Department Details
        // Admin + HR + Employee
        // ============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // ============================
        // Create Department - GET
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Create()
        {
            return View();
        }

        // ============================
        // Create Department - POST
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Department Added Successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        // ============================
        // Edit Department - GET
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FindAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // ============================
        // Edit Department - POST
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Department department)
        {
            if (id != department.DepartmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Departments.Update(department);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Department Updated Successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        // ============================
        // Delete Department - GET
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // ============================
        // Delete Department - POST
        // Admin + HR
        // ============================
        [Authorize(Roles = "Admin,HR")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var department = await _context.Departments
                .FindAsync(id);

            if (department != null)
            {
                _context.Departments.Remove(department);

                await _context.SaveChangesAsync();
            }

            TempData["Success"] =
                "Department Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}