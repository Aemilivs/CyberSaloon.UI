using System;

namespace CyberSaloon.Client.Pages.Arts.DTO
{
    public class ArtPostDTO
    {
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Guid Application { get; set; }
    }
}
