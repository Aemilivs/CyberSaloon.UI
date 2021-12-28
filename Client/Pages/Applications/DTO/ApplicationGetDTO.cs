using System;
using System.Collections.Generic;

namespace CyberSaloon.Client.Pages.Applications.DTO
{
    public class ApplicationGetDTO
    {
        public Guid Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Fullfilled { get; set; } = false;
        public string Artist { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public Guid Art { get; set; }
        public IList<Guid> Applicants { get; set; }
    }
}
