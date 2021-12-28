using Client.Pages.Common.DTO;
using CyberSaloon.Client.Pages.Applicants.DTO;
using CyberSaloon.Client.Pages.Applications.DTO;
using CyberSaloon.Client.Pages.Artists.DTO;
using CyberSaloon.Client.Pages.Arts.DTO;
using CyberSaloon.Client.Pages.Common.DTO;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace CyberSaloon.Client.Pages.Common
{
    public interface ICoreAPIClient
    {
        #region Applicants
        Task<ApplicantGetDTO> GetApplicantAsync();
        Task<ApplicantGetDTO> GetApplicantAsync(Guid applicantId);
        Task<ApplicantGetDTO> GetApplicantAsync(string applicantId);
        Task<Pagination<ApplicantGetDTO>> GetApplicantsAsync(ResourceParameters parameters);
        Task<ApplicantGetDTO> PostApplicantAsync(string Alias);
        
        #endregion

        #region Applications
        Task<ApplicationGetDTO> GetApplicationAsync(Guid ApplicationId);
        Task<Pagination<ApplicationGetDTO>> GetApplicationsAsync(
                ResourceParameters parameters,
                string action = default,
                string id = default
            );
        Task<ApplicationGetDTO> PostApplicationAsync(ApplicationPostDTO application);
        Task<bool> PatchApplicationAsync(Guid applicationId, JsonPatchDocument payload);
        Task<bool> DeleteApplicationAsync(Guid applicationId);

        Task GetApplyAsync(Guid applicationId);
        Task GetDefyAsync(Guid applicationId);
        #endregion

        #region Artists
        Task<ArtistGetDTO> GetArtistAsync();
        Task<ArtistGetDTO> GetArtistAsync(Guid ArtistId);
        Task<ArtistGetDTO> GetArtistAsync(string ArtistAlias);
        Task<Pagination<ArtistGetDTO>> GetArtistsAsync(ResourceParameters parameters);
        Task<ArtistGetDTO> PostArtistAsync(string Alias);
        #endregion

        #region Arts
        Task<ArtGetDTO> GetArtAsync(Guid ArtId);
        Task<Pagination<ArtGetDTO>> GetArtsAsync(ResourceParameters parameters);
        Task<ArtGetDTO> PostArtAsync(ArtPostDTO art);
        Task<bool> PatchArtAsync(Guid artId, JsonPatchDocument payload);
        Task<bool> DeleteArtAsync(Guid artId);

        #endregion
    }
}