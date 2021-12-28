using CyberSaloon.Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Arts.DTO;

namespace CyberSaloon.Client.Pages.Arts.Pages
{
    public partial class Arts : ComponentBase
    {
        public ArtGetDTO[] arts = Array.Empty<ArtGetDTO>();

        [Inject]
        public ICoreAPIClient Client { get; init; }

        protected async override Task OnInitializedAsync()
        {
            ResourceParameters parameters = new();
            arts = (await Client.GetArtsAsync(parameters)).Entities.ToArray();
        }
    }
}
