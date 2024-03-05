using System.Drawing;
using System.Drawing.Imaging;
using Svg;

namespace OrleansHost.Helpers
{
    public static class ThumbnailProvider
    {
        private static bool ThumbnailCallback()
        {
            return false;
        }

        public static byte[] GetThumbnail(byte[] fileContent, string extension)
        {
            using var ms = new MemoryStream(fileContent);
            if (extension == "svg")
            {
                var svgDocument = SvgDocument.Open<SvgDocument>(ms);
                var bitmap = svgDocument.Draw();

                return SaveThumbnail(CreateThumbnail(bitmap));
            }

#pragma warning disable CA1416 // Validate platform compatibility
            using var image = Image.FromStream(ms);
            return SaveThumbnail(CreateThumbnail(image));
#pragma warning restore CA1416 // Validate platform compatibility
        }

        private static byte[] SaveThumbnail(Image thumbnail)
        {
            using (var memoryStream = new MemoryStream())
            {
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
                thumbnail.Save(memoryStream, ImageFormat.Png);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
                return memoryStream.ToArray();
            }
        }

        private static Image CreateThumbnail(Image image)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var ratioX = (double)150 / image.Width;
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            var ratioY = (double)150 / image.Height;
#pragma warning restore CA1416 // Validate platform compatibility
            var ratio = Math.Min(ratioX, ratioY);

#pragma warning disable CA1416 // Validate platform compatibility
            var width = (int)((image.Width * ratio) * 3);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            var height = (int)((image.Height * ratio) * 3);
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
            return image.GetThumbnailImage(width, height, ThumbnailCallback, IntPtr.Zero);
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
