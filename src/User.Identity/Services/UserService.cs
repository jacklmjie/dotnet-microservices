using DnsClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using User.Identity.Dtos;
using User.Identity.Infrastructure;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _userServiceUrl;
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

        public async Task<UserIdentity> CheckOrCreateAsync(string phone)
        {
            var json = JsonConvert.SerializeObject(new { phone });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_userServiceUrl + "/api/users/check-or-create", content);
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
