using System.Threading.Tasks;
using User.API.Entity.Models;

namespace User.API.Data.IRepository
{
    public interface IUserRepository
    {
        Task<AppUser> GetAsync(int id);

        Task<AppUser> GetByContribAsync(int id);
    }
}
