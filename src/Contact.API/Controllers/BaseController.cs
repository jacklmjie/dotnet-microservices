using Contact.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected UserIdentity UserIdentity => new UserIdentity() { UserId = 1, Name = "jack.li" };
    }
}
