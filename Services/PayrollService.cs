using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;

        public PayrollService(
            IPayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository;
        }


        // ==========================================
        // GET ALL PAYROLL
        // ==========================================
        public async Task<IEnumerable<Payroll>> GetAllPayrollAsync()
        {
            return await _payrollRepository.GetAllAsync();
        }


        // ==========================================
        // GET PAYROLL BY ID
        // ==========================================
        public async Task<Payroll?> GetPayrollByIdAsync(int id)
        {
            return await _payrollRepository.GetByIdAsync(id);
        }


        // ==========================================
        // GET ACTIVE EMPLOYEES
        // ==========================================
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employees =
                await _payrollRepository.GetEmployeesAsync();

            return employees.ToList();
        }


        // ==========================================
        // ADD PAYROLL
        // ==========================================
        public async Task AddPayrollAsync(Payroll payroll)
        {
            payroll.PayrollMonth =
                ValidateMonth(payroll.PayrollMonth);

            ValidatePayroll(payroll);

            // Check duplicate payroll
            bool exists =
                await _payrollRepository.ExistsAsync(
                    payroll.EmployeeId,
                    payroll.PayrollMonth,
                    payroll.PayrollYear);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Payroll already exists for this employee for the selected month and year.");
            }

            // Calculate net salary
            payroll.NetSalary =
                payroll.BasicSalary +
                payroll.Allowances -
                payroll.Deductions;

            payroll.PaymentStatus = "Pending";
            payroll.PaymentDate = null;
            payroll.CreatedDate = DateTime.Now;

            await _payrollRepository.AddAsync(payroll);

            await _payrollRepository.SaveAsync();
        }


        // ==========================================
        // UPDATE PAYROLL
        // ==========================================
        public async Task<bool> UpdatePayrollAsync(
            Payroll payroll)
        {
            var existing =
                await _payrollRepository
                    .GetByIdAsync(payroll.PayrollId);

            if (existing == null)
                return false;

            // Paid payroll should not be edited
            if (existing.PaymentStatus == "Paid")
            {
                throw new InvalidOperationException(
                    "Paid payroll cannot be edited.");
            }

            payroll.PayrollMonth =
                ValidateMonth(payroll.PayrollMonth);

            ValidatePayroll(payroll);

            // Check duplicate
            bool exists =
                await _payrollRepository.ExistsAsync(
                    payroll.EmployeeId,
                    payroll.PayrollMonth,
                    payroll.PayrollYear,
                    payroll.PayrollId);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Another payroll already exists for this employee for the selected month and year.");
            }

            existing.EmployeeId =
                payroll.EmployeeId;

            existing.PayrollMonth =
                payroll.PayrollMonth;

            existing.PayrollYear =
                payroll.PayrollYear;

            existing.BasicSalary =
                payroll.BasicSalary;

            existing.Allowances =
                payroll.Allowances;

            existing.Deductions =
                payroll.Deductions;

            existing.NetSalary =
                payroll.BasicSalary +
                payroll.Allowances -
                payroll.Deductions;

            existing.Remarks =
                payroll.Remarks;

            await _payrollRepository
                .UpdateAsync(existing);

            await _payrollRepository
                .SaveAsync();

            return true;
        }


        // ==========================================
        // DELETE PAYROLL
        // ==========================================
        public async Task<bool> DeletePayrollAsync(int id)
        {
            var existing =
                await _payrollRepository.GetByIdAsync(id);

            if (existing == null)
                return false;

            if (existing.PaymentStatus == "Paid")
            {
                throw new InvalidOperationException(
                    "Paid payroll cannot be deleted.");
            }

            await _payrollRepository.DeleteAsync(id);

            await _payrollRepository.SaveAsync();

            return true;
        }


        // ==========================================
        // MARK AS PAID
        // ==========================================
        public async Task<bool> MarkAsPaidAsync(int id)
        {
            var payroll =
                await _payrollRepository.GetByIdAsync(id);

            if (payroll == null)
                return false;

            if (payroll.PaymentStatus == "Paid")
            {
                throw new InvalidOperationException(
                    "Payroll is already marked as paid.");
            }

            payroll.PaymentStatus = "Paid";
            payroll.PaymentDate = DateTime.Now;

            await _payrollRepository
                .UpdateAsync(payroll);

            await _payrollRepository
                .SaveAsync();

            return true;
        }


        // ==========================================
        // VALIDATE MONTH
        // ==========================================
        private int ValidateMonth(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new InvalidOperationException(
                    "Payroll month must be between 1 and 12.");
            }

            return month;
        }


        // ==========================================
        // VALIDATE PAYROLL
        // ==========================================
        private void ValidatePayroll(Payroll payroll)
        {
            if (payroll.EmployeeId <= 0)
            {
                throw new InvalidOperationException(
                    "Please select an employee.");
            }

            if (payroll.PayrollYear < 2000 ||
                payroll.PayrollYear > 2100)
            {
                throw new InvalidOperationException(
                    "Invalid payroll year.");
            }

            if (payroll.BasicSalary < 0)
            {
                throw new InvalidOperationException(
                    "Basic salary cannot be negative.");
            }

            if (payroll.Allowances < 0)
            {
                throw new InvalidOperationException(
                    "Allowances cannot be negative.");
            }

            if (payroll.Deductions < 0)
            {
                throw new InvalidOperationException(
                    "Deductions cannot be negative.");
            }

            decimal netSalary =
                payroll.BasicSalary +
                payroll.Allowances -
                payroll.Deductions;

            if (netSalary < 0)
            {
                throw new InvalidOperationException(
                    "Deductions cannot be greater than total salary.");
            }
        }
    }
}