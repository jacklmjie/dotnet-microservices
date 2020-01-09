using Contact.API.Dtos;

namespace Contact.API.Event
{
    public interface ISubscriberService
    {
        public void UserPatchChangedEvent(UserIdentity identity);
    }
}
