using Microsoft.AspNetCore.Http;
using System.IO;

namespace Tozawa.Attachment.Svc.Tests.Helpers;

internal static class FormFileHelper
{
    public static IFormFile GetIFormFileFromFile(string file)
    {
        var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last())
        {
            Headers = new HeaderDictionary(),
            ContentType = string.Empty
        };

        return formFile;
    }
}
