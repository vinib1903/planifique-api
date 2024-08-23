using System.Security.Claims;

namespace PlanifiqueAPI.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}