using System.Threading.Tasks;
using Client.Pages.Common.DTO;

namespace CyberSaloon.Client.Pages.Common
{
    public interface IIdentityServerAPIClient
    {
        Task UpdateUserAsync(UpdateUserDTO dto);
    }
}