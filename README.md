# HRMS - Human Resource Management System

A web-based Human Resource Management System built using ASP.NET Core MVC, Entity Framework Core and SQL Server.

The application provides a centralized platform for managing employees, departments, attendance, leave, payroll and reports.

## Features

- Employee Management
- Department Management
- Attendance Management
- Leave Management
- Payroll Management
- Payroll & Attendance Reports
- Dashboard with statistics
- User Management
- Authentication & Authorization
- Role-based access control
- Search, filtering and pagination
- CRUD operations

## Technologies

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor Views
- HTML5
- CSS3
- Bootstrap
- JavaScript
- LINQ
- Git & GitHub

## Modules

### Dashboard
- Total Employees
- Active Employees
- Departments
- Today's Attendance
- Leave Summary
- Payroll Summary
- Recent Attendance
- Recent Leaves
- Recent Payroll

### Employee Management
- Add Employee
- View Employee
- Edit Employee
- Delete Employee
- Active/Inactive Employee

### Attendance
- Mark Attendance
- Present
- Absent
- Late
- Half Day
- Check-in / Check-out
- Attendance Reports
- Employee, Month, Year and Status Filters

### Leave Management
- Apply Leave
- View Leave
- Approve Leave
- Reject Leave
- Leave Reports
- Employee and Status Filters

### Payroll
- Create Payroll
- Basic Salary
- Allowances
- Deductions
- Net Salary
- Pending / Paid Status
- Payment Date
- Payroll Reports

### Reports
- Payroll Report
- Attendance Report
- Leave Report
- Month and Year Filters
- Employee Filters
- Status Filters

## Authentication & Authorization

The application uses role-based authorization to protect HRMS functionality.

Example:

```csharp
[Authorize(Roles = "Admin,HR")]
public class ReportsController : Controller
{
}

## Architecture
User
  ↓
Razor Views
  ↓
Controllers
  ↓
Services / Repository
  ↓
Entity Framework Core
  ↓
SQL Server

Database

The application uses SQL Server with Entity Framework Core.

Main entities include:

Employee
Department
Attendance
Leave
Payroll
User
Role

How to Run

1. Clone the repository
git clone https://github.com/subodhkrsoni/HRMS-Employee-Management-System.git

2. Open the project

Open the solution in Visual Studio.

3. Configure SQL Server

Update the database connection string in:

appsettings.json
4. Apply migrations
dotnet ef database update
5. Run the application
dotnet run

Or run the project directly from Visual Studio.
Testing

The major modules have been tested for:

Create
View
Edit
Delete
Filtering
Authentication
Authorization
Attendance status
Leave approval/rejection
Payroll status
Reports

Future Enhancements
Payslip PDF generation
Excel report export
Email notifications
Employee self-service portal
Docker support
CI/CD pipeline
AWS deployment

Developer
Subodh Kumar Soni

HRMS - Human Resource Management System

### 3. Commit

Page ke bottom par:

**Commit changes**

Message:

```text
Add professional README
