using System;

namespace Core.Data.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
