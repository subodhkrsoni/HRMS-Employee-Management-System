using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Leave
    {
        [Key]
        public int LeaveId { get; set; }

        // ==========================================
        // EMPLOYEE
        // ==========================================

        [Required]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }


        // ==========================================
        // LEAVE TYPE
        // ==========================================

        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty;


        // ==========================================
        // LEAVE DATES
        // ==========================================

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }


        // ==========================================
        // REASON
        // ==========================================

        [StringLength(500)]
        public string? Reason { get; set; }


        // ==========================================
        // STATUS
        // ==========================================

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";


        // ==========================================
        // APPROVAL / REJECTION ACTION
        // ==========================================

        public int? ActionBy { get; set; }

        public DateTime? ActionDate { get; set; }

        public DateTime? ApprovedDate { get; set; }


        // ==========================================
        // ADMIN / HR COMMENTS
        // ==========================================

        [StringLength(500)]
        public string? ApprovalComments { get; set; }


        // ==========================================
        // CREATED DATE
        // ==========================================

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}