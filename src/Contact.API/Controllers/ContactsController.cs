using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contact.API.Models;
using System.Threading;
using Contact.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Contact.API.Infrastructure.Repositories;
using Contact.API.Infrastructure.Services;

namespace Contact.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        public ContactsController(IContactApplyRequestRepository contactApplyRequestRep,
            IContactRepository contactRepository,
            IUserService userService,
            IIdentityService identityService)
        {
            _contactApplyRequestRepository = contactApplyRequestRep;
            _contactRepository = contactRepository;
            _userService = userService;
            _identityService = identityService;
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            var userId = _identityService.GetUserIdentity();
            var contacts = await _contactRepository.GetContactsAsync(userId);
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
            var userId = _identityService.GetUserIdentity();
            var result = await _contactRepository.TagContactsAsync(userId, viewModel.ContactId, viewModel.Tags, cancellationToken);

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
            var userId = _identityService.GetUserIdentity();
            var requests = await _contactApplyRequestRepository.GetRequestListAsync(userId, cancellationToken);
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

            var userId = _identityService.GetUserIdentity();
            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                UserId = userId,
                Name = user.Name,
                ApplierId = userId,
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
            var userId = _identityService.GetUserIdentity();
            var result = await _contactApplyRequestRepository.ApprovalAsync(userId, userId, cancellationToken);
            if (!result)
            {
                //log
                return BadRequest();
            }
            var applier = await _userService.GetUserAsync(userId);
            var user = await _userService.GetUserAsync(userId);

            await _contactRepository.AddContactInfoAsync(userId, applier, cancellationToken);
            await _contactRepository.AddContactInfoAsync(userId, user, cancellationToken);

            return Ok();
        }
    }
}
