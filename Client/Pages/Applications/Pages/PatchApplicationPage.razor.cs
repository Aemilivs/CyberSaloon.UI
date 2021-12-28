using System;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Applications.DTO;
using CyberSaloon.Client.Pages.Common;
using CyberSaloon.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;

namespace CyberSaloon.Client.Pages.Applications.Pages
{
    public partial class PatchApplicationPage : ComponentBase
    {
        public ApplicationPatchDTO DestinationDTO { get; set; } = new();

        [Parameter]
        public string ApplicationId { get; set; } = default;

        public ApplicationPatchDTO SourceDTO { get; set; } = new();

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            var result = Guid.TryParse(ApplicationId, out var id);

            if (!result)
            {
                if (SourceDTO != default)
                    ApplicationId = SourceDTO.Id.ToString();

                return;
            }

            if (SourceDTO != default && SourceDTO.Id == id)
                return;

            var application = await Client.GetApplicationAsync(id);

            SourceDTO = 
                new ApplicationPatchDTO
                {
                    Id = application.Id,
                    Summary = application.Summary,
                    Description = application.Description
                };

            DestinationDTO = 
                new ApplicationPatchDTO
                {
                    Id = application.Id,
                    Summary = application.Summary,
                    Description = application.Description
                };
        }

        public async void PatchApplicationAsync()
        {
            IsFailed = false;

            var patch = JsonPatchDocumentHelper.CreatePatch<ApplicationPatchDTO>(SourceDTO, DestinationDTO);

            IsLoading = true;

            var result = await Client.PatchApplicationAsync(SourceDTO.Id, patch);
            IsLoading = false;

            if(!result)
            {
                IsFailed = true;
                return;
            }

            NavigationManager.NavigateTo($"/application/{SourceDTO.Id}");
        }
    }
}