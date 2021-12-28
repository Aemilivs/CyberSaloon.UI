using Microsoft.AspNetCore.Components;

namespace CyberSaloon.Client.Pages
{
    public partial class LikedPage : ComponentBase 
    {
        [Parameter]
        public int CurrentPage { get; set; } = 1;
    }
}