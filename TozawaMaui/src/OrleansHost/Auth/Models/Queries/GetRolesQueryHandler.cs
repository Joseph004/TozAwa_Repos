using System.Buffers;
using System.Collections.Immutable;
using Grains;
using Grains.Auth.Models.Dtos;
using MediatR;

namespace OrleansHost.Auth.Models.Queries;

public class GetRolesQuery(Guid ownerId) : IRequest<IEnumerable<RoleDto>>
{
    public Guid OwnerId { get; } = ownerId;
}

public class GetRolesQueryHandler(IGrainFactory factory) : IRequestHandler<GetRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IGrainFactory _factory = factory;

    public async Task<IEnumerable<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        // get all item keys for this owner
        var keys = await _factory.GetGrain<IRoleManagerGrain>(request.OwnerId).GetAllAsync();

        // fast path for empty owner
        if (keys.Length == 0) return [];

        // fan out and get all individual items in parallel
        var tasks = ArrayPool<Task<RoleItem>>.Shared.Rent(keys.Length);
        try
        {
            // issue all requests at the same time
            for (var i = 0; i < keys.Length; ++i)
            {
                tasks[i] = _factory.GetGrain<IRoleGrain>(keys[i]).GetAsync();
            }

            // compose the result as requests complete
            var result = ImmutableArray.CreateBuilder<RoleItem>(tasks.Length);
            for (var i = 0; i < keys.Length; ++i)
            {
                result.Add(await tasks[i]);
            }
            List<RoleDto> converted = [];
            foreach (var roleItem in result)
            {
                var role = new RoleDto
                {
                    Id = roleItem.Id,
                    OrganizationId = roleItem.OrganizationId,
                    Role = (Role)roleItem.Role,
                    Functions = roleItem.Functions.Select(x => new FunctionDto
                    {
                        RoleId = roleItem.Id,
                        FunctionType = x
                    }).ToList(),
                };
                converted.Add(role);
            }
            return converted;
        }
        finally
        {
            ArrayPool<Task<RoleItem>>.Shared.Return(tasks);
        }
    }
}