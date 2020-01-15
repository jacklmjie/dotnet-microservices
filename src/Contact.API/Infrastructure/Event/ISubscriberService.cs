using Contact.API.Dtos;

namespace Contact.API.Infrastructure.Event
{
    public interface ISubscriberService
    {
        public void UserPatchChangedEvent(UserIdentityDTO identity);
    }
}
