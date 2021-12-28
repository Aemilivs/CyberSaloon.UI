using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace CyberSaloon.Client.Pages
{
    public partial class Profile : ComponentBase
    {
        [Parameter]
        public string Alias { get; set; }
        
        [Inject]
        public ICoreAPIClient Client { get; init; }
        
        public ArtistGetDTO ArtistDTO { get; set; } = default;
        public ApplicantGetDTO ApplicantDTO { get; set; } = default;
        public ProfileSelectedApplication Selection { get; set; } = ProfileSelectedApplication.None;
        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

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
