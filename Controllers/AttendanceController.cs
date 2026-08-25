using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(
            IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // ==========================================
        // ATTENDANCE LIST
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var attendance =
                await _attendanceService.GetAllAttendanceAsync();

            return View(attendance);
        }

        // ==========================================
        // CREATE - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();

            var attendance = new Attendance
            {
                AttendanceDate = DateTime.Today,
                Status = "Present"
            };

            return View(attendance);
        }

        // ==========================================
        // CREATE - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _attendanceService
                        .AddAttendanceAsync(attendance);

                    TempData["Success"] =
                        "Attendance marked successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        ex.Message);
                }
            }

            await LoadEmployeesAsync();

            return View(attendance);
        }

        // ==========================================
        // EDIT - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var attendance =
                await _attendanceService
                    .GetAttendanceByIdAsync(id.Value);

            if (attendance == null)
                return NotFound();

            await LoadEmployeesAsync();

            return View(attendance);
        }

        // ==========================================
        // EDIT - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Attendance attendance)
        {
            if (id != attendance.AttendanceId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var result =
                        await _attendanceService
                            .UpdateAttendanceAsync(attendance);

                    if (!result)
                        return NotFound();

                    TempData["Success"] =
                        "Attendance updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        ex.Message);
                }
            }

            await LoadEmployeesAsync();

            return View(attendance);
        }

        // ==========================================
        // DELETE - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _attendanceService
                    .DeleteAttendanceAsync(id);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "Attendance deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // CHECK IN
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var attendance =
                await _attendanceService
                    .GetAttendanceByIdAsync(id);

            if (attendance == null)
                return NotFound();

            if (attendance.CheckIn.HasValue)
            {
                TempData["Error"] =
                    "Employee is already checked in.";

                return RedirectToAction(nameof(Index));
            }

            if (attendance.Status == "Absent")
            {
                attendance.Status = "Present";
            }

            attendance.CheckIn =
                DateTime.Now.TimeOfDay;

            var result =
                await _attendanceService
                    .UpdateAttendanceAsync(attendance);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "Check-in recorded successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // CHECK OUT
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int id)
        {
            var attendance =
                await _attendanceService
                    .GetAttendanceByIdAsync(id);

            if (attendance == null)
                return NotFound();

            if (!attendance.CheckIn.HasValue)
            {
                TempData["Error"] =
                    "Employee must check-in first.";

                return RedirectToAction(nameof(Index));
            }

            if (attendance.CheckOut.HasValue)
            {
                TempData["Error"] =
                    "Employee is already checked out.";

                return RedirectToAction(nameof(Index));
            }

            attendance.CheckOut =
                DateTime.Now.TimeOfDay;

            var result =
                await _attendanceService
                    .UpdateAttendanceAsync(attendance);

            if (!result)
                return NotFound();

            TempData["Success"] =
                "Check-out recorded successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // LOAD EMPLOYEES
        // ==========================================
        private async Task LoadEmployeesAsync()
        {
            var employees =
                await _attendanceService
                    .GetEmployeesAsync();

            ViewBag.Employees =
                employees
                    .Select(e => new SelectListItem
                    {
                        Value = e.EmployeeId.ToString(),
                        Text =
                            $"{e.EmployeeCode} - {e.Name}"
                    })
                    .ToList();
        }
    }
}