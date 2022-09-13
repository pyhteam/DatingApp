﻿using System.Net;
using System.Security.Claims;
using API.Controllers.Base;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
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
        [Route("get-by-username/{userName}", Name = "GetUser")]
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

        // add photo
        [HttpPost]
        [Route("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                AppUserId = user.Id,
                IsMain = false,
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            if (await _photoService.SavePhotoAsync(photo) > 0)
            {
                return CreatedAtRoute("GetUser", new { userName = user.Username }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
        }
        [HttpPut]
        [Route("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
            {
                currentMain.IsMain = false;
                await _photoService.Update(currentMain.Id, currentMain);
            }
            photo.IsMain = true;
            int result = await _photoService.Update(photo.Id, photo);
            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete]
        [Route("delete-photo/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == id);
            if (photo == null)
            {
                return NotFound();
            }
            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            bool resultDelete = await _photoService.Delete(photo.Id);
            if (resultDelete)
            {
                return NoContent();
            }
            return BadRequest("Failed to delete the photo");
        }
    }
}
