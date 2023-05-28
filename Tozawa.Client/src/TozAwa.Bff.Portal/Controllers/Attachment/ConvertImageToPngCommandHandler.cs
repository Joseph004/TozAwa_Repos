using MediatR;
using Tozawa.Bff.Portal.Converters;

namespace Tozawa.Bff.Portal.Controllers
{

    public class ConvertImageToPngCommandHandler : IRequestHandler<ConvertImageToPngCommand, byte[]>
    {
        public async Task<byte[]> Handle(ConvertImageToPngCommand request, CancellationToken cancellationToken)
        {
            return await FormFileConverter.ImageToPng(request.File);
        }
    }
}