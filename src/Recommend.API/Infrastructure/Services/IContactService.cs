using Microsoft.Extensions.Options;
using Recommend.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Recommend.API.Infrastructure.Services
{
    public interface IContactService
    {
        /// <summary>
        /// 获取用户好友
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ContactDTO>> GetContactsByUserIdAsync(int userId);
    }
}
