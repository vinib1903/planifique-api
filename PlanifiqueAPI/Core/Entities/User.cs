using Microsoft.AspNetCore.Identity;

namespace PlanifiqueAPI.Core.Entities
{
    public class User : IdentityUser
    {
        public string Nome { get; set; }
        public string FotoPerfil { get; set; }
    }
}
