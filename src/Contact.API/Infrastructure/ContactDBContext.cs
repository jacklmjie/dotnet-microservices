using MongoDB.Driver;
using Contact.API.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

namespace Contact.API.Infrastructure
{
    public class ContactDBContext
    {
        private IMongoDatabase _database;
        private ContactDBContextSettings _options;

        public ContactDBContext(IOptions<ContactDBContextSettings> options)
        {
            _options = options.Value;
            var client = new MongoClient(_options.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(_options.DatabaseName);
            }
        }

        private void CheckAndCreateCollection(string collectionName)
        {
            var collectionList = _database.ListCollections().ToList();
            var collectionNames = new List<string>();

            collectionList.ForEach(x => collectionNames.Add(x["name"].AsString));
            if (!collectionNames.Contains(collectionName))
            {
                _database.CreateCollection(collectionName);
            }
        }

        public IMongoCollection<ContactBook> ContactBooks
        {
            get
            {
                CheckAndCreateCollection(_options.ContactBooksCollectionName);
                return _database.GetCollection<ContactBook>(_options.ContactBooksCollectionName);
            }
        }

        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateCollection(_options.ContactApplyRequestsCollectionName);
                return _database.GetCollection<ContactApplyRequest>(_options.ContactApplyRequestsCollectionName);
            }
        }
    }
}
