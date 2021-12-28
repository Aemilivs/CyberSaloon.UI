using System.Collections.Generic;
using CyberSaloon.Client.Pages.Applications.DTO;

namespace Client.Pages.Applications.DTO
{
    public class GetApplicationsResponse
    {
        public IEnumerable<ApplicationGetDTO> Applications { get; set; }
        public int TotalPages { get; set; } 
    }
}