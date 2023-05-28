using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.extension;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Services
{
    public interface ICurrentUserService
    {
        CurrentUserDto User { get; set; }
        Guid LanguageId { get; set; }
        Guid SystemTypeId { get; set; }
        bool IsAuthorizedFor(params FunctionType[] functions);
        bool IsRoot();

        bool HasFeatures(IEnumerable<int> features);
        bool HasFeatures(params int[] features);

        IEnumerable<Guid> AllOrganizationIds { get; }
    }


    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserDto User { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }

        public IEnumerable<Guid> AllOrganizationIds
        {
            get
            {
                if (User == null)
                    return Enumerable.Empty<Guid>();

                if (User.Organizations == null)
                    return
                       new[] { User.OrganizationId };

                return User.Organizations.Select(x => x.Id).
                    Concat(new[] { User.OrganizationId }).Distinct();
            }
        }

        public bool IsAuthorizedFor(params FunctionType[] functions)
        {
            try
            {
                if (User == null)
                {
                    return false;
                }
                return User.RootUser || User.GetFunctions().ContainsAtLeastOne(functions);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return true;
        }

        public bool IsRoot()
        {
            if (User == null)
            {
                return false;
            }
            return User.RootUser;
        }

        public bool HasFeatures(IEnumerable<int> features)
        {
            return HasFeatures(features.ToArray());
        }

        public bool HasFeatures(params int[] features)
        {
            if (User?.Features == null)
            {
                return false;
            }

            return User.RootUser || features.AllMatching(User.Features);
        }

    }
}