using Microsoft.AspNetCore.Http;
using System;
using Project.Domain.Exceptions;

namespace Project.API.Application.Services
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
                throw new ProjectDomainException("token错误");
            return userId;
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.FindFirst("name").Value;
        }
    }
}
