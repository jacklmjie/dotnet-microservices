namespace User.Identity.Infrastructure.Services
{
    public interface IServiceDiscovery
    {
        string FindServiceInstances(string name);
    }
}
