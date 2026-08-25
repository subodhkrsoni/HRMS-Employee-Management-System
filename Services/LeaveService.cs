using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;

        public LeaveService(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        // =====================================================
        // GET ALL LEAVES
        // =====================================================
        public async Task<IEnumerable<Leave>> GetAllLeavesAsync()
        {
            return await _leaveRepository.GetAllAsync();
        }


        // =====================================================
        // GET LEAVE BY ID
        // =====================================================
        public async Task<Leave?> GetLeaveByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _leaveRepository.GetByIdAsync(id);
        }


        // =====================================================
        // GET EMPLOYEES
        // =====================================================
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employees =
                await _leaveRepository.GetEmployeesAsync();

            return employees.ToList();
        }


        // =====================================================
        // ADD LEAVE
        // =====================================================
        public async Task<bool> AddLeaveAsync(Leave leave)
        {
            leave.StartDate = leave.StartDate.Date;
            leave.EndDate = leave.EndDate.Date;

            await ValidateLeaveAsync(leave);

            leave.Status = "Pending";

            leave.ActionBy = null;
            leave.ActionDate = null;
            leave.ApprovedDate = null;
            leave.ApprovalComments = null;

            leave.CreatedDate = DateTime.Now;

            await _leaveRepository.AddAsync(leave);

            await _leaveRepository.SaveAsync();

            return true;
        }
        // =====================================================
        // UPDATE LEAVE
        // =====================================================
        public async Task<bool> UpdateLeaveAsync(Leave leave)
        {
            var existing =
                await _leaveRepository
                    .GetByIdAsync(leave.LeaveId);

            if (existing == null)
                return false;


            // Only Pending leave can be edited
            if (existing.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending leave can be edited.");
            }


            // Normalize dates
            leave.StartDate = leave.StartDate.Date;
            leave.EndDate = leave.EndDate.Date;


            // Validate
            await ValidateLeaveAsync(
                leave,
                leave.LeaveId);


            // Update only editable fields
            existing.EmployeeId =
                leave.EmployeeId;

            existing.LeaveType =
                leave.LeaveType;

            existing.StartDate =
                leave.StartDate;

            existing.EndDate =
                leave.EndDate;

            existing.Reason =
                leave.Reason;


            await _leaveRepository.UpdateAsync(existing);

            await _leaveRepository.SaveAsync();

            return true;
        }


        // =====================================================
        // DELETE / CANCEL LEAVE
        // =====================================================
        public async Task<bool> DeleteLeaveAsync(int id)
        {
            var existing =
                await _leaveRepository
                    .GetByIdAsync(id);

            if (existing == null)
                return false;


            // Only Pending leave can be cancelled
            if (existing.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending leave can be cancelled.");
            }


            await _leaveRepository.DeleteAsync(id);

            await _leaveRepository.SaveAsync();

            return true;
        }


        // =====================================================
        // APPROVE LEAVE
        // =====================================================
        public async Task<bool> ApproveLeaveAsync(
            int id,
            int approvedBy,
            string? comments)
        {
            if (approvedBy <= 0)
            {
                throw new InvalidOperationException(
                    "Invalid approver.");
            }


            var leave =
                await _leaveRepository
                    .GetByIdAsync(id);

            if (leave == null)
                return false;


            // Only Pending can be approved
            if (leave.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending leave can be approved.");
            }


            leave.Status = "Approved";

            leave.ActionBy = approvedBy;

            leave.ActionDate = DateTime.Now;

            leave.ApprovedDate = DateTime.Now;

            leave.ApprovalComments =
                string.IsNullOrWhiteSpace(comments)
                    ? null
                    : comments.Trim();


            await _leaveRepository.UpdateAsync(leave);

            await _leaveRepository.SaveAsync();

            return true;
        }


        // =====================================================
        // REJECT LEAVE
        // =====================================================
        public async Task<bool> RejectLeaveAsync(
            int id,
            int rejectedBy,
            string? comments)
        {
            if (rejectedBy <= 0)
            {
                throw new InvalidOperationException(
                    "Invalid user.");
            }


            // Rejection reason required
            if (string.IsNullOrWhiteSpace(comments))
            {
                throw new InvalidOperationException(
                    "Rejection reason is required.");
            }


            var leave =
                await _leaveRepository
                    .GetByIdAsync(id);

            if (leave == null)
                return false;


            // Only Pending can be rejected
            if (leave.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending leave can be rejected.");
            }


            leave.Status = "Rejected";

            leave.ActionBy = rejectedBy;

            leave.ActionDate = DateTime.Now;

            leave.ApprovedDate = DateTime.Now;

            leave.ApprovalComments =
                comments.Trim();


            await _leaveRepository.UpdateAsync(leave);

            await _leaveRepository.SaveAsync();

            return true;
        }


        // =====================================================
        // VALIDATE LEAVE
        // =====================================================
        private async Task ValidateLeaveAsync(
            Leave leave,
            int? currentLeaveId = null)
        {
            // -------------------------------------------------
            // Employee validation
            // -------------------------------------------------

            if (leave.EmployeeId <= 0)
            {
                throw new InvalidOperationException(
                    "Please select an employee.");
            }


            var employees =
                await _leaveRepository
                    .GetEmployeesAsync();

            bool employeeExists =
                employees.Any(e =>
                    e.EmployeeId ==
                    leave.EmployeeId &&
                    e.IsActive);

            if (!employeeExists)
            {
                throw new InvalidOperationException(
                    "Selected employee does not exist or is inactive.");
            }


            // -------------------------------------------------
            // Leave Type validation
            // -------------------------------------------------

            string[] validLeaveTypes =
            {
                "Casual Leave",
                "Sick Leave",
                "Earned Leave",
                "Unpaid Leave",
                "Maternity Leave",
                "Paternity Leave"
            };


            if (!validLeaveTypes.Contains(
                    leave.LeaveType))
            {
                throw new InvalidOperationException(
                    "Invalid leave type.");
            }


            // -------------------------------------------------
            // Date validation
            // -------------------------------------------------

            if (leave.StartDate.Date <
                DateTime.Today)
            {
                throw new InvalidOperationException(
                    "Leave start date cannot be in the past.");
            }


            if (leave.EndDate.Date <
                leave.StartDate.Date)
            {
                throw new InvalidOperationException(
                    "End date cannot be earlier than start date.");
            }


            // -------------------------------------------------
            // Reason validation
            // -------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                    leave.Reason))
            {
                throw new InvalidOperationException(
                    "Leave reason is required.");
            }


            if (leave.Reason.Length > 500)
            {
                throw new InvalidOperationException(
                    "Leave reason cannot exceed 500 characters.");
            }


            // -------------------------------------------------
            // Overlapping leave validation
            // -------------------------------------------------

            bool overlappingLeave =
             await _leaveRepository.ExistsOverlapAsync(
        leave.EmployeeId,
        leave.StartDate.Date,
        leave.EndDate.Date,
        currentLeaveId);

            if (overlappingLeave)
            {
                throw new InvalidOperationException(
                    "Employee already has a leave application for one or more selected dates.");
            }
        }
    }
}