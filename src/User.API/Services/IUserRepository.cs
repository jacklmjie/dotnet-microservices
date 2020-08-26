using System.Collections.Generic;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Services
{
    public interface IUserRepository
    {
        Task<UserTage> GetBasketAsync(int userId);
        IEnumerable<string> GetUsers();
        Task<UserTage> UpdateBasketAsync(UserTage basket);
        Task<bool> DeleteBasketAsync(string id);
    }
}
