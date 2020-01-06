using System.Collections.Generic;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Data.IRepository
{
    public interface IUserPropertyRepository
    {
        Task<int> Create(List<UserProperty> Properties);

        Task<int> Delete(int userId);
    }
}