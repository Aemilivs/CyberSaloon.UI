using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Common;
using CyberSaloon.Client.Pages.Common.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Applicants.Pages
{
    public partial class ApplicantsPage : ComponentBase
    {
        public ApplicantGetDTO[] ApplicantsDTO = Array.Empty<ApplicantGetDTO>();

        [Inject]
        public ICoreAPIClient Client { get; init; }

        protected async override Task OnInitializedAsync()
        {
            ResourceParameters parameters = new();
            ApplicantsDTO = (await Client.GetApplicantsAsync(parameters)).Entities.ToArray();
        }
    }
}