using System;

namespace CyberSaloon.Client.Pages.Arts.DTO
{
    public class ArtPatchDTO
    {
        public Guid Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
