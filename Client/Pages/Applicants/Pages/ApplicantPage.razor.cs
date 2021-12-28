using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Applicants.Pages
{
    public partial class ApplicantPage : ComponentBase
    {
        [Parameter]
        public string Alias { get; set; }

        [Parameter]
        public ApplicantGetDTO ApplicantDTO { get; set; } = default;

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        protected override async Task OnInitializedAsync()
        {
            if (ApplicantDTO != default && ApplicantDTO.Alias == Alias)
            {
                return;
            }

            ApplicantDTO = await Client.GetApplicantAsync(Alias);
        }

        private async Task<bool> ArtistIsUser()
        {
            AuthenticationState state = await Provider.GetAuthenticationStateAsync();
            System.Security.Claims.ClaimsPrincipal user = state.User;

            if (user == default)
            {
                return false;
            }

            string username =
                 user
                     .Claims
                     .Single(it => it.Type == "username")
                     .Value;

            bool namesAreEqual =
                string.Equals(
                        ApplicantDTO.Alias,
                        username,
                        StringComparison.InvariantCulture
                    );

            if (namesAreEqual)
            {
                return true;
            }

            return false;
        }

        private string BuildSupportedCountMessage()
        {
            var count = ApplicantDTO.Supported.Count();
            var subject = 
                count == 1 ? 
                    "application" : 
                    "applications";
            return String.Join(' ', "Supported", count, subject);
        }
    }
}