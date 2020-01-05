using System.Threading.Tasks;
using Contact.API.Dtos;
using MongoDB.Driver;
using Contact.API.Models;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Contact.API.Data
{
    public class MongoContactRepository : IContactRepository
    {
        private readonly ContactContext _contactContext;
        public MongoContactRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }

        public async Task<bool> AddContactInfoAsync(int userId, UserIdentity contact, CancellationToken cancellationToken)
        {
            if (_contactContext.ContactBooks.CountDocuments(x => x.UserId == userId) == 0)
            {
                await _contactContext.ContactBooks.InsertOneAsync(new ContactBook { UserId = userId });
            }

            var filter = Builders<ContactBook>.Filter.Eq(x => x.UserId, userId);
            var update = Builders<ContactBook>.Update.AddToSet(x => x.Contacts, new Models.Contact
            {
                UserId = contact.UserId,
                Name = contact.Name,
                Tags = new List<string>()
            });
            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;
        }

        public async Task<List<Models.Contact>> GetContactsAsync(int userId)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(x => x.UserId == userId)).FirstOrDefault();
            if (contactBook != null)
            {
                return contactBook.Contacts;
            }

            //log
            return new List<Models.Contact>();
        }

        public async Task<bool> TagContactsAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactBook>.Filter.And(
               Builders<ContactBook>.Filter.Eq(x => x.UserId, userId),
               Builders<ContactBook>.Filter.Eq("Contacts.UserId", contactId));

            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Tags", tags);

            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount;
        }

        public async Task<bool> UpdateContactInfoAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(x => x.UserId == user.UserId, null, cancellationToken)).FirstOrDefault(cancellationToken);
            if (contactBook == null)
            {
                return true;
            }

            var contactIds = contactBook.Contacts.Select(x => x.UserId);
            var filter = Builders<ContactBook>.Filter.And(
                Builders<ContactBook>.Filter.In(x => x.UserId, contactIds),
                Builders<ContactBook>.Filter.ElemMatch(x => x.Contacts, contact => contact.UserId == user.UserId));

            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Name", user.Name);

            var updateResult = _contactContext.ContactBooks.UpdateMany(filter, update);
            return updateResult.MatchedCount == updateResult.ModifiedCount;
        }
    }
}
