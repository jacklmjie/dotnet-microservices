using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using User.API.Dtos;

namespace User.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected UserIdentity UserIdentity
        {
            get
            {
                var nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var identity = new UserIdentity();

                if (!int.TryParse(claimsIdentity.Claims.FirstOrDefault(x => x.Type == nameClaimType).Value, out int userId))
                    throw new PlatformNotSupportedException("token错误");

                identity.UserId = userId;
                identity.Name = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "name").Value;

                return identity;
            }
        }
    }
}
