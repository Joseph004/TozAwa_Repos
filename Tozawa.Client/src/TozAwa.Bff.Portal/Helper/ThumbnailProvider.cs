using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Svg;

namespace Tozawa.Bff.Portal.Helper
{
    public static class ThumbnailProvider
    {
        private static bool ThumbnailCallback()
        {
            return false;
        }

        public static byte[] GetThumbnail(byte[] fileContent, string extension)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                if (extension == "svg")
                {
                    var svgDocument = SvgDocument.Open<SvgDocument>(ms);
                    var bitmap = svgDocument.Draw();

                    return SaveThumbnail(CreateThumbnail(bitmap));
                }

                using (var image = Image.FromStream(ms))
                {
                    return SaveThumbnail(CreateThumbnail(image));
                }
            }
        }

        private static byte[] SaveThumbnail(Image thumbnail)
        {
            using (var memoryStream = new MemoryStream())
            {
                thumbnail.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        private static Image CreateThumbnail(Image image)
        {
            var ratioX = (double)150 / image.Width;
            var ratioY = (double)150 / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var width = (int)((image.Width * ratio) * 3);
            var height = (int)((image.Height * ratio) * 3);

            return image.GetThumbnailImage(width, height, ThumbnailCallback, IntPtr.Zero);
        }
    }
}
