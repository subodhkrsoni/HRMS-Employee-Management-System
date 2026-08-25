using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Designation { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? PhotoPath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
         = new List<Attendance>();
    }
}