using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // REPORTS HOME
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index(
            int? month,
            int? year)
        {
            var query = _context.Payrolls
                .Include(p => p.Employee)
                .AsQueryable();

            // Month filter
            if (month.HasValue)
            {
                query = query.Where(
                    p => p.PayrollMonth == month.Value);
            }

            // Year filter
            if (year.HasValue)
            {
                query = query.Where(
                    p => p.PayrollYear == year.Value);
            }

            var payrolls = await query
                .OrderByDescending(p => p.PayrollYear)
                .ThenByDescending(p => p.PayrollMonth)
                .ThenBy(p => p.Employee!.Name)
                .ToListAsync();

            ViewBag.Month = month;
            ViewBag.Year = year;

            return View(payrolls);
        }


        // ==========================================
        // ATTENDANCE REPORT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Attendance(
            int? employeeId,
            int? month,
            int? year,
            string? status)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            // ==========================================
            // EMPLOYEE FILTER
            // ==========================================

            if (employeeId.HasValue)
            {
                query = query.Where(a =>
                    a.EmployeeId == employeeId.Value);
            }

            // ==========================================
            // MONTH FILTER
            // ==========================================

            if (month.HasValue)
            {
                query = query.Where(a =>
                    a.AttendanceDate.Month == month.Value);
            }

            // ==========================================
            // YEAR FILTER
            // ==========================================

            if (year.HasValue)
            {
                query = query.Where(a =>
                    a.AttendanceDate.Year == year.Value);
            }

            // ==========================================
            // STATUS FILTER
            // ==========================================

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a =>
                    a.Status == status);
            }

            // ==========================================
            // GET ATTENDANCE
            // ==========================================

            var attendance = await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenBy(a => a.Employee!.Name)
                .ToListAsync();

            // ==========================================
            // EMPLOYEE DROPDOWN
            // ==========================================

            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();

            // ==========================================
            // VIEWBAG
            // ==========================================

            ViewBag.Employees = employees;

            ViewBag.SelectedEmployee = employeeId;
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.SelectedStatus = status;

            return View(attendance);
        }
        // ==========================================
        // LEAVE REPORT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Leave(
            int? employeeId,
            int? month,
            int? year,
            string? status,
            string? leaveType)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .AsQueryable();

            // ==========================================
            // EMPLOYEE FILTER
            // ==========================================

            if (employeeId.HasValue)
            {
                query = query.Where(l =>
                    l.EmployeeId == employeeId.Value);
            }

            // ==========================================
            // MONTH FILTER
            // ==========================================

            if (month.HasValue)
            {
                query = query.Where(l =>
                    l.StartDate.Month == month.Value);
            }

            // ==========================================
            // YEAR FILTER
            // ==========================================

            if (year.HasValue)
            {
                query = query.Where(l =>
                    l.StartDate.Year == year.Value);
            }

            // ==========================================
            // STATUS FILTER
            // ==========================================

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l =>
                    l.Status == status);
            }

            // ==========================================
            // LEAVE TYPE FILTER
            // ==========================================

            if (!string.IsNullOrWhiteSpace(leaveType))
            {
                query = query.Where(l =>
                    l.LeaveType == leaveType);
            }

            // ==========================================
            // GET LEAVES
            // ==========================================

            var leaves = await query
                .OrderByDescending(l => l.StartDate)
                .ThenBy(l => l.Employee!.Name)
                .ToListAsync();

            // ==========================================
            // EMPLOYEE DROPDOWN
            // ==========================================

            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();

            // ==========================================
            // VIEWBAG
            // ==========================================

            ViewBag.Employees = employees;

            ViewBag.SelectedEmployee = employeeId;
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedLeaveType = leaveType;

            return View(leaves);
        }
    }
}