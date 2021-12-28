using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using CyberSaloon.Client.Pages.Applications.DTO;

namespace CyberSaloon.Client.Pages.Applications.Pages
{
    public partial class PostApplicationPage : ComponentBase
    {
        public ApplicationPostDTO Payload { get; set; } = new();

        [Parameter]
        public string AuthorAlias { get; set; } = default;
        
        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public bool IsLoading { get; set; } = false;
        public bool IsFailed { get; set; } = false;

        public async void PostApplicationAsync()
        {
            IsLoading = true;
            IsFailed = false;

            Payload.Artist = AuthorAlias;
            var result = await Client.PostApplicationAsync(Payload);
            IsLoading = false;
            
            if(result == null)
            {
                IsFailed = true;
                return;
            }

            NavigationManager.NavigateTo($"/application/{result.Id}");
        }
    }
}
