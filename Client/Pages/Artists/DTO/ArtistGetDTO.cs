using System;
using System.Collections.Generic;

namespace CyberSaloon.Client.Pages.Artists.DTO
{
    public class ArtistGetDTO
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public virtual ICollection<Guid> Applications { get; set; } = new List<Guid>();
    }
}
