namespace User.Identity.Infrastructure.Services
{
    public class TestAuthCodeService : IAuthCodeService
    {
        public bool Validate(string phone, string authCode)
        {
            return true;
        }
    }
}
