using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace User.API.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int GetUserIdentity()
        {
            if (!int.TryParse(_context.HttpContext.User.FindFirst("sub").Value, out int userId))
                throw new PlatformNotSupportedException("token错误");
            return userId;
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.FindFirst("name").Value;
        }
    }
}
