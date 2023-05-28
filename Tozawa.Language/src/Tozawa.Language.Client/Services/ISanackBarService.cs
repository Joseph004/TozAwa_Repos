

using System.Threading.Tasks;

namespace Tozawa.Language.Client.Services;

public interface ISnackBarService
{
    void Add(IResponse response);
}