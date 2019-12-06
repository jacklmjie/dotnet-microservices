using System.Collections.Generic;
using System.Threading.Tasks;
using User.API.Entity.Models;

namespace User.API.IRepository
{
    public interface IUserPropertyRepository
    {
        Task<int> Create(List<UserProperty> Properties);

        Task<int> Delete(int userId);
    }
}