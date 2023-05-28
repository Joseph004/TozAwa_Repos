using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Dtos;

namespace Tozawa.Auth.Svc.Models.Converters
{
    public interface ICurrentUserConverter
    {
        CurrentUserDto Convert(ApplicationUser user);
    }

    public class CurrentUserConverter : ICurrentUserConverter
    {

        public CurrentUserConverter()
        {
        }

        public CurrentUserDto Convert(ApplicationUser user)
        {
            var workingOrganzation = user.Organizations.FirstOrDefault(x => x.Id == user.WorkingOrganizationId);
            if (workingOrganzation == null)
            {
                workingOrganzation = user.Organizations.FirstOrDefault(x => x.Id == user.OrganizationId);
            }

            if (workingOrganzation == null || workingOrganzation.Id == Guid.Empty)
            {
                throw new ArgumentNullException();
            }

            return new()
            {
                Id = user.UserId,
                Oid = user.Oid,
                RootUser = user.RootUser,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Organization = workingOrganzation.Name,
                OrganizationId = workingOrganzation.Id,
                FallbackLanguageId = workingOrganzation.FallbackLanguageId,
                OrganizationLanguageIds = user.Organization.LanguageIds,
                ExportLanguageIds = user.Organization.ExportLanguageIds,
                Features = workingOrganzation.Features.Select(x => x.Feature).ToList(),
                Organizations = user.Organizations.Select(x => new Models.Dtos.CurrentUserOrganizationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.Id == user.WorkingOrganizationId
                }).ToList(),
                Roles = user.Roles.Where(x => x.OrganizationId == workingOrganzation.Id && x.Active).Select(y => new CurrentUserRoleDto
                {
                    TextId = y.Role.TextId
                }).ToList(),
                Functions = user.Roles.Where(x => x.OrganizationId == workingOrganzation.Id).SelectMany(x => x.Role.Functions).Select(x => x.Functiontype).Distinct().Select(x => new CurrentUserFunctionDto
                {
                    FunctionType = x,
                    TextId = x
                }).ToList(),
                IsFederatedUser = workingOrganzation.IsFederated
            };

        }
    }
}