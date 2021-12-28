using System;
using Microsoft.AspNetCore.Components;

namespace CyberSaloon.Client.Pages.Common
{
    public partial class PaginationComponent : ComponentBase
    {
        [Parameter]
        public int TotalPages { get; set; }
        
        [Parameter]
        public int CurrentPage { get; set; }

        [Parameter]
        public string PaginationUri { get; set; }

        public string GeneratePaginationUri(int index)
        {
            var builder = new UriBuilder(PaginationUri); 
            builder.Query = $"page={index}";
            return builder.ToString();
        }
    }
}