using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Dtos;

namespace Contact.API.Service
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<UserIdentity> GetUserAsync(int UserId);
    }
}
