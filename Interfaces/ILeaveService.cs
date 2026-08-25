using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface ILeaveService
    {
        // ==========================================
        // GET ALL LEAVES
        // ==========================================
        Task<IEnumerable<Leave>> GetAllLeavesAsync();

        // ==========================================
        // GET LEAVE BY ID
        // ==========================================
        Task<Leave?> GetLeaveByIdAsync(int id);

        // ==========================================
        // GET EMPLOYEES
        // ==========================================
        Task<List<Employee>> GetEmployeesAsync();

        // ==========================================
        // APPLY / ADD LEAVE
        // ==========================================
        Task<bool> AddLeaveAsync(Leave leave);

        // ==========================================
        // UPDATE LEAVE
        // ==========================================
        Task<bool> UpdateLeaveAsync(Leave leave);

        // ==========================================
        // APPROVE LEAVE
        // ==========================================
        Task<bool> ApproveLeaveAsync(
            int leaveId,
            int approvedBy,
            string? comments);

        // ==========================================
        // REJECT LEAVE
        // ==========================================
        Task<bool> RejectLeaveAsync(
            int leaveId,
            int rejectedBy,
            string? comments);

        // ==========================================
        // DELETE / CANCEL LEAVE
        // ==========================================
        Task<bool> DeleteLeaveAsync(int id);
    }
}