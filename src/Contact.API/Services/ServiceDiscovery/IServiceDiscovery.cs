namespace Contact.API.Services
{
    public interface IServiceDiscovery
    {
        string FindServiceInstances(string name);
    }
}
