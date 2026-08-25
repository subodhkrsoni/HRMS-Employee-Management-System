using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Repository
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly AppDbContext _context;

        public PayrollRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET ALL PAYROLL
        // ==========================================
        public async Task<IEnumerable<Payroll>> GetAllAsync()
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .OrderByDescending(p => p.PayrollYear)
                .ThenByDescending(p => p.PayrollMonth)
                .ToListAsync();
        }


        // ==========================================
        // GET PAYROLL BY ID
        // ==========================================
        public async Task<Payroll?> GetByIdAsync(int id)
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(
                    p => p.PayrollId == id);
        }


        // ==========================================
        // GET ACTIVE EMPLOYEES
        // ==========================================
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }


        // ==========================================
        // CHECK DUPLICATE PAYROLL
        // ==========================================
        public async Task<bool> ExistsAsync(
            int employeeId,
            int month,
            int year,
            int? excludePayrollId = null)
        {
            var query = _context.Payrolls
                .Where(p =>
                    p.EmployeeId == employeeId &&
                    p.PayrollMonth == month &&
                    p.PayrollYear == year);

            // Ignore current record during edit
            if (excludePayrollId.HasValue)
            {
                query = query.Where(p =>
                    p.PayrollId != excludePayrollId.Value);
            }

            return await query.AnyAsync();
        }


        // ==========================================
        // ADD
        // ==========================================
        public async Task AddAsync(Payroll payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
        }


        // ==========================================
        // UPDATE
        // ==========================================
        public Task UpdateAsync(Payroll payroll)
        {
            _context.Payrolls.Update(payroll);

            return Task.CompletedTask;
        }


        // ==========================================
        // DELETE
        // ==========================================
        public async Task DeleteAsync(int id)
        {
            var payroll =
                await _context.Payrolls.FindAsync(id);

            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
            }
        }


        // ==========================================
        // SAVE
        // ==========================================
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}