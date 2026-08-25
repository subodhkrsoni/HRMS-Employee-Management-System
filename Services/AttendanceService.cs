using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(
            IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        // ==========================================
        // GET ALL
        // ==========================================
        public async Task<IEnumerable<Attendance>> GetAllAttendanceAsync()
        {
            return await _attendanceRepository.GetAllAsync();
        }

        // ==========================================
        // GET BY ID
        // ==========================================
        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _attendanceRepository.GetByIdAsync(id);
        }

        // ==========================================
        // GET EMPLOYEES
        // ==========================================
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employees =
                await _attendanceRepository.GetEmployeesAsync();

            return employees.ToList();
        }

        // ==========================================
        // ADD
        // ==========================================
        public async Task AddAttendanceAsync(
            Attendance attendance)
        {
            // Normalize date
            attendance.AttendanceDate =
                attendance.AttendanceDate.Date;

            // Validate attendance
            ValidateAttendance(attendance);

            // Check duplicate attendance
            bool duplicate =
                await _attendanceRepository.ExistsAsync(
                    attendance.EmployeeId,
                    attendance.AttendanceDate);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Attendance already exists for this employee on this date.");
            }

            attendance.CreatedDate = DateTime.Now;

            await _attendanceRepository.AddAsync(attendance);

            await _attendanceRepository.SaveAsync();
        }

        // ==========================================
        // UPDATE
        // ==========================================
        public async Task<bool> UpdateAttendanceAsync(
            Attendance attendance)
        {
            var existing =
                await _attendanceRepository
                    .GetByIdAsync(attendance.AttendanceId);

            if (existing == null)
                return false;

            // Normalize date
            attendance.AttendanceDate =
                attendance.AttendanceDate.Date;

            // Validate attendance
            ValidateAttendance(attendance);

            // Check duplicate attendance
            bool duplicate =
                await _attendanceRepository.ExistsAsync(
                    attendance.EmployeeId,
                    attendance.AttendanceDate,
                    attendance.AttendanceId);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Another attendance record already exists for this employee on this date.");
            }

            // Update fields
            existing.EmployeeId =
                attendance.EmployeeId;

            existing.AttendanceDate =
                attendance.AttendanceDate;

            existing.Status =
                attendance.Status;

            existing.CheckIn =
                attendance.CheckIn;

            existing.CheckOut =
                attendance.CheckOut;

            existing.Remarks =
                attendance.Remarks;

            await _attendanceRepository.UpdateAsync(existing);

            await _attendanceRepository.SaveAsync();

            return true;
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var existing =
                await _attendanceRepository
                    .GetByIdAsync(id);

            if (existing == null)
                return false;

            await _attendanceRepository.DeleteAsync(id);

            await _attendanceRepository.SaveAsync();

            return true;
        }

        // ==========================================
        // ATTENDANCE VALIDATION
        // ==========================================
        private void ValidateAttendance(
            Attendance attendance)
        {
            // ------------------------------------------
            // Status validation
            // ------------------------------------------

            string[] validStatuses =
            {
                "Present",
                "Absent",
                "Late",
                "Half Day"
            };

            if (!validStatuses.Contains(attendance.Status))
            {
                throw new InvalidOperationException(
                    "Invalid attendance status.");
            }

            // ------------------------------------------
            // Employee validation
            // ------------------------------------------

            if (attendance.EmployeeId <= 0)
            {
                throw new InvalidOperationException(
                    "Please select an employee.");
            }

            // ------------------------------------------
            // Absent validation
            // ------------------------------------------

            if (attendance.Status == "Absent")
            {
                attendance.CheckIn = null;
                attendance.CheckOut = null;
            }

            // ------------------------------------------
            // Check-In / Check-Out validation
            // ------------------------------------------

            if (attendance.CheckIn.HasValue &&
                attendance.CheckOut.HasValue)
            {
                if (attendance.CheckOut.Value <
                    attendance.CheckIn.Value)
                {
                    throw new InvalidOperationException(
                        "Check-out time cannot be earlier than check-in time.");
                }
            }

            // ------------------------------------------
            // Future date validation
            // ------------------------------------------

            if (attendance.AttendanceDate.Date >
                DateTime.Today)
            {
                throw new InvalidOperationException(
                    "Attendance date cannot be in the future.");
            }
        }
    }
}