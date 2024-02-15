

using System.Threading.Tasks;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Services;

public interface ISnackBarService
{
    void Add(IResponse response);
}