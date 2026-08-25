using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Interfaces
{
    public interface IPayrollService
    {
        // ==========================================
        // GET ALL PAYROLL
        // ==========================================
        Task<IEnumerable<Payroll>> GetAllPayrollAsync();


        // ==========================================
        // GET PAYROLL BY ID
        // ==========================================
        Task<Payroll?> GetPayrollByIdAsync(int id);


        // ==========================================
        // GET ACTIVE EMPLOYEES
        // ==========================================
        Task<List<Employee>> GetEmployeesAsync();


        // ==========================================
        // ADD PAYROLL
        // ==========================================
        Task AddPayrollAsync(Payroll payroll);


        // ==========================================
        // UPDATE PAYROLL
        // ==========================================
        Task<bool> UpdatePayrollAsync(Payroll payroll);


        // ==========================================
        // DELETE PAYROLL
        // ==========================================
        Task<bool> DeletePayrollAsync(int id);


        // ==========================================
        // MARK AS PAID
        // ==========================================
        Task<bool> MarkAsPaidAsync(int id);
    }
}