using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recommend.API.Application;
using Microsoft.AspNetCore.Authorization;
using Recommend.API.Services;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Recommend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController : ControllerBase
    {
        private readonly RecommendDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public RecommendController(RecommendDbContext dbContext,
            IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        [Route("")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            var userId = _identityService.GetUserIdentity();

            var result = await _dbContext.ProjectRecommends.AsNoTracking()
                .Where(u => u.UserId == userId)
                .ToListAsync();

            return Ok(result);
        }
    }
}
