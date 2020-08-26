using Microsoft.AspNetCore.Identity;

namespace User.API.Dtos
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
