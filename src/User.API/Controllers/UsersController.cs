using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.API.Data;
using User.API.Entity.Models;
using User.API.Filters;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private UserContext _useContext;
        private ILogger<UsersController> _logger;
        public UsersController(UserContext userContext, ILogger<UsersController> logger)
        {
            _useContext = userContext;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var user = await _useContext.Users
                .AsNoTracking()
                .Include(p => p.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                //return NotFound();
                throw new UserOperationException($"错误的用户上下文Id={UserIdentity.UserId}");

            return Ok(user);
        }

        [Route("")]
        [HttpPatch]
        public async Task<ActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _useContext.Users
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserOperationException($"错误的用户上下文Id={UserIdentity.UserId}");

            patch.ApplyTo(user);

            var userProperties = user.Properties == null ? new List<UserProperty>() : user.Properties;
            foreach (var property in userProperties)
            {
                _useContext.Entry(property).State = EntityState.Detached;
            }

            var originProperties = await _useContext.UserProperties
                .AsNoTracking()
                .Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var allProperties = originProperties.Union(userProperties).Distinct();

            var removeProperties = originProperties.Except(userProperties);
            var newProperties = allProperties.Except(originProperties);

            foreach (var property in removeProperties)
            {
                _useContext.Remove(property);
            }

            foreach (var property in newProperties)
            {
                _useContext.Add(property);
            }

            _useContext.Users.Update(user);
            _useContext.SaveChanges();

            return Ok(user);
        }
    }
}
