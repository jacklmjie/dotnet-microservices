namespace Contact.API.Infrastructure
{
    public class ContactDBContextSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContactBooksCollectionName { get; set; }
        public string ContactApplyRequestsCollectionName { get; set; }
    }
}
