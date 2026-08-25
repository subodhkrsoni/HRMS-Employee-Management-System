using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET ALL LEAVES
        // ==========================================
        public async Task<IEnumerable<Leave>> GetAllAsync()
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();
        }

        // ==========================================
        // GET LEAVE BY ID
        // ==========================================
        public async Task<Leave?> GetByIdAsync(int id)
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(
                    l => l.LeaveId == id);
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
        // CHECK OVERLAPPING LEAVE
        // ==========================================
        public async Task<bool> ExistsOverlapAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate,
            int? excludeLeaveId = null)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            var query = _context.Leaves
                .Where(l =>
                    l.EmployeeId == employeeId &&

                    // Ignore rejected leaves
                    l.Status != "Rejected" &&

                    // Date overlap condition
                    startDate <= l.EndDate.Date &&
                    endDate >= l.StartDate.Date);

            // When editing, exclude current record
            if (excludeLeaveId.HasValue)
            {
                query = query.Where(l =>
                    l.LeaveId != excludeLeaveId.Value);
            }

            return await query.AnyAsync();
        }

        // ==========================================
        // ADD
        // ==========================================
        public async Task AddAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
        }

        // ==========================================
        // UPDATE
        // ==========================================
        public Task UpdateAsync(Leave leave)
        {
            _context.Leaves.Update(leave);

            return Task.CompletedTask;
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task DeleteAsync(int id)
        {
            var leave = await _context.Leaves
                .FindAsync(id);

            if (leave != null)
            {
                _context.Leaves.Remove(leave);
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