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
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.Username == managerObj.Username && x.Password == managerObj.Password);
            if (manager == null)
                return NotFound(new {Message = " Use Not Found!"}); 
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
            await _context.Managers.AddAsync(managerObj);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });
        }
    }
} 
