using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Authentication
{
    public class ProfileServices : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            if (!int.TryParse(subjectId, out int initUserId))
                throw new ArgumentNullException("Invalid subject indetifier");

            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            context.IsActive = int.TryParse(subjectId, out int initUserId);

            return Task.CompletedTask;
        }
    }
}
