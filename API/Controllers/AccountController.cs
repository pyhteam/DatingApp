using System.Net;
using System.Security.Cryptography;
using System.Text;
using API.Controllers.Base;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController : ApiBaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is exists");
            }
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();
            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            _context.Users.Add(user);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(new UserDto
                {
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                });
            }
            return BadRequest("Failed to register user");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
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
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}