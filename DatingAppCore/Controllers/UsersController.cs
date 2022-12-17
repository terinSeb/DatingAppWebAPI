using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppCore.Data;
using DatingAppCore.DTOs;
using DatingAppCore.Entities;
using DatingAppCore.Extensions;
using DatingAppCore.Helpers;
using DatingAppCore.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppCore.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository,IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]        
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams )
        {
            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPage));
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]        
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);            
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            //In the below Code memberUpdateDto Content is copied to user Object.
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            else return BadRequest("Failed to Update User");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var result = await _photoService.AddPhtoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var Photo = new Photo
            {
                Url  = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count == 0)
            {
                Photo.IsMain = true;
            }

            user.Photos.Add(Photo);

            if (await _userRepository.SaveAllAsync())
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(Photo));

            return BadRequest("Problem Adding Photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMain) return BadRequest("This is already your main Photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to set Main Photo");
        }
        [HttpDelete("delete-photo/{PhotoId}")]
        public async Task<ActionResult> DeletePhoto(int PhotoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == PhotoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your Main Photo.");
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to Delete");
        }
    }
}