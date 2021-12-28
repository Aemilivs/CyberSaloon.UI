using Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Applications.DTO;
using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Arts.DTO;
using CyberSaloon.Client.Pages.Common.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Common
{
    public class CoreAPIClient : ICoreAPIClient
    {
        private readonly HttpClient _authorizedClient;
        private readonly HttpClient _unauthorizedClient;
        private readonly AuthenticationStateProvider _provider;

        public CoreAPIClient(
                IHttpClientFactory factory,
                AuthenticationStateProvider provider 
            )
        {
            if(factory == null)
                throw new ArgumentNullException(nameof(factory));

            _provider = 
                provider ?? 
                throw new ArgumentNullException(nameof(provider));

            _authorizedClient = factory.CreateClient("CyberSaloon.CoreAPI");
            _unauthorizedClient = factory.CreateClient("CyberSaloon.CoreAPI.Anonymous");
        }

        public async Task<ApplicantGetDTO> GetApplicantAsync(Guid applicantId)
        {
            var response = await _unauthorizedClient.GetAsync($"applicants/{applicantId}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicantGetDTO>(raw);
        }

        public async Task<ApplicantGetDTO> GetApplicantAsync(string applicantAlias)
        {
            var response = await _unauthorizedClient.GetAsync($"applicants/profile/{applicantAlias}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicantGetDTO>(raw);
        }

        public async Task<Pagination<ApplicantGetDTO>> GetApplicantsAsync(ResourceParameters parameters)
        {
            var response = await _unauthorizedClient.GetAsync($"applicants?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pagination<ApplicantGetDTO>>(raw);
        }

        public async Task<ApplicationGetDTO> GetApplicationAsync(Guid applicationId)
        {
            var user = await _provider.GetAuthenticationStateAsync();
            var response = await _unauthorizedClient.GetAsync($"applications/{applicationId}");
            
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicationGetDTO>(raw);
        }

        public async Task<Pagination<ApplicationGetDTO>> GetApplicationsAsync(
                ResourceParameters parameters,
                string action,
                string id
            )
        {
            action =
                action == default ?
                    string.Empty :
                    $"/{action}";
            id = 
                id == default ? 
                    string.Empty :
                    $"/{id}";
            var url = $"applications{action}{id}?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";
            
            var response = await _unauthorizedClient.GetAsync(url);

            if(response.StatusCode == HttpStatusCode.Unauthorized)
                response = await _authorizedClient.GetAsync(url);

            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Pagination<ApplicationGetDTO>>(raw);
        }

        public async Task<ArtGetDTO> GetArtAsync(Guid artId)
        {
            var response = await _unauthorizedClient.GetAsync($"arts/{artId}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtGetDTO>(raw);
        }

        public async Task<Pagination<ArtGetDTO>> GetArtsAsync(ResourceParameters parameters)
        {
            var response = await _unauthorizedClient.GetAsync($"arts?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pagination<ArtGetDTO>>(raw);
        }

        public async Task<ArtistGetDTO> GetArtistAsync(Guid artistId)
        {
            var response = await _unauthorizedClient.GetAsync($"artists/{artistId}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtistGetDTO>(raw);
        }

        public async Task<ArtistGetDTO> GetArtistAsync(String artistAlias)
        {
            var response = await _unauthorizedClient.GetAsync($"artists/profile/{artistAlias}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtistGetDTO>(raw);
        }

        public async Task<Pagination<ArtistGetDTO>> GetArtistsAsync(ResourceParameters parameters)
        {
            var response = await _unauthorizedClient.GetAsync($"artists?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}");
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pagination<ArtistGetDTO>>(raw);
        }

        public async Task<ApplicationGetDTO> PostApplicationAsync(ApplicationPostDTO application)
        {
            var content = JsonConvert.SerializeObject(application);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json"
                    );
            var response = await _authorizedClient.PostAsync($"applications", payload);
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicationGetDTO>(raw);
        }

        
        public async Task<bool> PatchApplicationAsync(Guid applicationId, JsonPatchDocument patchDocument)
        {
            var content = JsonConvert.SerializeObject(patchDocument);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json-patch+json"
                    );
            var response = await _authorizedClient.PatchAsync($"applications/{applicationId}", payload);
            
            if(!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        public async Task<bool> DeleteApplicationAsync(Guid applicationId)
        {
            var response = await _authorizedClient.DeleteAsync($"applications/{applicationId}");
            
            if(!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        public async Task<ApplicantGetDTO> PostApplicantAsync(string alias)
        {
            var serializee =
                new 
                {
                    Alias = alias
                };
            var content = JsonConvert.SerializeObject(serializee);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );
            var response = await _authorizedClient.PostAsync($"applicants", payload);
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicantGetDTO>(raw);
        }

        public async Task<ArtistGetDTO> PostArtistAsync(string alias)
        {
            var serializee =
                new 
                {
                    Alias = alias
                };
            var content = JsonConvert.SerializeObject(serializee);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json"
                    );
            var response = await _authorizedClient.PostAsync($"artists", payload);
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtistGetDTO>(raw);
        }

        public async Task<ApplicantGetDTO> GetApplicantAsync()
        {
            var response = await _authorizedClient.GetAsync($"applicants/self");
            if (!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApplicantGetDTO>(raw);
        }

        public async Task<ArtistGetDTO> GetArtistAsync()
        {
            var response = await _authorizedClient.GetAsync($"artists/self");
            if (!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtistGetDTO>(raw);
        }

        public async Task GetApplyAsync(Guid applicationId) => 
            await _authorizedClient.GetAsync($"applications/apply/{applicationId}");

        public async Task GetDefyAsync(Guid applicationId) => 
            await _authorizedClient.GetAsync($"applications/defy/{applicationId}");

        public async Task<ArtGetDTO> PostArtAsync(ArtPostDTO art)
        {
            var content = JsonConvert.SerializeObject(art);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json"
                    );
            var response = await _authorizedClient.PostAsync($"arts", payload);
            if(!response.IsSuccessStatusCode)
                return default;

            var raw = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ArtGetDTO>(raw);
        }

        
        public async Task<bool> PatchArtAsync(Guid artId, JsonPatchDocument patchDocument)
        {
            var content = JsonConvert.SerializeObject(patchDocument);
            var payload = 
                new StringContent(
                        content, 
                        System.Text.Encoding.UTF8, 
                        "application/json-patch+json"
                    );
            var response = await _authorizedClient.PatchAsync($"arts/{artId}", payload);
            
            if(!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        public async Task<bool> DeleteArtAsync(Guid artId)
        {
            var response = await _authorizedClient.DeleteAsync($"arts/{artId}");
            
            if(!response.IsSuccessStatusCode)
                return false;

            return true;
        }
    }
}
