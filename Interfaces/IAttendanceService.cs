using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendanceAsync();

        Task<Attendance?> GetAttendanceByIdAsync(int id);

        Task<List<Employee>> GetEmployeesAsync();

        Task AddAttendanceAsync(Attendance attendance);

        Task<bool> UpdateAttendanceAsync(Attendance attendance);

        Task<bool> DeleteAttendanceAsync(int id);
    }
}