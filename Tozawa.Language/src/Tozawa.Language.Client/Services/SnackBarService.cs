using MudBlazor;

namespace Tozawa.Language.Client.Services;
public class SnackBarService : ISnackBarService
{
    private readonly ISnackbar _snackBar;

    public SnackBarService(ISnackbar snackBar)
    {
        _snackBar = snackBar;
    }

    public void Add(IResponse response)
    {
        var responseMessage = !string.IsNullOrWhiteSpace(response.Message) ? response.Message : string.Empty;
        _snackBar.Add(responseMessage, response.Success ? Severity.Success : Severity.Error);
    }
}
