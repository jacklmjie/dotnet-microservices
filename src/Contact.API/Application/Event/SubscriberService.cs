using Contact.API.Dtos;
using Contact.API.Application.Repositories;
using DotNetCore.CAP;
using System.Threading;

namespace Contact.API.Application.Event
{
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        private readonly IContactRepository _contactRepository;
        public SubscriberService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [CapSubscribe("user.api.user_patch_change_event")]
        public void UserPatchChangedEvent(UserIdentityDTO identity)
        {
            var token = new CancellationToken();
            _contactRepository.UpdateContactInfoAsync(identity, token);
        }
    }
}
