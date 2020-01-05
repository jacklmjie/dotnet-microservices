using MongoDB.Driver;
using Contact.API.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

namespace Contact.API.Data
{
    public class ContactContext
    {
        private IMongoDatabase _database;
        private ContactOptions _options;

        public ContactContext(IOptionsSnapshot<ContactOptions> options)
        {
            _options = options.Value;
            var client = new MongoClient(_options.MongoConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(_options.MongoDatabase);
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
                CheckAndCreateCollection("ContactBooks");
                return _database.GetCollection<ContactBook>("ContactBooks");
            }
        }

        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateCollection("ContactApplyRequests");
                return _database.GetCollection<ContactApplyRequest>("ContactApplyRequests");
            }
        }
    }
}
