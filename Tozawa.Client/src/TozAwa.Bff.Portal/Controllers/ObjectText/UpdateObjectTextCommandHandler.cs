

using System.Net;
using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.NotificationHandlers;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers;

public partial class UpdateObjectTextCommandHandler : IRequestHandler<UpdateObjectTextCommand, UpdateResponse>
{
    private readonly ILanguageService _languageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateObjectTextCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly AppSettings _appSettings;

    public UpdateObjectTextCommandHandler(ILanguageService languageService, ICurrentUserService currentUserService, ILogger<UpdateObjectTextCommandHandler> logger, IMediator mediator, AppSettings appSettings)
    {
        _languageService = languageService;
        _currentUserService = currentUserService;
        _logger = logger;
        _mediator = mediator;
        _appSettings = appSettings;
    }
    public async Task<UpdateResponse> Handle(UpdateObjectTextCommand request, CancellationToken cancellationToken)
    {
        var languageId = _currentUserService.LanguageId == Guid.Empty ? Guid.Parse("ea3ff133-cd1b-42d7-9d42-0aeee6b731c8") : _currentUserService.LanguageId;

        var notification = new EntityUpdatedNotification(request.Id, request.EntityType, _currentUserService.User.UserName);
        var updateTextIdNotification = new TextIdAddedNotification(request.EntityType, request.Id);

        if (request.TextId.HasValue)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                throw new ArgumentException(request.Text);
            }
            try
            {
                if (request.TextId.HasValue && !Guid.Empty.Equals(request.TextId.Value))
                {
                    await _languageService.UpdateText(new TranslationUpdateDto
                    {
                        Id = request.TextId.Value,
                        LanguageId = languageId,
                        Text = request.Text
                    });
                    notification.UpdatedProperties.Name = request.Text;
                    notification.UpdatedProperties.NameTextId = request.TextId;
                }
                else
                {
                    var addedText = await _languageService.Import(new ImportTranslationDto { Original = new ImportTranslationTextDto { LanguageId = languageId, Text = request.Text }, SystemTypeId = _appSettings.SystemTextGuid, Translations = new List<ImportTranslationTextDto>() });
                    notification.UpdatedProperties.Name = request.Text;
                    notification.UpdatedProperties.NameTextId = request.TextId;

                    updateTextIdNotification.NameTextId = addedText.TextId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update text for entitytype {entityType} with id {id} textid {textid} text {text} exception {exception}", request.EntityType, request.Id, request.TextId, request.Text, ex.Message);
                return new UpdateResponse(false, UpdateMessages.Error, HttpStatusCode.InternalServerError);
            }
        }
        try
        {
            if (!string.IsNullOrEmpty(request.Description))
            {
                if (request.DescriptionTextId.HasValue && !Guid.Empty.Equals(request.DescriptionTextId.Value))
                {
                    await _languageService.UpdateText(new TranslationUpdateDto
                    {
                        Id = request.DescriptionTextId.Value,
                        LanguageId = languageId,
                        Text = request.Description
                    });
                    notification.UpdatedProperties.Description = request.Description;
                    notification.UpdatedProperties.DescriptionTextId = request.DescriptionTextId.Value;
                }
                else
                {
                    var addedText = await _languageService.Import(new ImportTranslationDto { Original = new ImportTranslationTextDto { LanguageId = languageId, Text = request.Description }, SystemTypeId = _appSettings.SystemTextGuid, Translations = new List<ImportTranslationTextDto>() });
                    notification.UpdatedProperties.Description = request.Description;
                    notification.UpdatedProperties.DescriptionTextId = addedText.TextId;

                    updateTextIdNotification.DescriptionTextId = addedText.TextId;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update description text for entitytype {entityType} with id {id} textid {textid} text {text} exception {exception}", request.EntityType, request.Id, request.TextId, request.Text, ex.Message);
            return new UpdateResponse(false, UpdateMessages.Error, HttpStatusCode.InternalServerError);
        }

        await _mediator.Publish(notification, cancellationToken);

        await _mediator.Publish(updateTextIdNotification, CancellationToken.None);

        return new UpdateResponse(true, UpdateMessages.Success, HttpStatusCode.OK);
    }
}