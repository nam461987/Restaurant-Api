using Newtonsoft.Json;
using Restaurant.Common.Dtos.AdminAccount;
using System.Linq;
using System.Security.Claims;

namespace Restaurant.API.Extensions
{
    public static class ClaimPrincipalExtension
    {
        public static AuthenticationDto ToAuthenticationDto(this ClaimsPrincipal source)
           => JsonConvert.DeserializeObject<AuthenticationDto>(source
               .Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value);
    }
}
