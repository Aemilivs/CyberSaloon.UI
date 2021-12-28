using System;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Arts.DTO;
using CyberSaloon.Client.Pages.Common;
using CyberSaloon.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;

namespace CyberSaloon.Client.Pages.Arts.Pages
{
    public partial class PatchArtPage : ComponentBase
    {
        public ArtPatchDTO DestinationDTO { get; set; } = new();

        [Parameter]
        public string ArtId { get; set; } = default;

        public ArtPatchDTO SourceDTO { get; set; } = new();

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            var result = Guid.TryParse(ArtId, out var id);

            if (!result)
            {
                if (SourceDTO != default)
                    ArtId = SourceDTO.Id.ToString();

                return;
            }

            if (SourceDTO != default && SourceDTO.Id == id)
                return;

            var art = await Client.GetArtAsync(id);

            SourceDTO = 
                new ArtPatchDTO
                {
                    Id = art.Id,
                    Summary = art.Summary,
                    Description = art.Description,
                    Url = art.Url
                };

            DestinationDTO = 
                new ArtPatchDTO
                {
                    Id = art.Id,
                    Summary = art.Summary,
                    Description = art.Description,
                    Url = art.Url
                };
        }

        public async void PatchArtAsync()
        {
            IsFailed = false;

            var patch = JsonPatchDocumentHelper.CreatePatch<ArtPatchDTO>(SourceDTO, DestinationDTO);

            IsLoading = true;

            var result = await Client.PatchArtAsync(SourceDTO.Id, patch);
            IsLoading = false;

            if(!result)
            {
                IsFailed = true;
                return;
            }

            NavigationManager.NavigateTo($"/art/{SourceDTO.Id}");
        }
    }
}