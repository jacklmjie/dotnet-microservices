using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Contact.API.Dtos;
using Contact.API.Infrastructure;
using DnsClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Contact.API.Service
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private string _userServiceUrl;
        public UserService(HttpClient httpClient,
            IDnsQuery dnsQuery,
            IOptions<ServiceDiscoveryOptions> options)
        {
            if (dnsQuery == null)
                throw new ArgumentNullException(nameof(dnsQuery));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _httpClient = httpClient;
            var address = dnsQuery.ResolveService("service.consul", options.Value.ServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ?
                addressList.First().ToString() : address.First().HostName.Substring(0, address.First().HostName.Length - 1);
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}";
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<UserIdentity> GetUserAsync(int UserId)
        {
            var response = await _httpClient.GetAsync(_userServiceUrl + $"/api/users/identity/{UserId}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var userIdentity = JsonConvert.DeserializeObject<UserIdentity>(result);
                return userIdentity;
            }
            return null;
        }
    }
}
