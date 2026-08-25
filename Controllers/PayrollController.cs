using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(
            IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }


        // ==========================================
        // PAYROLL LIST
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var payrolls =
                await _payrollService
                    .GetAllPayrollAsync();

            return View(payrolls);
        }


        // ==========================================
        // CREATE - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();

            var payroll = new Payroll
            {
                PayrollMonth = DateTime.Today.Month,
                PayrollYear = DateTime.Today.Year,
                PaymentStatus = "Pending"
            };

            return View(payroll);
        }


        // ==========================================
        // CREATE - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _payrollService
                        .AddPayrollAsync(payroll);

                    TempData["Success"] =
                        "Payroll created successfully.";

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

            return View(payroll);
        }


        // ==========================================
        // EDIT - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var payroll =
                await _payrollService
                    .GetPayrollByIdAsync(id.Value);

            if (payroll == null)
                return NotFound();

            if (payroll.PaymentStatus == "Paid")
            {
                TempData["Error"] =
                    "Paid payroll cannot be edited.";

                return RedirectToAction(nameof(Index));
            }

            await LoadEmployeesAsync();

            return View(payroll);
        }


        // ==========================================
        // EDIT - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Payroll payroll)
        {
            if (id != payroll.PayrollId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var result =
                        await _payrollService
                            .UpdatePayrollAsync(payroll);

                    if (!result)
                        return NotFound();

                    TempData["Success"] =
                        "Payroll updated successfully.";

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

            return View(payroll);
        }


        // ==========================================
        // DELETE
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result =
                    await _payrollService
                        .DeletePayrollAsync(id);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Payroll deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        // ==========================================
        // PAYROLL DETAILS
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var payroll = await _payrollService
                .GetPayrollByIdAsync(id.Value);

            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // ==========================================
        // MARK AS PAID
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            try
            {
                var result =
                    await _payrollService
                        .MarkAsPaidAsync(id);

                if (!result)
                    return NotFound();

                TempData["Success"] =
                    "Payroll marked as paid successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // LOAD ACTIVE EMPLOYEES
        // ==========================================
        private async Task LoadEmployeesAsync()
        {
            var employees =
                await _payrollService
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