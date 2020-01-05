using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dtos;

namespace Contact.API.Data
{
    public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateContactInfoAsync(UserIdentity user, CancellationToken cancellationToken);

        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AddContactInfoAsync(int userId, UserIdentity contact, CancellationToken cancellationToken);

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Models.Contact>> GetContactsAsync(int userId);

        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="tags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> TagContactsAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken);
    }
}
