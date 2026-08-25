using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface ILeaveRepository
    {
        // ==========================================
        // GET ALL LEAVES
        // ==========================================
        Task<IEnumerable<Leave>> GetAllAsync();

        // ==========================================
        // GET LEAVE BY ID
        // ==========================================
        Task<Leave?> GetByIdAsync(int id);

        // ==========================================
        // GET EMPLOYEES
        // ==========================================
        Task<IEnumerable<Employee>> GetEmployeesAsync();

        // ==========================================
        // CHECK OVERLAPPING LEAVE
        // ==========================================
        Task<bool> ExistsOverlapAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate,
            int? excludeLeaveId = null);

        // ==========================================
        // ADD
        // ==========================================
        Task AddAsync(Leave leave);

        // ==========================================
        // UPDATE
        // ==========================================
        Task UpdateAsync(Leave leave);

        // ==========================================
        // DELETE
        // ==========================================
        Task DeleteAsync(int id);

        // ==========================================
        // SAVE
        // ==========================================
        Task SaveAsync();
    }
}