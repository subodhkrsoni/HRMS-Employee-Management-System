using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        // ==========================================
        // LEAVE LIST
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leaves =
                await _leaveService.GetAllLeavesAsync();

            return View(leaves);
        }

        // ==========================================
        // CREATE - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();

            var leave = new Leave
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Status = "Pending"
            };

            return View(leave);
        }

        // ==========================================
        // CREATE - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _leaveService.AddLeaveAsync(leave);

                    TempData["Success"] =
                        "Leave application submitted successfully.";

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

            return View(leave);
        }

        // ==========================================
        // EDIT - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var leave =
                await _leaveService
                    .GetLeaveByIdAsync(id.Value);

            if (leave == null)
                return NotFound();

            if (leave.Status != "Pending")
            {
                TempData["Error"] =
                    "Only pending leave can be edited.";

                return RedirectToAction(nameof(Index));
            }

            await LoadEmployeesAsync();

            return View(leave);
        }

        // ==========================================
        // EDIT - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Leave leave)
        {
            if (id != leave.LeaveId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var result =
                        await _leaveService
                            .UpdateLeaveAsync(leave);

                    if (!result)
                        return NotFound();

                    TempData["Success"] =
                        "Leave updated successfully.";

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

            return View(leave);
        }

        // ==========================================
        // DELETE / CANCEL
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result =
                    await _leaveService
                        .DeleteLeaveAsync(id);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Leave cancelled successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // APPROVE
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(
            int id,
            string? comments)
        {
            var currentUserId =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                    currentUserId,
                    out int approvedBy))
            {
                return Forbid();
            }

            try
            {
                var result =
                    await _leaveService
                        .ApproveLeaveAsync(
                            id,
                            approvedBy,
                            comments);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Leave approved successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // REJECT
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(
            int id,
            string? comments)
        {
            var currentUserId =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                    currentUserId,
                    out int rejectedBy))
            {
                return Forbid();
            }

            try
            {
                var result =
                    await _leaveService
                        .RejectLeaveAsync(
                            id,
                            rejectedBy,
                            comments);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Leave rejected successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // LOAD EMPLOYEES
        // ==========================================
        private async Task LoadEmployeesAsync()
        {
            var employees =
                await _leaveService
                    .GetEmployeesAsync();

            ViewBag.Employees =
                employees
                    .Select(e => new SelectListItem
                    {
                        Value =
                            e.EmployeeId.ToString(),

                        Text =
                            $"{e.EmployeeCode} - {e.Name}"
                    })
                    .ToList();
        }
    }
}