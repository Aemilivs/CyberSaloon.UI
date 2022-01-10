using CyberSaloon.Client.Pages.Applications.DTO;
using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Applications.Pages
{
    public partial class ApplicationPage : ComponentBase
    {
        [Parameter]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Flag describing if this application page is rendering standalone application
        /// or an application within a list of applications.
        /// </summary>
        /// <value>True by default.</value>
        [Parameter]
        public bool IsStandalone { get; set; } = true;

        [Parameter]
        public ApplicationGetDTO ApplicationDTO { get; set; } = default;

        [Inject]
        public ICoreAPIClient Client { get; init; }

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        [Inject]
        public NavigationManager Navigation { get; init; }

        [Parameter]
        public bool WasLiked { get; set; } = false;

        [Parameter]
        public bool ShouldLike { get; set; } = false;

        [Parameter]
        public bool ApplicationIsDedicatedToClient { get; set; } = false;
        
        [Parameter]
        public bool ApplicationIsSubmitted { get; set; } = false;
        
        [Parameter]
        public bool ApplicationIsLiked { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            var result = Guid.TryParse(ApplicationId, out var id);

            if (!result)
            {
                if(ApplicationDTO != default)
                    ApplicationId = ApplicationDTO.Id.ToString();
                
                ApplicationIsLiked = await ApplicationIsLikedAsync();
                ApplicationIsDedicatedToClient = await ApplicationIsDedicatedToClientAsync();
                ApplicationIsSubmitted = await ApplicationIsSubmittedAsync();
                
                return;
            }

            
            if (ApplicationDTO != default && ApplicationDTO.Id == id)
                return;

            ApplicationDTO = await Client.GetApplicationAsync(id);
            ApplicationIsLiked = await ApplicationIsLikedAsync();

            var uri = Navigation.ToAbsoluteUri(Navigation.Uri); 
            var shouldLikeIsPresent = 
                QueryHelpers
                    .ParseQuery(uri.Query)
                    .TryGetValue("shouldLike", out var shouldLikeRaw);
            if (shouldLikeIsPresent)  
            {  
                await ApplyApplicationAsync(ApplicationDTO.Id);
            } 

            ApplicationIsDedicatedToClient = await ApplicationIsDedicatedToClientAsync();
            ApplicationIsSubmitted = await ApplicationIsSubmittedAsync();
        }
        
        private async Task<bool> ApplicationIsDedicatedToClientAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();

            if (state == null)
                return false;

            var user = state.User;

            if(user == default)
                return false;

            if (!user.Claims.Any())
                return false;

            var username =
                 user
                     .Claims
                     .Single(it => it.Type == "username")
                     .Value;

            var namesAreEqual = 
                string.Equals(
                        ApplicationDTO.Artist, 
                        username,
                        StringComparison.InvariantCulture
                    ); 

            if(namesAreEqual)
                return true;

            return false;
        }

        private string ProcessDescription()
        {
            if(IsStandalone)
                return ApplicationDTO.Description;

            var threshold = 200;

            if(ApplicationDTO.Description.Length <= threshold)
                return ApplicationDTO.Description;

            return 
                String.Join(
                        string.Empty, 
                        ApplicationDTO
                            .Description
                            .Substring(0, threshold - 3), 
                        "..."
                    );
        }

        private async Task<bool> ApplicationIsSubmittedAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();

            if (state == null)
                return false;

            var user = state.User;

            if(user == default)
                return false;

            if (!user.Claims.Any())
                return false;

            var username =
                 user
                     .Claims
                     .Single(it => it.Type == "username")
                     .Value;

            var namesAreEqual = 
                string.Equals(
                        ApplicationDTO.Author, 
                        username,
                        StringComparison.InvariantCulture
                    ); 

            if(namesAreEqual)
                return true;

            return false;
        }

        private async Task<bool> ApplicationIsLikedAsync()
        {
            if(WasLiked)
                return true;

            var applicantId = await GetApplicantIdAsync();

            if(applicantId == Guid.Empty)
                return false;

            return 
                ApplicationDTO
                    .Applicants
                    .Any(it => it == applicantId);
        }

        public async Task OnLikeAsync()
        {
            var guid = 
                ApplicationId == default ? 
                    ApplicationDTO.Id : 
                    Guid.Parse(ApplicationId);
            
            if(await ApplicationIsLikedAsync())
            {
                await Client.GetDefyAsync(guid);
                ApplicationDTO.Applicants.Remove(guid);
            }
            else
            {
                await Client.GetApplyAsync(guid);
                ApplicationDTO.Applicants.Add(guid);
            }

            WasLiked = !WasLiked;
            ApplicationIsLiked = !ApplicationIsLiked;
        }

        private async Task ApplyApplicationAsync(Guid applicationId)
        {
            await Client.GetApplyAsync(applicationId);

            var applicantId = await GetApplicantIdAsync();

            if (applicantId == Guid.Empty)
                return;

            ApplicationDTO
                .Applicants
                .Add(applicantId);
        }

        private async Task DefyApplicationAsync(Guid applicationId)
        {
            await Client.GetDefyAsync(applicationId);

            var applicantId = await GetApplicantIdAsync();

            if (applicantId == Guid.Empty)
                return;

            ApplicationDTO
                    .Applicants
                    .Remove(applicantId);
        }

        private async Task DeleteApplicationAsync()
        {
            await Client.DeleteApplicationAsync(ApplicationDTO.Id);
            
            ApplicationDTO = default;

            if(IsStandalone)
                Navigation.NavigateTo("/");
            else
                Navigation.NavigateTo(Navigation.Uri, forceLoad: false);
        }

        private async Task<Guid> GetApplicantIdAsync()
        {
            var state = await Provider.GetAuthenticationStateAsync();
            var user = state.User;

            if(user == default)
                return Guid.Empty;

            if (!user.Claims.Any())
                return Guid.Empty;

            var raw =
                 user
                     .Claims
                     .Single(it => it.Type == "sub")
                     .Value;
            
            var isGuid = Guid.TryParse(raw, out var applicantId);
            
            if(!isGuid || applicantId == Guid.Empty)
                return (await Client.GetApplicantAsync()).Id;

            return applicantId;
        } 

        private string BuildLikeCountMessage()
        {
            var count = ApplicationDTO.Applicants.Count();
            var subject = 
                count == 1 ? 
                    "applicant" : 
                    "applicants";
            return String.Join(' ', count, subject, "liked this application.");
        }

        private string ParseApplicationStatus(ApplicationGetDTO application) => 
            application?.Art == Guid.Empty ? 
                "unfullfiled" : 
                "fullfiled" ?? 
                    "unfullfiled";
    }
}
