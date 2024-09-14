

using System.Threading.Tasks;
using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Services;

public interface ISnackBarService
{
    void Add(IResponse response);
}