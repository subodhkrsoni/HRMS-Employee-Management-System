using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // DASHBOARD
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;


            // ==========================================
            // EMPLOYEES
            // ==========================================

            ViewBag.TotalEmployees =
                await _context.Employees.CountAsync();

            ViewBag.ActiveEmployees =
                await _context.Employees
                    .CountAsync(e => e.IsActive);


            // ==========================================
            // DEPARTMENTS
            // ==========================================

            ViewBag.TotalDepartments =
                await _context.Departments.CountAsync();


            // ==========================================
            // TODAY ATTENDANCE
            // ==========================================

            var todayAttendance =
                await _context.Attendances
                    .Where(a =>
                        a.AttendanceDate.Date == today)
                    .ToListAsync();


            ViewBag.PresentToday =
                todayAttendance.Count(a =>
                    a.Status == "Present");


            ViewBag.AbsentToday =
                todayAttendance.Count(a =>
                    a.Status == "Absent");


            ViewBag.LateToday =
                todayAttendance.Count(a =>
                    a.Status == "Late");


            ViewBag.HalfDayToday =
                todayAttendance.Count(a =>
                    a.Status == "Half Day");


            // ==========================================
            // ATTENDANCE PERCENTAGE
            // Present = 1
            // Late = 1
            // Half Day = 0.5
            // ==========================================

            var attendanceTotal =
                todayAttendance.Count;


            var attendanceScore =
                todayAttendance.Sum(a =>
                    a.Status == "Present" ? 1.0 :
                    a.Status == "Late" ? 1.0 :
                    a.Status == "Half Day" ? 0.5 :
                    0.0);


            ViewBag.AttendancePercentage =
                attendanceTotal > 0
                    ? Math.Round(
                        (attendanceScore /
                         attendanceTotal) * 100,
                        2)
                    : 0;


            // ==========================================
            // LEAVE
            // ==========================================

            ViewBag.PendingLeaves =
                await _context.Leaves
                    .CountAsync(l =>
                        l.Status == "Pending");


            ViewBag.ApprovedLeaves =
                await _context.Leaves
                    .CountAsync(l =>
                        l.Status == "Approved");


            ViewBag.RejectedLeaves =
                await _context.Leaves
                    .CountAsync(l =>
                        l.Status == "Rejected");


            // ==========================================
            // PAYROLL COUNT
            // ==========================================

            ViewBag.PendingPayrollCount =
                await _context.Payrolls
                    .CountAsync(p =>
                        p.PaymentStatus == "Pending");


            ViewBag.PaidPayrollCount =
                await _context.Payrolls
                    .CountAsync(p =>
                        p.PaymentStatus == "Paid");


            // ==========================================
            // PAYROLL AMOUNTS
            // ==========================================

            ViewBag.PendingPayrollAmount =
                await _context.Payrolls
                    .Where(p =>
                        p.PaymentStatus == "Pending")
                    .SumAsync(p =>
                        (decimal?)p.NetSalary)
                ?? 0;


            ViewBag.PaidPayrollAmount =
                await _context.Payrolls
                    .Where(p =>
                        p.PaymentStatus == "Paid")
                    .SumAsync(p =>
                        (decimal?)p.NetSalary)
                ?? 0;


            // ==========================================
            // TOTAL PAYROLL
            // ==========================================

            ViewBag.TotalPayrollAmount =
                await _context.Payrolls
                    .SumAsync(p =>
                        (decimal?)p.NetSalary)
                ?? 0;


            // ==========================================
            // RECENT ATTENDANCE
            // ==========================================

            ViewBag.RecentAttendance =
                await _context.Attendances
                    .Include(a => a.Employee)
                    .OrderByDescending(a =>
                        a.AttendanceDate)
                    .ThenByDescending(a =>
                        a.AttendanceId)
                    .Take(5)
                    .ToListAsync();


            // ==========================================
            // RECENT LEAVES
            // ==========================================

            ViewBag.RecentLeaves =
                await _context.Leaves
                    .Include(l => l.Employee)
                    .OrderByDescending(l =>
                        l.CreatedDate)
                    .ThenByDescending(l =>
                        l.LeaveId)
                    .Take(5)
                    .ToListAsync();


            // ==========================================
            // RECENT PAYROLL
            // ==========================================

            ViewBag.RecentPayroll =
                await _context.Payrolls
                    .Include(p => p.Employee)
                    .OrderByDescending(p =>
                        p.CreatedDate)
                    .ThenByDescending(p =>
                        p.PayrollId)
                    .Take(5)
                    .ToListAsync();


            // ==========================================
            // RETURN DASHBOARD
            // ==========================================

            return View();
        }
    }
}
