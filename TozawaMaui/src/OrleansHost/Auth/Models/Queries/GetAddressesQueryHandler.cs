using System.Buffers;
using System.Collections.Immutable;
using Grains;
using Grains.Auth.Models.Dtos;
using MediatR;

namespace OrleansHost.Auth.Models.Queries;

public class GetAddressesQuery(Guid ownerId) : IRequest<IEnumerable<AddressDto>>
{
    public Guid OwnerId { get; } = ownerId;
}

public class GetAddressesQueryHandler(IGrainFactory factory) : IRequestHandler<GetAddressesQuery, IEnumerable<AddressDto>>
{
    private readonly IGrainFactory _factory = factory;

    public async Task<IEnumerable<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        // get all item keys for this owner
        var keys = await _factory.GetGrain<IAddressManagerGrain>(request.OwnerId).GetAllAsync();

        // fast path for empty owner
        if (keys.Length == 0) return [];

        // fan out and get all individual items in parallel
        var tasks = ArrayPool<Task<AddressItem>>.Shared.Rent(keys.Length);
        try
        {
            // issue all requests at the same time
            for (var i = 0; i < keys.Length; ++i)
            {
                tasks[i] = _factory.GetGrain<IAddressGrain>(keys[i]).GetAsync();
            }

            // compose the result as requests complete
            var result = ImmutableArray.CreateBuilder<AddressItem>(tasks.Length);
            for (var i = 0; i < keys.Length; ++i)
            {
                result.Add(await tasks[i]);
            }
            List<AddressDto> converted = [];
            foreach (var AddresseItem in result)
            {
                var Addresse = new AddressDto
                {
                    Id = AddresseItem.Id,
                    Name = AddresseItem.Name,
                    Address = AddresseItem.Address,
                    City = AddresseItem.City,
                    State = AddresseItem.State,
                    Country = AddresseItem.Country,
                    ZipCode = AddresseItem.ZipCode,
                    Active = AddresseItem.Active,
                };
                converted.Add(Addresse);
            }
            return converted;
        }
        finally
        {
            ArrayPool<Task<AddressItem>>.Shared.Return(tasks);
        }
    }
}