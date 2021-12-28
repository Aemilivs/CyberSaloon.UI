using CyberSaloon.Client.Pages.Arts.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Arts.Pages
{
    public partial class ArtComponent : ComponentBase
    {
        [Parameter]
        public string RawId { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        /// <summary>
        /// Flag describing if this art page is rendering standalone art
        /// or an art within an application.
        /// </summary>
        /// <value>True by default.</value>
        [Parameter]
        public bool IsStandalone { get; set; } = true;

        [Parameter]
        public ArtGetDTO ArtDTO { get; set; } = default;

        [Parameter]
        public bool ArtIsSubmitted { get; set; } = false;

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        [Inject]
        public NavigationManager Navigation { get; init; }

        public bool IsLoading { get; set; } = false;

        public bool IsFailed { get; set; } = false;

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            IsFailed = false;
            IsLoading = false;

            var result = Guid.TryParse(RawId, out var artId);

            if (Id == default && ArtDTO == default && !result)
            {
                IsFailed = true;
                return;
            }
            else
                if(result)
                    Id = artId;

            if (ArtDTO != default && ArtDTO.Id == Id)
                return;

            IsLoading = true;
            ArtDTO = await Client.GetArtAsync(Id);
            IsLoading = false;

            ArtIsSubmitted = await ArtIsSubmittedAsync();
        }

        private async Task<bool> ArtIsSubmittedAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();

            if (state == null)
                return false;

            var user = state.User;

            if(user == default)
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
                        ArtDTO.Author, 
                        username,
                        StringComparison.InvariantCulture
                    ); 

            if(namesAreEqual)
                return true;

            return false;
        }

        private string ProcessDescription()
        {
            if(IsStandalone)
                return ArtDTO.Description;

            var threshold = 200;

            if(ArtDTO.Description.Length <= threshold)
                return ArtDTO.Description;

            return 
                String.Join(
                        string.Empty, 
                        ArtDTO
                            .Description
                            .Substring(0, threshold - 3), 
                        "..."
                    );
        }

        private async Task DeleteArtAsync()
        {
            await Client.DeleteArtAsync(ArtDTO.Id);

            ArtDTO = default;

            if(IsStandalone)
                Navigation.NavigateTo("/");
            else
                Navigation.NavigateTo(Navigation.Uri, forceLoad: false);
        }
    }
}
