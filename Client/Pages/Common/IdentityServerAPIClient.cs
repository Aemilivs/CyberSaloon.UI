using System;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Pages.Common.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace CyberSaloon.Client.Pages.Common
{
    public class IdentityServerAPIClient : IIdentityServerAPIClient
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _provider;

        public IdentityServerAPIClient(
                IHttpClientFactory factory,
                AuthenticationStateProvider provider 
            )
        {
            if(factory == null)
                throw new ArgumentNullException(nameof(factory));

            _provider = 
                provider ?? 
                throw new ArgumentNullException(nameof(provider));

            var name = 
                _provider
                    .GetAuthenticationStateAsync()
                    .Result
                    .User
                    .Identity
                    .Name;
            
            var isAuthorized = name != null;
            
            _client = 
                isAuthorized ?
                factory.CreateClient("CyberSaloon.ServerAPI") :
                factory.CreateClient("CyberSaloon.ServerAPI.Anonymous");
        }
        
        public async Task UpdateUserAsync(UpdateUserDTO dto)
        {
            var content = JsonConvert.SerializeObject(dto);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json"
                    );
            var response = await _client.PostAsync($"_configuration/update", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}