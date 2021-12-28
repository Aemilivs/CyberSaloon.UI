using System;

namespace CyberSaloon.Client.Pages.Applicants.DTO
{
    public class ApplicantPostDTO
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
    }
}