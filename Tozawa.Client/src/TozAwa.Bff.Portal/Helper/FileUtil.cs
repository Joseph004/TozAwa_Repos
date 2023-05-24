
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Helper
{
    public static class FileUtil
    {
        public static IFormFile FileFromBase64(FileDto fileDto)
        {
            IFormFile file;
            byte[] bytes = Convert.FromBase64String(fileDto.Content);
            MemoryStream stream = new(bytes);

            file = new FormFile(stream, 0, bytes.Length, fileDto.Name, fileDto.FileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = fileDto.ContentType
            };
            return file;
        }

        public static IFormFile FileFromByteArray(string name, string contentType, byte[] content)
        {
            IFormFile file;
            MemoryStream stream = new(content);

            file = new FormFile(stream, 0, content.Length, name, name)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
            return file;
        }
        public static MemoryStream StreamFromByteArray(byte[] content)
        {
            MemoryStream stream = new(content);

            return stream;
        }
        public static byte[] ReadAllBytesFromStream(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}