using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Models;
using Microsoft.Build.Framework;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace ApartmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly ApartmentManagementContext _context;

        public ManagersController(ApartmentManagementContext context)
        {
            _context = context;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Manager manager)
        {
            if (manager == null)
            {
                return BadRequest(new { Message = "Yêu cầu không hợp lệ." });
            }

            try
            {
                var existingManager = await _context.Managers
                    .FirstOrDefaultAsync(x => x.Username == manager.Username && x.Password == manager.Password);

                if (existingManager == null)
                {
                    return NotFound(new { Message = "Tài khoản hoặc mật khẩu không đúng." });
                }

                return Ok(new
                {
                    Message = "Đăng nhập thành công!"
                });
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi để kiểm tra
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }




        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Manager manager)
        {
            if (manager == null)
                return BadRequest(new { Message = "Invalid data." });

            // Initialize a list to collect error messages
            var errors = new List<string>();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(manager.Username))
                errors.Add("Username is required.");

            if (string.IsNullOrWhiteSpace(manager.Password))
                errors.Add("Password is required.");

            if (string.IsNullOrWhiteSpace(manager.Email))
                errors.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(manager.Address))
                errors.Add("Address is required.");

            if (string.IsNullOrWhiteSpace(manager.FullName))
                errors.Add("FullName is required.");

            if (string.IsNullOrWhiteSpace(manager.PhoneNumber))
                errors.Add("PhoneNumber is required.");

            if (!string.IsNullOrWhiteSpace(manager.PhoneNumber))
            {
                // Check if phone number is exactly 10 digits
                if (!System.Text.RegularExpressions.Regex.IsMatch(manager.PhoneNumber, @"^\d{10}$"))
                {
                    errors.Add("Phone number must be exactly 10 digits.");
                }
            }

            // If there are validation errors, return them
            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // Check if the username already exists
            var existingUsername = await _context.Managers
                .AnyAsync(x => x.Username == manager.Username);

            if (existingUsername)
                errors.Add("Username already exists.");

            // Check if the email already exists
            var existingEmail = await _context.Managers
                .AnyAsync(x => x.Email == manager.Email);

            if (existingEmail)
                errors.Add("Email already exists.");

            // Check if the phone number already exists (if provided)
            if (!string.IsNullOrWhiteSpace(manager.PhoneNumber))
            {
                var existingPhoneNumber = await _context.Managers
                    .AnyAsync(x => x.PhoneNumber == manager.PhoneNumber);

                if (existingPhoneNumber)
                    errors.Add("Phone number already exists.");
            }

            

            // Check if the FullName already exists (if provided)
            if (!string.IsNullOrWhiteSpace(manager.FullName))
            {
                var existingCccd = await _context.Managers
                    .AnyAsync(x => x.FullName == manager.FullName);

                if (existingCccd)
                    errors.Add("FullName already exists.");
            }

            // If there are validation errors, return them
            if (errors.Any())
                return Conflict(new { Errors = errors });

            // Set the creation date
            manager.CreateDate = DateTime.UtcNow;

            // Add new manager
            await _context.Managers.AddAsync(manager);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful!" });
        }

    }
}