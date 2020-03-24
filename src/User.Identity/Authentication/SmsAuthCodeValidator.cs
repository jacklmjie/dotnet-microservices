using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using User.Identity.Services;

namespace User.Identity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        private readonly IAuthCodeService _authCodeService;
        private readonly IUserService _userService;
        public SmsAuthCodeValidator(IAuthCodeService authCodeService,
            IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;
        }

        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            //验证字段
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
            {
                context.Result = errorValidationResult;
                return;
            }

            //检查验证码
            if (!_authCodeService.Validate(phone, code))
            {
                context.Result = errorValidationResult;
                return;
            }

            //完成用户注册
            var userIdentity = await _userService.CheckOrCreateAsync(phone);
            if (userIdentity == null)
            {
                context.Result = errorValidationResult;
                return;
            }

            var claims = new Claim[]
            {
                new Claim("name",userIdentity.Name??string.Empty)
            };

            context.Result = new GrantValidationResult(userIdentity.UserId.ToString(), GrantType, claims);
        }
    }
}
