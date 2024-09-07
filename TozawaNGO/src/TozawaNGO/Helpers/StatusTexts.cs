using System.Net;

namespace TozawaNGO.Helpers;

public static class StatusTexts
{
    public static string GetHttpStatusText(HttpStatusCode? code)
    {
        return code switch
        {
            HttpStatusCode.OK => "Success",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.NotFound => "Not found",
            _ => "Something went wrong"
        };
    }
}
