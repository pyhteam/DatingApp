using System.Net;
using System.Security.Cryptography;
using System.Text;
using API.Controllers.Base;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController : ApiBaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using var hmac = new HMACSHA512();

            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username already exists");
            }
            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (user == null)
            {
                return BadRequest("Password or username is incorrect");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized();
                }
            }
            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}