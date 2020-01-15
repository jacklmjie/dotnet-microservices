using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Dtos;
using User.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetCore.CAP;
using User.API.Infrastructure.Exceptions;
using User.API.Infrastructure;
using User.API.Infrastructure.Services;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly IIdentityService _identityService;
        private readonly ICapPublisher _capPublisher;
        public UsersController(UserContext userContext,
            IIdentityService identityService,
            ICapPublisher capPublisher)
        {
            _userContext = userContext;
            _identityService = identityService;
            _capPublisher = capPublisher;
        }

        private void UserPatchChangedEvent(AppUser user)
        {
            if (_userContext.Entry(user).Property(nameof(user.Name)).IsModified)
            {
                var identity = new UserIdentityDTO()
                {
                    UserId = user.Id,
                    Name = user.Name
                };
                _capPublisher.Publish("user.api.user_patch_change_event", identity);
            }
        }

        [Route("")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            var userId = _identityService.GetUserIdentity();

            var user = await _userContext.Users
                .AsNoTracking()
                .Include(p => p.Properties)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserDomainException($"错误的用户上下文Id={userId}");

            return Ok(user);
        }

        [Route("identity/{userId}")]
        [HttpGet]
        public async Task<ActionResult> GetUserIdentity(int userId)
        {
            //todo:检查用户是否好友关系

            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var identity = new UserIdentityDTO()
            {
                UserId = user.Id,
                Name = user.Name
            };

            return Ok(identity);
        }

        [Route("")]
        [HttpPatch]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var userId = _identityService.GetUserIdentity();

            var user = await _userContext.Users
               .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserDomainException($"错误的用户上下文Id={userId}");

            patch.ApplyTo(user);

            using (var transaction = _userContext.Database.BeginTransaction())
            {
                UserPatchChangedEvent(user);
                _userContext.Users.Update(user);
                _userContext.SaveChanges();
                transaction.Commit();
            }

            return Ok(user);
        }

        /// <summary>
        /// 检查或者创建用户（当用户手机不存在时创建用户）
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<ActionResult> CheckOrCreate(AppUserCheckOrCreateDTO dto)
        {
            if (dto == null)
            {
                return NoContent();
            }
            //todo:做手机号码格式的验证
            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Phone == dto.phone);
            if (user == null)
            {
                user = new AppUser() { Phone = dto.phone };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }
            var userIdentity = new UserIdentityDTO
            {
                UserId = user.Id,
                Name = user.Name
            };
            return Ok(userIdentity);
        }

        /// <summary>
        /// 获取用户标签选项数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tags")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserTags()
        {
            var userId = _identityService.GetUserIdentity();

            var userTags = await _userContext.UserTages.Where(u => u.AppUserId == userId).ToListAsync();
            return Ok(userTags);
        }

        /// <summary>
        /// 根据手机号码查询用户资料
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult> Search(string phone)
        {
            var userTags = await _userContext.Users.Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Phone == phone);
            return Ok(userTags);
        }

        /// <summary>
        /// 更新用户标签数据
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("tags")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateUserTags([FromBody]List<string> tags)
        {
            var userId = _identityService.GetUserIdentity();

            var originTags = await _userContext.UserTages.Where(u => u.AppUserId == userId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag));

            await _userContext.UserTages.AddRangeAsync(newTags.Select(t => new UserTage
            {
                CreatedTime = DateTime.Now,
                AppUserId = userId,
                Tag = t
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }
    }
}
