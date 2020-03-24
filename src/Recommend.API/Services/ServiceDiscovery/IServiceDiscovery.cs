namespace Recommend.API.Services
{
    public interface IServiceDiscovery
    {
        string FindServiceInstances(string name);
    }
}
