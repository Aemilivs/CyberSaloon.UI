using Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages
{
    public partial class WelcomePage : ComponentBase
    {
        [Inject]
        public ICoreAPIClient CoreClient { get; init; }
        
        [Inject]
        public IIdentityServerAPIClient ServerClient { get; init; }

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        [Inject]
        public NavigationManager Manager { get; init; }

        protected async override Task OnInitializedAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();
            var user = state.User;
            var username =
                user
                    .Claims
                    .Single(it => it.Type == "username")
                    .Value;

            var artist = await GetOrCreateArtist(username).ConfigureAwait(false);
            var applicant = await GetOrCreateApplicant(username).ConfigureAwait(false);
    
            var payload =
                new UpdateUserDTO
                {
                    ApplicantID = applicant?.Id ?? Guid.Empty,
                    ArtistID = artist?.Id ?? Guid.Empty
                };
                
            await ServerClient
                    .UpdateUserAsync(payload)
                    .ConfigureAwait(false);
        }

        private async Task<ApplicantGetDTO> GetOrCreateApplicant(string username)
        {
            var response =
                await CoreClient
                            .GetApplicantAsync()
                            .ConfigureAwait(false);

            if (response == default)
                return
                    await CoreClient
                            .PostApplicantAsync(username)
                            .ConfigureAwait(false);

            return response;
        }

        private async Task<ArtistGetDTO> GetOrCreateArtist(string username)
        {
            var response =
                await CoreClient
                            .GetArtistAsync()
                            .ConfigureAwait(false);

            if (response == default)
                return 
                    await CoreClient
                            .PostArtistAsync(username)
                            .ConfigureAwait(false);
                    
            return response;
        }
    }
}
