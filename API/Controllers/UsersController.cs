using System.Net;
using System.Security.Claims;
using API.Controllers.Base;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Authorize]
    public class UsersController : ApiBaseController
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Route("get-all")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get()
        {
            var users = await _userRepository.GetUsersAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usersToReturn);
        }

        // GET api/<UsersController>/5

        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<MemberDto>> Get(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        // get by username
        [HttpGet]
        [Route("get-by-username/{userName}")]
        public async Task<ActionResult<MemberDto>> Get(string userName)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult> Put(MemberUpdateDto memberUpdateDto)
        {
            string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            user.City = memberUpdateDto.City;
            user.Country = memberUpdateDto.Country;
            user.Interests = memberUpdateDto.Interests;
            user.Introduction = memberUpdateDto.Introduction;
            user.LookingFor = memberUpdateDto.LookingFor;

            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
