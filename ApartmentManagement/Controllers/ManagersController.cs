using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Models;
using ApartmentManagement.Dto;
using ApartmentManagement.UtilityService;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ApartmentManagement.Helpers;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
namespace ApartmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly ApartmentManagementContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public ManagersController(ApartmentManagementContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Manager managerObj)
        {
            if (managerObj == null)
                return BadRequest();
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.Username == managerObj.Username);
            if (manager == null)
                return NotFound(new { Message = " Use Not Found!" });

            if (!PasswordHasher.VerifyPassword(managerObj.Password, manager.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            manager.Token = CreateJwt(manager);
            var newAccessToken = manager.Token;
            var newRefreshToken = CreateRefreshToken();
            manager.RefreshToken = newRefreshToken;
            manager.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _context.SaveChangesAsync();


            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Manager managerObj)
        {
            if (managerObj == null)
                return BadRequest();
            //Check username
            if (await CheckUserNameExistAsync(managerObj.Username))
                return BadRequest(new { Message = "Username Already Exist!" });

            //Check email
            if (await CheckEmailExistAsync(managerObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            //Check password Strength
            var pass = CheckPasswordStrength(managerObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            managerObj.Password = PasswordHasher.HashPassword(managerObj.Password);
            managerObj.Token = "";
            await _context.Managers.AddAsync(managerObj);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });
        }

        private Task<bool> CheckUserNameExistAsync(string? username)
        => _context.Managers.AnyAsync(x => x.Username == username);

        private Task<bool> CheckEmailExistAsync(string? email)
        => _context.Managers.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minium password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]"))) sb.Append("Password should be Alphanumeric" + Environment.NewLine);

            if (!Regex.IsMatch(password, "[!, @, #, $, %, ^, &, *, (, ), -, _, =, +, {, }, \\[, \\], |, \\, :, ;, \", ', <, >, ,, ., ?, /, ~, `]"))
                sb.Append("Password should contain special chars" + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwt(Manager manager)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecretkeythatneedstobelongenough!!");

            var identity = new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, manager.Role.ToString()), // Convert Role to string
        new Claim(ClaimTypes.Name, manager.FullName),
        new Claim("ManagerId", manager.ManagerId.ToString()) // Add other claims as needed
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(1), // Set token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInManager = _context.Managers
                .Any(m => m.RefreshToken == refreshToken);
            if (tokenInManager)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysecretkeythatneedstobelongenough!!");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken != null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))

                throw new SecurityTokenException("This is Invalid Token");
            return principal;
        }
        [HttpGet]
        public async Task<ActionResult<Manager>> GetAllManagers()
        {
            return Ok(await _context.Managers.ToListAsync());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto == null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Username == username);
            if (manager == null || manager.RefreshToken != refreshToken || manager.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(manager);
            var newRefreshToken = CreateRefreshToken();
            manager.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == email);
            if (manager == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "email Doesn't Exist"
                });


            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            manager.ResetPasswordToken = emailToken;
            manager.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _context.Entry(manager).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"

            });

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");
            var manager = await _context.Managers.AsNoTracking().FirstOrDefaultAsync(m => m.Email == resetPasswordDto.Email);
            if (manager == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Manager Doesn't Exist"
                });


            }
            var tokenCode = manager.ResetPasswordToken;
            DateTime emailTokenExprity = (DateTime)manager.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExprity < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid Reset link"
                });

            }

            manager.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _context.Entry(manager).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Statuscode = 200,
                Message = "Password Reset Successfully"
            });
        }
    }
}
