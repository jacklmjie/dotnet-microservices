using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Recommend.API.Dtos;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Recommend.API.Services
{
    public class ContactService : IContactService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ServiceDiscoveryOptions _serviceDiscoveryOptions;
        public ContactService(HttpClient httpClient,
            IServiceDiscovery serviceDiscovery,
            IOptions<ServiceDiscoveryOptions> options)
        {
            _httpClient = httpClient;
            _serviceDiscovery = serviceDiscovery;
            _serviceDiscoveryOptions = options.Value;
        }

        /// <summary>
        /// 获取用户好友
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ContactDTO>> GetContactsByUserIdAsync(int userId)
        {
            var url = _serviceDiscovery.FindServiceInstances(_serviceDiscoveryOptions.ContactServiceName);
            var response = await _httpClient.GetAsync(url + $"/api/contacts/{userId}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var contacts = JsonConvert.DeserializeObject<List<ContactDTO>>(result);
                return contacts;
            }
            return null;
        }
    }
}
