using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ==========================================
        // USER LIST
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(users);
        }

        // ==========================================
        // CREATE USER - GET
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new UserViewModel
            {
                Role = "Employee",
                IsActive = true
            };

            return View(vm);
        }

        // ==========================================
        // CREATE USER - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Check duplicate username
            bool usernameExists = await _context.Users
                .AnyAsync(u => u.Username == vm.Username);

            if (usernameExists)
            {
                ModelState.AddModelError(
                    nameof(vm.Username),
                    "Username already exists.");

                return View(vm);
            }

            var user = new User
            {
                FullName = vm.FullName,
                Username = vm.Username,
                Role = vm.Role,
                IsActive = vm.IsActive
            };

            // Hash password
            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    vm.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDIT USER - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .FindAsync(id.Value);

            if (user == null)
                return NotFound();

            var vm = new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return View(vm);
        }

        // ==========================================
        // EDIT USER - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UserViewModel vm)
        {
            if (id != vm.UserId)
                return NotFound();

            // Password is optional during edit
            ModelState.Remove(nameof(vm.Password));

            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
                return NotFound();

            // Check duplicate username
            bool usernameExists = await _context.Users
                .AnyAsync(u =>
                    u.Username == vm.Username &&
                    u.UserId != id);

            if (usernameExists)
            {
                ModelState.AddModelError(
                    nameof(vm.Username),
                    "Username already exists.");

                return View(vm);
            }

            // Prevent changing the last Admin
            if (user.Role == "Admin" &&
                vm.Role != "Admin")
            {
                int adminCount = await _context.Users
                    .CountAsync(u =>
                        u.Role == "Admin" &&
                        u.IsActive);

                if (adminCount <= 1)
                {
                    ModelState.AddModelError(
                        nameof(vm.Role),
                        "At least one active Admin must exist.");

                    return View(vm);
                }
            }

            // Prevent deactivating the last Admin
            if (user.Role == "Admin" &&
                user.IsActive &&
                !vm.IsActive)
            {
                int activeAdminCount = await _context.Users
                    .CountAsync(u =>
                        u.Role == "Admin" &&
                        u.IsActive);

                if (activeAdminCount <= 1)
                {
                    ModelState.AddModelError(
                        nameof(vm.IsActive),
                        "The last active Admin cannot be deactivated.");

                    return View(vm);
                }
            }

            user.FullName = vm.FullName;
            user.Username = vm.Username;
            user.Role = vm.Role;
            user.IsActive = vm.IsActive;

            // Update password only if entered
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(
                        user,
                        vm.Password);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ACTIVATE / DEACTIVATE USER
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
                return NotFound();

            var currentUserId =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;

            // Prevent self deactivation
            if (currentUserId == id.ToString())
            {
                TempData["Error"] =
                    "You cannot deactivate your own account.";

                return RedirectToAction(nameof(Index));
            }

            // Prevent deactivating last Admin
            if (user.Role == "Admin" && user.IsActive)
            {
                int activeAdminCount = await _context.Users
                    .CountAsync(u =>
                        u.Role == "Admin" &&
                        u.IsActive);

                if (activeAdminCount <= 1)
                {
                    TempData["Error"] =
                        "The last active Admin cannot be deactivated.";

                    return RedirectToAction(nameof(Index));
                }
            }

            user.IsActive = !user.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                user.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // DELETE USER
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
                return NotFound();

            var currentUserId =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;

            // Prevent self deletion
            if (currentUserId == id.ToString())
            {
                TempData["Error"] =
                    "You cannot delete your own account.";

                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting last Admin
            if (user.Role == "Admin")
            {
                int adminCount = await _context.Users
                    .CountAsync(u => u.Role == "Admin");

                if (adminCount <= 1)
                {
                    TempData["Error"] =
                        "The last Admin cannot be deleted.";

                    return RedirectToAction(nameof(Index));
                }
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}