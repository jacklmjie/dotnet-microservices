using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Contact.API.Models;
using Contact.API.Data;
using Contact.API.Service;
using System.Threading;
using Contact.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Contact.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : BaseController
    {
        private IContactApplyRequestRepository _contactApplyRequestRepository;
        private IContactRepository _contactRepository;
        private IUserService _userService;
        public ContactsController(IContactApplyRequestRepository contactApplyRequestRep,
            IContactRepository contactRepository,
            IUserService userService)
        {
            _contactApplyRequestRepository = contactApplyRequestRep;
            _contactRepository = contactRepository;
            _userService = userService;
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            var contacts = await _contactRepository.GetContactsAsync(UserIdentity.UserId);
            return Ok(contacts);
        }

        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("tag")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> TagContacts(TagContactInputViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _contactRepository.TagContactsAsync(UserIdentity.UserId, viewModel.ContactId, viewModel.Tags, cancellationToken);

            if (result)
            {
                return Ok();
            }

            //log
            return BadRequest();
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetApplyRequests(CancellationToken cancellationToken)
        {
            var requests = await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId, cancellationToken);
            return Ok(requests);
        }

        /// <summary>
        /// 添加好友请求
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddApplyRequest(int userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                throw new Exception("用户参数错误");
            }

            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                UserId = userId,
                Name = user.Name,
                ApplierId = UserIdentity.UserId,
                ApplyTime = DateTime.Now
            }, cancellationToken);

            if (!result)
            {
                //log
                return BadRequest();
            }
            return Ok();
        }

        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <param name="applierId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-requests/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ApprovalApplyRequest(int userId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(userId, UserIdentity.UserId, cancellationToken);
            if (!result)
            {
                //log
                return BadRequest();
            }
            var applier = await _userService.GetUserAsync(userId);
            var user = await _userService.GetUserAsync(UserIdentity.UserId);

            await _contactRepository.AddContactInfoAsync(UserIdentity.UserId, applier, cancellationToken);
            await _contactRepository.AddContactInfoAsync(userId, user, cancellationToken);

            return Ok();
        }
    }
}
