using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using User.Identity.Dtos;
using User.Identity.Services;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ServiceDiscoveryOptions _serviceDiscoveryOptions;
        public UserService(HttpClient httpClient,
            IServiceDiscovery serviceDiscovery,
            IOptions<ServiceDiscoveryOptions> options)
        {
            _httpClient = httpClient;
            _serviceDiscovery = serviceDiscovery;
            _serviceDiscoveryOptions = options.Value;
        }

        public async Task<UserIdentityDTO> CheckOrCreateAsync(string phone)
        {
            var url = _serviceDiscovery.FindServiceInstances(_serviceDiscoveryOptions.UserServiceName);
            var json = JsonConvert.SerializeObject(new { phone });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url + "/api/users/check-or-create", content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var userIdentity = JsonConvert.DeserializeObject<UserIdentityDTO>(result);
                return userIdentity;
            }
            return null;
        }
    }
}
