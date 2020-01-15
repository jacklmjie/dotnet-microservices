namespace Contact.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        int GetUserIdentity();

        string GetUserName();
    }
}
