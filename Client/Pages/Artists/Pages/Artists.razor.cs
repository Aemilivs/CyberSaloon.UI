using CyberSaloon.Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Artists.DTO;

namespace CyberSaloon.Client.Pages.Artists.Pages
{
    public partial class Artists : ComponentBase
    {
        public ArtistGetDTO[] artists = Array.Empty<ArtistGetDTO>();

        [Inject]
        public ICoreAPIClient Client { get; init; }

        protected async override Task OnInitializedAsync()
        {
            ResourceParameters parameters = new();
            artists = (await Client.GetArtistsAsync(parameters)).Entities.ToArray();
        }
    }
}
