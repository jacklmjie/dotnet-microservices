﻿namespace Contact.API.Infrastructure.Services
{
    public interface IServiceDiscovery
    {
        string FindServiceInstances(string name);
    }
}