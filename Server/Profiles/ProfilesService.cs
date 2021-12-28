using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CyberSaloon.Server.Models;
using Microsoft.Extensions.Configuration;

namespace Server.Profiles
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly ITokenCreationService _tokenCreationService;
        protected readonly IConfiguration _configuration;
        
        public ProfileService(
                UserManager<ApplicationUser> userManager, 
                IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                ITokenCreationService tokenCreationService,
                IConfiguration configuration
            )
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _tokenCreationService = tokenCreationService;
            _configuration = configuration;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = 
                claims
                    .Where(claim => context.RequestedClaimTypes.Contains(claim.Type))
                    .ToList();
            claims.Add(new Claim("username", user.UserName));
            claims.Add(new Claim("applicantId", user.ApplicantId.ToString()));
            claims.Add(new Claim("artistId", user.ArtistId.ToString()));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}