using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Data;
using User.API.Data.IRepository;
using User.API.Dtos;
using User.API.Models;
using User.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly UserContext _userContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserPropertyRepository _userPropertyRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public UsersController(UserContext userContext,
            IUserRepository userRepository,
            IUserPropertyRepository userPropertyRepository,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _userContext = userContext;
            _userRepository = userRepository;
            _userPropertyRepository = userPropertyRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [Route("")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            //var user1 = await _userRepository.GetAsync(UserIdentity.UserId);
            //var user2 = await _userRepository.GetByContribAsync(UserIdentity.UserId);

            var user = await _userContext.Users
                .AsNoTracking()
                .Include(p => p.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserOperationException($"错误的用户上下文Id={UserIdentity.UserId}");

            return Ok(user);
        }

        [Route("")]
        [HttpPatch]
        public async Task<ActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _userContext.Users
               .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserOperationException($"错误的用户上下文Id={UserIdentity.UserId}");

            patch.ApplyTo(user);

            _userContext.Users.Update(user);
            _userContext.SaveChanges();
            return Ok(user);
        }

        [Route("PatchByUnit")]
        [HttpPatch]
        public async Task<ActionResult> PatchByUnit([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _userRepository.GetByContribAsync(UserIdentity.UserId);

            if (user == null)
                throw new UserOperationException($"错误的用户上下文Id={UserIdentity.UserId}");

            patch.ApplyTo(user);

            //mysql事务,参考https://fl.vu/mysql-trans
            var unit = _unitOfWorkFactory.Create();
            await _userPropertyRepository.Delete(user.Id);
            user.Properties.ForEach(x => x.AppUserId = user.Id);
            await _userPropertyRepository.Create(user.Properties);
            unit.SaveChanges();
            return Ok(user);
        }

        /// <summary>
        /// 检查或者创建用户（当用户手机不存在时创建用户）
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<ActionResult> CheckOrCreate(AppUserCheckOrCreate dto)
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
            var userIdentity = new UserIdentity
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
        public async Task<ActionResult> GetUserTags()
        {
            var userTags = await _userContext.UserTages.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
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
        public async Task<ActionResult> UpdateUserTags([FromBody]List<string> tags)
        {
            var originTags = await _userContext.UserTages.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag));

            await _userContext.UserTages.AddRangeAsync(newTags.Select(t => new UserTage
            {
                CreatedTime = DateTime.Now,
                AppUserId = UserIdentity.UserId,
                Tag = t
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }
    }
}
