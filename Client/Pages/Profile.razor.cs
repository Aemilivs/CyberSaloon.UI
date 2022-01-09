using System;
using System.Linq;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CyberSaloon.Client.Pages
{
    public partial class Profile : ComponentBase
    {
        [Parameter]
        public string Alias { get; set; }
        
        [Inject]
        public ICoreAPIClient Client { get; init; }
        
        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        [Inject]
        public NavigationManager Manager { get; init; }
        
        public ArtistGetDTO ArtistDTO { get; set; } = default;
        public ApplicantGetDTO ApplicantDTO { get; set; } = default;
        public ProfileSelectedApplication Selection { get; set; } = ProfileSelectedApplication.None;
        public bool IsOwned { get; set; } = false;
        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            IsOwned = await ProfileIsOwnedByClientAsync().ConfigureAwait(false);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (string.IsNullOrEmpty(Alias))
                Alias = await GetClientUsernameAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(Alias))
                Manager.NavigateTo("/", false);

            IsFailed = false;
            IsLoading = false;
            
            IsLoading = true;

            if(ArtistDTO == default)
                ArtistDTO = await Client.GetArtistAsync(Alias);

            if(ArtistDTO == default)
            {
                IsLoading = false;
                IsFailed = true;
                return;
            }

            if(ApplicantDTO == default)
                ApplicantDTO = await Client.GetApplicantAsync(Alias);

            if(ApplicantDTO == default)
            {
                IsLoading = false;
                IsFailed = true;
                return;
            }

            IsLoading = false;
        }

        private async Task<bool> ProfileIsOwnedByClientAsync()
        {
            var username = await GetClientUsernameAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(username))
                return false;

            return
                string.Equals(
                        Alias, 
                        username,
                        StringComparison.InvariantCulture
                    ); 
        }

        private async Task<string> GetClientUsernameAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();

            if (state == null)
                return string.Empty;

            var user = state.User;

            if (user == default)
                return string.Empty;

            if (!user.Claims.Any())
                return string.Empty;

            return
                user
                     .Claims
                     .Single(it => it.Type == "username")
                     .Value;
        }
    }

    public enum ProfileSelectedApplication
    {
        None,
        Pending,
        Fullfilled,
        Submitted,
        Liked
    }
}
