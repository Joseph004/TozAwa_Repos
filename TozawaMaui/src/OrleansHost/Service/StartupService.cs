using Grains;
using Grains.Auth.Models.Authentication;
using Grains.Context;
using Grains.Helpers;
using Grains.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrleansHost.Service;

public class StartupService(IServiceProvider services) : IHostedService
{
    private IServiceProvider _services = services;

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var scope = _services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IGrainFactory>();
        var context = scope.ServiceProvider.GetRequiredService<TozawangoDbContext>();
        var googleService = scope.ServiceProvider.GetRequiredService<IGoogleService>();

        await SetTodos(factory);
        await SetUsers(factory, context, googleService);
        await SetOrganizations(factory, context, googleService);
        await SetTranslations(factory, context);
        await SetFeatures(factory, context);
        //await SetAttachments(factory, context, googleService);

        await Task.CompletedTask;
    }
    private async Task SetTodos(IGrainFactory factory)
    {
        var items = new List<TodoItem>{
                new(Guid.NewGuid(), "Test one", false, SystemTextId.ToDoOwnerId),
                new(Guid.NewGuid(), "Test two", false, SystemTextId.ToDoOwnerId),
                new(Guid.NewGuid(), "Test three", false, SystemTextId.ToDoOwnerId)
            };

        foreach (var item in items)
        {
            await factory.GetGrain<ITodoGrain>(item.Key).ActivateAsync(item);
        }
    }

    private async Task SetUsers(IGrainFactory factory, TozawangoDbContext context, IGoogleService googleService)
    {
        var items = await context.TzUsers
                    .Include(t => t.UserOrganizations)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.OrganizationUsers)
                    .Include(y => y.Organizations)
                    .ThenInclude(u => u.Features)
                    .Include(w => w.Addresses)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role)
                    .Include(u => u.Roles)
                    .ThenInclude(y => y.Role.Functions)
                    .ToListAsync();

        foreach (var memberItem in items)
        {
            var attachmentsCount = await context.FileAttachments.Include(t => t.Owners).CountAsync(x => x.Owners.Any(y => y.OwnerId == memberItem.UserId));
            var item = new MemberItem(
            memberItem.UserId,
            memberItem.Description,
            memberItem.DescriptionTextId,
            memberItem.FirstName,
            memberItem.LastName,
            memberItem.LastLoginCountry,
            memberItem.LastLoginCity,
            memberItem.LastLoginState,
            memberItem.LastLoginIPAdress,
            memberItem.Roles.Select(x => x.Role != null ? x.Role.RoleEnum : RoleEnum.None).ToList(),
            memberItem.LastAttemptLogin,
            memberItem.RefreshToken,
            memberItem.RefreshTokenExpiryTime,
            memberItem.UserCountry,
            memberItem.Deleted,
            memberItem.AdminMember,
            memberItem.LastLogin,
            memberItem.CreatedBy,
            memberItem.CreateDate,
            memberItem.ModifiedBy,
            memberItem.ModifiedDate,
            memberItem.StationIds,
            memberItem.Email,
            memberItem.PasswordHash,
            attachmentsCount,
            memberItem.Tenants != null ?
             memberItem.Tenants.Select(x => x.UserTenant_TenantUser.UserId).ToList() : [],
            memberItem.LandLords != null ?
             memberItem.LandLords?.Select(x => x.UserLandLord_LandLordUser.UserId).ToList() : [],
            memberItem.Organizations?.SelectMany(o => o.Features) != null
            ? memberItem.Organizations?.SelectMany(o => o.Features).Select(x => x.Feature).ToList()
            : [],
            memberItem.Roles
                .SelectMany(x => x.Role.Functions)
                .Select(function => function.FunctionType)
                .Distinct()
                .ToList(),
            memberItem.Comment,
            memberItem.CommentTextId,
            memberItem.UserOrganizations.Select(x => x.OrganizationId).ToList(),
            memberItem.CityCode,
            memberItem.CountryCode,
            memberItem.Gender,
            memberItem.UserOrganizations.First(o => o.PrimaryOrganization).OrganizationId
            );
            var emailGuid = new EmailGuidItem(
            memberItem.UserId,
            memberItem.Email,
            SystemTextId.EmailOwnerId
            );
            await factory.GetGrain<IMemberGrain>(item.UserId).ActivateAsync(item);
            await factory.GetGrain<IEmailGuidGrain>(emailGuid.Email).ActivateAsync(emailGuid);
            await SetUserAddresses(factory, context, item.UserId);
            await SetAttachments(factory, context, googleService, memberItem.UserId);
        }
        await SetUserRoles(factory, context);
    }
    private async Task SetOrganizations(IGrainFactory factory, TozawangoDbContext context, IGoogleService googleService)
    {
        var items = await context.Organizations.Include(u => u.Features).Include(t => t.Addresses).Include(u => u.OrganizationUsers).Include(t => t.Addresses).Include(u => u.Roles).ToListAsync();
        foreach (var organization in items)
        {
            var attachmentsCount = await context.FileAttachments.Include(t => t.Owners).CountAsync(x => x.Owners.Any(y => y.OwnerId == organization.Id));
            var item = new OrganizationItem(
            organization.Id,
            organization.Name,
            organization.City,
            organization.Email,
            organization.PhoneNumber,
            organization.CreatedBy,
            organization.CreateDate,
            organization.ModifiedBy,
            organization.ModifiedDate,
            organization.Features.Select(x => x.Feature).ToList(),
            attachmentsCount,
            SystemTextId.OrganizationOwnerId,
            organization.Comment,
            organization.CommentTextId,
            organization.Description,
            organization.DescriptionTextId,
            organization.OrganizationUsers.Select(x => x.UserId).ToList(),
            organization.CityCode,
            organization.CountryCode,
            organization.City,
            organization.Deleted
            );
            await factory.GetGrain<IOrganizationGrain>(item.Id).ActivateAsync(item);
            await SetOrganizationAddresses(factory, context, item.Id);

            organization.Roles?.Select(async y =>
            {
                var roleItem = new RoleItem
                (
                y.Id,
                y.OrganizationId,
                y.RoleEnum,
                y.Functions.Select(x => x.FunctionType).ToList(),
                organization.Id
                );
                await factory.GetGrain<IRoleGrain>(roleItem.Id).ActivateAsync(roleItem);
            });

            await SetAttachments(factory, context, googleService, organization.Id);
        }
    }
    private async Task SetFeatures(IGrainFactory factory, TozawangoDbContext context)
    {
        var items = await context.TozawaFeatures.ToListAsync();
        foreach (var feature in items)
        {
            var item = new FeatureItem(
            feature.Id,
            feature.TextId,
            feature.DescriptionTextId,
            feature.Deleted,
            SystemTextId.FeatureOwnerId
            );
            await factory.GetGrain<IFeatureGrain>(item.TextId).ActivateAsync(item);
        }
    }
    private async Task SetUserRoles(IGrainFactory factory, TozawangoDbContext context)
    {
        var items = await context.TzUserRoles.ToListAsync();
        var roles = await context.TzRoles.ToListAsync();
        foreach (var userRole in items)
        {
            var role = roles.First(x => x.Id == userRole.RoleId);
            var item = new RoleItem(
            role.Id,
            role.OrganizationId,
            role.RoleEnum,
            role.Functions.Select(x => x.FunctionType).ToList(),
            userRole.UserId
            );
            await factory.GetGrain<IRoleGrain>(item.Id).ActivateAsync(item);
        }
    }
    private async Task SetUserAddresses(IGrainFactory factory, TozawangoDbContext context, Guid ownerId)
    {
        var items = await context.UserAddresses.ToListAsync();
        foreach (var address in items)
        {
            var item = new AddressItem(
            address.Id,
            address.Name,
            address.Address,
            address.City,
            address.State,
            address.Country,
            address.ZipCode,
            address.Active,
            ownerId
            );
            await factory.GetGrain<IAddressGrain>(item.Id).ActivateAsync(item);
        }
    }
    private async Task SetOrganizationAddresses(IGrainFactory factory, TozawangoDbContext context, Guid ownerId)
    {
        var items = await context.OrganizationAddresses.ToListAsync();

        foreach (var address in items)
        {
            var item = new AddressItem(
            address.Id,
            address.Name,
            address.Address,
            address.City,
            address.State,
            address.Country,
            address.ZipCode,
            address.Active,
            ownerId
            );
            await factory.GetGrain<IAddressGrain>(item.Id).ActivateAsync(item);
        }
    }
    private async Task SetTranslations(IGrainFactory factory, TozawangoDbContext context)
    {
        var items = await context.Translations.ToListAsync();

        foreach (var translation in items)
        {
            var item = new TranslationItem(translation.Id, translation.TextId, translation.LanguageText, SystemTextId.TranslationOwnerId,
                       translation.CreatedBy, translation.CreateDate, translation.ModifiedBy, translation.ModifiedDate);
            await factory.GetGrain<ITranslationGrain>(item.TextId).ActivateAsync(item);
        }
    }

    private async Task SetAttachments(IGrainFactory factory, TozawangoDbContext context, IGoogleService googleService, Guid ownerId)
    {
        var items = await context.FileAttachments.Include(x => x.Owners).Where(t => t.Owners.Any(y => y.OwnerId == ownerId)).ToListAsync();
        foreach (var item in items)
        {
            var thumbnail = string.Empty;
            var miniatureBlobUrl = string.Empty;

            if (!string.IsNullOrEmpty(item.MiniatureId) && item.Owners.Count > 0)
            {
                var stream = await googleService.StreamFromGoogleFileByFolder(item.Owners.Select(x => x.OwnerId).First().ToString(), item.MiniatureId);
                var bytes = FileUtil.ReadAllBytesFromStream(stream);
                if (bytes != null)
                {
                    thumbnail = Convert.ToBase64String(bytes);
                    miniatureBlobUrl = Convert.ToBase64String(bytes);
                }
            }
            var attach = new AttachmentItem(
               item.Id,
               item.CreatedDate,
               item.ModifiedDate,
               item.ModifiedBy,
               item.CreatedBy,
               ownerId,
               item.BlobId,
               item.MiniatureId,
               item.Name,
               item.Extension,
               item.MimeType,
               item.Size,
               item.AttachmentType,
               item.FileAttachmentType,
               item.MetaData,
               item.Owners.Select(x => x.OwnerId).ToList(),
               thumbnail,
               miniatureBlobUrl);
            await factory.GetGrain<IAttachmentGrain>(item.Id).ActivateAsync(attach);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        // The code in here will run when the application stops
        // In your case, nothing to do
        return Task.CompletedTask;
    }
}