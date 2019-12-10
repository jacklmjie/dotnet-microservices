using System.Threading.Tasks;
using Core.Data.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.API.Data;
using User.API.Data.IRepository;
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
        private IUserRepository _userRepository;
        private IUserPropertyRepository _userPropertyRepository;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        public UsersController(UserContext userContext,
            ILogger<UsersController> logger,
            IUserRepository userRepository,
            IUserPropertyRepository userPropertyRepository,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _useContext = userContext;
            _logger = logger;
            _userRepository = userRepository;
            _userPropertyRepository = userPropertyRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [Route("")]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //var user1 = await _userRepository.GetAsync(UserIdentity.UserId);
            //var user2 = await _userRepository.GetByContribAsync(UserIdentity.UserId);

            var user = await _useContext.Users
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
    }
}
