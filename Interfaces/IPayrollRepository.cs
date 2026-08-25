using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface IPayrollRepository
    {
        // ==========================================
        // GET ALL PAYROLL
        // ==========================================
        Task<IEnumerable<Payroll>> GetAllAsync();


        // ==========================================
        // GET PAYROLL BY ID
        // ==========================================
        Task<Payroll?> GetByIdAsync(int id);


        // ==========================================
        // GET ACTIVE EMPLOYEES
        // ==========================================
        Task<IEnumerable<Employee>> GetEmployeesAsync();


        // ==========================================
        // CHECK DUPLICATE PAYROLL
        // ==========================================
        Task<bool> ExistsAsync(
            int employeeId,
            int month,
            int year,
            int? excludePayrollId = null);


        // ==========================================
        // ADD
        // ==========================================
        Task AddAsync(Payroll payroll);


        // ==========================================
        // UPDATE
        // ==========================================
        Task UpdateAsync(Payroll payroll);


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