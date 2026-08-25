using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Leave> Leaves { get; set; }

        public DbSet<Payroll> Payrolls { get; set; }


        // ==========================================
        // DATABASE CONFIGURATION
        // ==========================================
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // UNIQUE ATTENDANCE
            // ==========================================

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new
                {
                    a.EmployeeId,
                    a.AttendanceDate
                })
                .IsUnique();


            // ==========================================
            // PAYROLL DECIMAL PRECISION
            // ==========================================

            modelBuilder.Entity<Payroll>()
                .Property(p => p.BasicSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.Allowances)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.Deductions)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.NetSalary)
                .HasPrecision(18, 2);
        }
    }
}