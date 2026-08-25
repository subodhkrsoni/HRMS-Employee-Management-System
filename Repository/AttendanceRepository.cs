using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET ALL
        // ==========================================
        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AttendanceDate)
                .ThenBy(a => a.Employee.Name)
                .ToListAsync();
        }

        // ==========================================
        // GET BY ID
        // ==========================================
        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(
                    a => a.AttendanceId == id);
        }

        // ==========================================
        // GET EMPLOYEES
        // ==========================================
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        // ==========================================
        // CHECK DUPLICATE
        // ==========================================
        public async Task<bool> ExistsAsync(
            int employeeId,
            DateTime attendanceDate,
            int? excludeAttendanceId = null)
        {
            DateTime date =
                attendanceDate.Date;

            var query = _context.Attendances
                .AsQueryable();

            query = query.Where(a =>
                a.EmployeeId == employeeId &&
                a.AttendanceDate.Date == date);

            if (excludeAttendanceId.HasValue)
            {
                query = query.Where(a =>
                    a.AttendanceId !=
                    excludeAttendanceId.Value);
            }

            return await query.AnyAsync();
        }

        // ==========================================
        // ADD
        // ==========================================
        public async Task AddAsync(
            Attendance attendance)
        {
            await _context.Attendances
                .AddAsync(attendance);
        }

        // ==========================================
        // UPDATE
        // ==========================================
        public Task UpdateAsync(
            Attendance attendance)
        {
            _context.Attendances.Update(attendance);

            return Task.CompletedTask;
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task DeleteAsync(int id)
        {
            var attendance =
                await _context.Attendances
                    .FindAsync(id);

            if (attendance != null)
            {
                _context.Attendances
                    .Remove(attendance);
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