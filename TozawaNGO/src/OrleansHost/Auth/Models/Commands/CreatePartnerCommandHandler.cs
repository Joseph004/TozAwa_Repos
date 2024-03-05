using MediatR;
using OrleansHost.Context;
using OrleansHost.Auth.Models.Converters;
using OrleansHost.Auth.Models.Authentication;
using OrleansHost.Auth.Models.Dtos.Backend;

namespace OrleansHost.Auth.Models.Commands
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