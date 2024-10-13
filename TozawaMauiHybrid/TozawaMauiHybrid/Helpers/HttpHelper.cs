namespace TozawaMauiHybrid.Helpers;

public static class HttpHelper
{
    public static HttpClientHandler GetInsecureHandler()
    {
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert.Issuer.Equals("CN=localhost"))
                return true;
            return errors == System.Net.Security.SslPolicyErrors.None;
        }
        };
        return handler;
    }
}