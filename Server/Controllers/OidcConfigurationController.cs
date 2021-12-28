using System;
using System.Linq;
using System.Threading.Tasks;
using CyberSaloon.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CyberSaloon.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly ILogger<OidcConfigurationController> _logger;

        public OidcConfigurationController(
                IClientRequestParametersProvider clientRequestParametersProvider, 
                ILogger<OidcConfigurationController> logger,
                UserManager<ApplicationUser> manager
            )
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            _manager = manager;
            _logger = logger;
        }

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);
        }

        [DisableCors]
        [HttpPost("_configuration/update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO dto)
        {
            var name = 
                    User
                        .Claims
                        .Single(it => it.Type == "username")
                        .Value;
            var user = await _manager.FindByNameAsync(name).ConfigureAwait(false);

            if(user == null)
                return NotFound();

            user.ApplicantId = dto.ApplicantId;
            user.ArtistId = dto.ArtistId;

            await _manager.UpdateAsync(user).ConfigureAwait(false);
            
            return Ok();
        }

        public record UpdateUserDTO(Guid ArtistId, Guid ApplicantId);

        [HttpOptions("_configuration/update")]
        public ActionResult OidcConfigurationOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, OPTIONS");
            return Ok();
        }
    }
}
