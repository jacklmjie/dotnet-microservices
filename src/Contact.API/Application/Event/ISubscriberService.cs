using Contact.API.Dtos;

namespace Contact.API.Application.Event
{
    public interface ISubscriberService
    {
        public void UserPatchChangedEvent(UserIdentityDTO identity);
    }
}
