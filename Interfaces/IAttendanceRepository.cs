using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface IAttendanceRepository
    {
        // Get all attendance records
        Task<IEnumerable<Attendance>> GetAllAsync();

        // Get attendance by ID
        Task<Attendance?> GetByIdAsync(int id);

        // Get all employees
        Task<IEnumerable<Employee>> GetEmployeesAsync();

        // Check duplicate attendance
        Task<bool> ExistsAsync(
            int employeeId,
            DateTime attendanceDate,
            int? excludeAttendanceId = null);

        // Add
        Task AddAsync(Attendance attendance);

        // Update
        Task UpdateAsync(Attendance attendance);

        // Delete
        Task DeleteAsync(int id);

        // Save
        Task SaveAsync();
    }
}