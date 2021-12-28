using Microsoft.AspNetCore.Components;

namespace CyberSaloon.Client.Pages
{
    public partial class SubmittedPage : ComponentBase 
    {
        [Parameter]
        public int CurrentPage { get; set; } = 1;
    }
}