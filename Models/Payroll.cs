using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        // ==========================================
        // EMPLOYEE
        // ==========================================

        [Required]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }


        // ==========================================
        // PAYROLL PERIOD
        // ==========================================

        [Required]
        public int PayrollMonth { get; set; }

        [Required]
        public int PayrollYear { get; set; }


        // ==========================================
        // SALARY
        // ==========================================

        [Range(0, double.MaxValue)]
        public decimal BasicSalary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Allowances { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Deductions { get; set; }


        // ==========================================
        // NET SALARY
        // ==========================================

        public decimal NetSalary { get; set; }


        // ==========================================
        // PAYMENT STATUS
        // ==========================================

        [Required]
        [StringLength(30)]
        public string PaymentStatus { get; set; } = "Pending";


        // ==========================================
        // PAYMENT DATE
        // ==========================================

        public DateTime? PaymentDate { get; set; }


        // ==========================================
        // REMARKS
        // ==========================================

        [StringLength(500)]
        public string? Remarks { get; set; }


        // ==========================================
        // CREATED DATE
        // ==========================================

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}