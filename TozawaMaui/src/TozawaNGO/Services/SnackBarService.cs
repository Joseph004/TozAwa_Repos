using MudBlazor;

namespace TozawaNGO.Services;
public class SnackBarService(ISnackbar snackBar) : ISnackBarService
{
    private readonly ISnackbar _snackBar = snackBar;

    public void Add(IResponse response)
    {
        var responseMessage = !string.IsNullOrWhiteSpace(response.Message) ? response.Message : string.Empty;
        _snackBar.Add(responseMessage, response.Success ? Severity.Success : Severity.Error);
    }
}
