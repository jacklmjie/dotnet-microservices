using Microsoft.AspNetCore.Http;
using Project.Domain.Exceptions;
using System;

namespace Project.API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int GetUserIdentity()
        {
            if (!int.TryParse(_context.HttpContext.User.FindFirst("sub").Value, out int userId))
                throw new ProjectDomainException("token错误");
            return userId;
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.FindFirst("name").Value;
        }
    }
}
