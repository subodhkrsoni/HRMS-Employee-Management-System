using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==========================
        // Get All Employees
        // ==========================
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .OrderByDescending(e => e.EmployeeId)
                .ToListAsync();
        }

        // ==========================
        // Get Employee By Id
        // ==========================
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        // ==========================
        // Add Employee
        // ==========================
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        // ==========================
        // Update Employee
        // ==========================
        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await Task.CompletedTask;
        }

        // ==========================
        // Delete Employee
        // ==========================
        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
        }

        // ==========================
        // Save Changes
        // ==========================
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}