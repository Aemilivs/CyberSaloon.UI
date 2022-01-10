using CyberSaloon.Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using CyberSaloon.Client.Pages.Applications.DTO;
using System.Collections.Generic;

namespace CyberSaloon.Client.Pages.Applications.Pages
{
    public partial class ApplicationsPage : ComponentBase
    {
        public ApplicationGetDTO[] applications = Array.Empty<ApplicationGetDTO>();

        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public int CurrentPage { get; set; } = 1;

        [Parameter]
        public int TotalPages { get; set; } = 1;

        [Parameter]
        public string ErrorMessage { get; set; } = "There is no application found.";

        [Parameter]
        public bool IsLoading { get; set; } = false; 

        [Parameter]
        public bool IsFailed { get; set; } = false;

        [Inject]
        public NavigationManager Navigation { get; init; }

        [Inject]
        public ICoreAPIClient Client { get; init; }

        public int MaxPages { get; set; } = 9;
        public int MaxPagesBeforeCurrentPage { get => (int)Math.Floor((decimal) MaxPages / (decimal)2); }
        public int MaxPagesAfterCurrentPage  { get => (int)Math.Ceiling((decimal) MaxPages / (decimal)2) - 1; }

        protected async override Task OnParametersSetAsync()
        {
            IsFailed = false;

            if(CurrentPage < 1)
                CurrentPage = 1;
                
            ResourceParameters parameters = 
                new()
                {
                    PageNumber = CurrentPage 
                };

            IsLoading = true;
            var response = await Client.GetApplicationsAsync(parameters, Action, Id);
            IsLoading = false;

            applications = 
                response?
                    .Entities?
                    .ToArray() ?? 
                        Array.Empty<ApplicationGetDTO>();

            if (applications == null || !applications.Any())
            {
                IsFailed = true;
                return;
            }

            StateHasChanged();

            TotalPages = response.TotalPages;
        }

        protected string GeneratePaginationLink(int index)
        {
            var bits = Navigation.Uri.Split('/');
            List<string> path = new(bits);
            var lastBit = path.Last();

            var isInteger = Int16.TryParse(lastBit, out _);
            
            if(isInteger || path.Last() == string.Empty)
                path[path.Count - 1] = index.ToString();
            else
                path.Add(index.ToString());

            return path.Aggregate((left, right) => $"{left}/{right}");
        }

        protected void FlipPage(int pageNumber)
        {
            if(CurrentPage != pageNumber)
                CurrentPage = pageNumber;

            OnParametersSetAsync().ConfigureAwait(false);

            Navigation.NavigateTo(Navigation.Uri, forceLoad: false);
        }
    }
}
