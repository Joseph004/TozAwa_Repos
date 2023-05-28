

using System.Threading.Tasks;
using Tozawa.Client.Portal.Models.Dtos;

namespace Tozawa.Client.Portal.Services;

public interface ISnackBarService
{
    void Add(IResponse response);
}