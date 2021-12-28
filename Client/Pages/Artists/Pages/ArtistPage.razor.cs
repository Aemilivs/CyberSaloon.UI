using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Artists.Pages
{
    public partial class ArtistPage : ComponentBase
    {
        [Parameter]
        public string Alias { get; set; }

        [Parameter]
        public ArtistGetDTO ArtistDTO { get; set; } = default;

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        protected async override Task OnInitializedAsync()
        {
            if (ArtistDTO != default && ArtistDTO.Alias == Alias)
                return;

            ArtistDTO = await Client.GetArtistAsync(Alias);
        }

        private async Task<bool> ArtistIsUser()
        {
            var state = await Provider.GetAuthenticationStateAsync();
            var user = state.User;

            if (user == default)
                return false;

            if (!user.Claims.Any())
                return false;

            var username =
                 user
                     .Claims
                     .Single(it => it.Type == "username")
                     .Value;

            var namesAreEqual =
                string.Equals(
                        ArtistDTO.Alias,
                        username,
                        StringComparison.InvariantCulture
                    );

            if (namesAreEqual)
                return true;

            return false;
        }

        private string BuildFullfilledCountMessage()
        {
            var count = ArtistDTO.Applications.Count();
            var subject = 
                count == 1 ? 
                    "application" : 
                    "applications";
            return String.Join(' ', "Fullfilled", count, subject);
        }
    }
}
