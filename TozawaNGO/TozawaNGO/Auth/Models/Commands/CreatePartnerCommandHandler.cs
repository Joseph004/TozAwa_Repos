using MediatR;
using TozawaNGO.Context;
using TozawaNGO.Auth.Models.Converters;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos.Backend;

namespace TozawaNGO.Auth.Models.Commands
{
    public class CreatePartnerCommandHandler(TozawangoDbContext context) : IRequestHandler<CreatePartnerCommand, PartnerDto>
    {
        private readonly TozawangoDbContext _context = context;

        public async Task<PartnerDto> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
        {
            var newPartner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Adress = request.Adress
            };
            _context.Partners.Add(newPartner);
            _context.SaveChanges();

            var response = PartnerConverter.Convert(newPartner);

            return await Task.FromResult(response);

        }
    }
}