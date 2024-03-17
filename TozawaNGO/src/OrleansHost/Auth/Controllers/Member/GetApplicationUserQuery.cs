
using Grains.Auth.Models.Authentication;
using MediatR;

namespace Grains.Auth.Controllers
{
    public class GetApplicationUserQuery(string email) : IRequest<ApplicationUser>
    {
        public string Email { get; set; } = email;
    }
}