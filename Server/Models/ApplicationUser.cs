using Microsoft.AspNetCore.Identity;
using System;

namespace CyberSaloon.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid ApplicantId { get; set; }
        public Guid ArtistId { get; set; }
    }
}
