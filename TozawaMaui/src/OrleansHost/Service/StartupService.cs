using Grains;
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
        await SetUsers(factory, context);
        await SetTranslations(factory, context);
        await SetAttachments(factory, context, googleService);

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

    private async Task SetUsers(IGrainFactory factory, TozawangoDbContext context)
    {
        var items = await context.TzUsers.ToListAsync();

        foreach (var memberItem in items)
        {
            var item = new MemberItem(
        memberItem.UserId,
      memberItem.PartnerId,
      memberItem.Description,
     memberItem.DescriptionTextId,
      memberItem.FirstName,
      memberItem.LastName,
      memberItem.LastLoginCountry,
      memberItem.LastLoginCity,
      memberItem.LastLoginState,
      memberItem.LastLoginIPAdress,
      memberItem.Adress,
      memberItem.UserPasswordHash,
      memberItem.Roles,
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
      SystemTextId.MemberOwnerId
            );
            await factory.GetGrain<IMemberGrain>(item.UserId).ActivateAsync(item);
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

    private async Task SetAttachments(IGrainFactory factory, TozawangoDbContext context, IGoogleService googleService)
    {
        var items = await context.FileAttachments.Include(x => x.Owners).ToListAsync();

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
               SystemTextId.AttachmentOwnerId,
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