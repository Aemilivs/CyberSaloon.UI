using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using CyberSaloon.Client.Pages.Arts.DTO;
using System;

namespace CyberSaloon.Client.Pages.Arts.Pages
{
    public partial class PostArtPage : ComponentBase
    {
        public ArtPostDTO Payload { get; set; } = new();

        [Parameter]
        public string ApplicationId { get; set; } = default;
        
        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        public async void PostArtAsync()
        {
            IsLoading = true;
            IsFailed = false;

            var isGuid = Guid.TryParse(ApplicationId, out var id);

            if(!isGuid)
            {
                IsLoading = false;
                IsFailed = true;
                return;
            }

            Payload.Application = id;
            var result = await Client.PostArtAsync(Payload);
            IsLoading = false;
            
            if(result == null)
            {
                IsFailed = true;
                return;
            }

            NavigationManager.NavigateTo($"/art/{result.Id}");
        }
    }
}
