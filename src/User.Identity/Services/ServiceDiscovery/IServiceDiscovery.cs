namespace User.Identity.Services
{
    public interface IServiceDiscovery
    {
        string FindServiceInstances(string name);
    }
}
