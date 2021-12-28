using CyberSaloon.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages
{
    public partial class RecentlyPostedPage : ComponentBase
    {
        
        [Parameter]
        public int CurrentPage { get; set; } = 1;

        [Inject]
        public AuthenticationStateProvider Provider { get; init; }

        [Inject]
        public NavigationManager Manager { get; init; }
        
        [Inject]
        public SessionStorage Storage { get; init; }

        public const string Key = "_is_initialized";

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var state = await Provider.GetAuthenticationStateAsync();

            if (!state.User.Claims.Any())
                return;

            var name = 
                state
                    .User?
                    .Identity?
                    .Name;

            var userIsInitialized = 
                await UserIsInitialized() || 
                await Storage.GetValue<string>($"{name}{Key}") == "true";

            if (userIsInitialized)
                return;
            
            await Storage.SetValue<string>($"{name}{Key}", "true");
            Manager.NavigateTo("/welcome/");
        }

        public async Task<bool> UserIsInitialized()
        {
            var state = await Provider.GetAuthenticationStateAsync();
            var user = state.User;

            var applicantId =
                user
                    .Claims
                    .Single(it => it.Type == "applicantId")
                    .Value;

            var applicantIsMissing = Guid.Parse(applicantId) == Guid.Empty;

            var artistId =
                 user
                     .Claims
                     .Single(it => it.Type == "artistId")
                     .Value;

            var artistIsMissing = Guid.Parse(artistId) == default;

            if (applicantIsMissing || artistIsMissing)
                return false;

            return true;
        }
    }
}
