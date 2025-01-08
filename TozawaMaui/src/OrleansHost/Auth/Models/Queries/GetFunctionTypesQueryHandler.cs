using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains.Models;
using MediatR;

namespace OrleansHost.Auth.Models.Queries;

public class GetFunctionTypesQuery : IRequest<IEnumerable<FunctionDto>>
{
}

public class GetFunctionTypesQueryHandler(ICurrentUserService currentUserService) : IRequestHandler<GetFunctionTypesQuery, IEnumerable<FunctionDto>>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IEnumerable<FunctionDto>> Handle(GetFunctionTypesQuery request, CancellationToken cancellationToken)
    {
        var functions = await Task.FromResult(GetFunctionTypes());
        return functions;
    }

    private IEnumerable<FunctionDto> GetFunctionTypes()
    {
        return Enum.GetValues(typeof(FunctionType)).Cast<FunctionType>().Select(CreateFunctionDto);
    }

    private FunctionDto CreateFunctionDto(FunctionType x)
    {
        return new FunctionDto
        {
            FunctionType = x
        };
    }
}