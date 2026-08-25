using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Interfaces;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repository;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// MVC
// ==========================================
builder.Services.AddControllersWithViews();

// ==========================================
// Database
// ==========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// Repository
// ==========================================
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Attendance Repository
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();

// ==========================================
// Services
// ==========================================
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();


// Password Hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ==========================================
// Cookie Authentication
// ==========================================
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

// ==========================================
// Authorization
// ==========================================
builder.Services.AddAuthorization();

var app = builder.Build();

// ==========================================
// Create / Update Default Admin
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher<User>>();

    var admin = context.Users
        .FirstOrDefault(u => u.Username == "admin");

    if (admin == null)
    {
        admin = new User
        {
            FullName = "System Administrator",
            Username = "admin",
            Role = "Admin",
            IsActive = true
        };

        admin.PasswordHash =
            passwordHasher.HashPassword(admin, "admin123");

        context.Users.Add(admin);
    }
    else
    {
        admin.FullName = "System Administrator";
        admin.Role = "Admin";
        admin.IsActive = true;

        admin.PasswordHash =
            passwordHasher.HashPassword(admin, "admin123");
    }

    context.SaveChanges();
}

// ==========================================
// Middleware
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Authentication MUST come before Authorization
app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// Default Route
// ==========================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();