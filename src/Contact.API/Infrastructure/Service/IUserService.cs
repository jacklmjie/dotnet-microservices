using System.Threading.Tasks;
using Contact.API.Dtos;

namespace Contact.API.Infrastructure.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<UserIdentityDTO> GetUserAsync(int UserId);
    }
}
