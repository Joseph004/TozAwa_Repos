using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ShareRazorClassLibrary.Helpers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class TozawaFirstRequestError : PageModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<TozawaFirstRequestError> _logger;

    public TozawaFirstRequestError(ILogger<TozawaFirstRequestError> logger)
    {
        _logger = logger;
    }

    public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch", Justification = "<Pending>")]
    public void OnPost()
    {
        var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandler is not null)
        {
            _logger.LogCritical("An error occured", exceptionHandler.Error.Message);
            _logger.LogCritical("Trace error", exceptionHandler.Error.StackTrace);
        }
    }
}