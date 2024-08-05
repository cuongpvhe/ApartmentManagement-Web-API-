using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ApartmentManagement.Helpers;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Authenticate([FromBody] Manager managerObj)
        {
            if (managerObj == null)
                return BadRequest();
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.Username == managerObj.Username);
            if (manager == null)
                return NotFound(new {Message = " Use Not Found!"}); 

            if(!PasswordHasher.VerifyPassword(managerObj.Password, manager.Password))
            {
                return BadRequest(new {Message = "Password is Incorrect"});
            }

            return Ok(new
            {
                Message = " Login Success!"
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Manager managerObj)
        {
            if(managerObj == null)
                return BadRequest();
            //Check username
            if(await CheckUserNameExistAsync(managerObj.Username))
                return BadRequest(new { Message = "Username Already Exist!"});

            //Check email
            if (await CheckEmailExistAsync(managerObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            //Check password Strength
            var pass = CheckPasswordStrength(managerObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new {Message = pass.ToString()}); 

            managerObj.Password = PasswordHasher.HashPassword(managerObj.Password);
            managerObj.Token = "";
            await _context.Managers.AddAsync(managerObj);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });
        }

        private Task<bool> CheckUserNameExistAsync(string username)
        => _context.Managers.AnyAsync(x => x.Username == username);

        private Task<bool> CheckEmailExistAsync(string email)
        => _context.Managers.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
               sb.Append("Minium password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]"))) sb.Append("Password should be Alphanumeric" + Environment.NewLine);  

            if(!Regex.IsMatch(password, "[!, @, #, $, %, ^, &, *, (, ), -, _, =, +, {, }, \\[, \\], |, \\, :, ;, \", ', <, >, ,, ., ?, /, ~, `]"))
                sb.Append("Password should contain special chars" + Environment.NewLine) ;
            return sb.ToString();
        }
    }
} 
