using ImageMagick;

namespace Tozawa.Bff.Portal.Helper
{
    public static class ImageProcessingHelper
    {
        /// <summary>
        /// Resizes an image to a specific size 
        /// </summary>
        /// <param name="imageData">The image to be resized</param>
        /// <param name="width">The width in pixels to resize to</param>
        /// <param name="height">The height in pixels to resize to</param>
        /// <param name="outputMimeType">The output mime type of the image</param>
        /// <param name="ignoreAspectRatio">Determines of the image should be resized with or without regards to the aspect ratio</param>
        /// <returns>The resized image</returns>
        public static byte[] Resize(byte[] imageData, int? width, int? height, MagickFormat? outputMimeType = null, bool ignoreAspectRatio = false)
        {
            if (!width.HasValue && !height.HasValue)
                throw new Exception("Width or height has to have a value when resizing");

            using (MagickImage image = new MagickImage(imageData))
            {
                var targetSize = new MagickGeometry(width ?? image.Width, height ?? image.Height)
                {
                    IgnoreAspectRatio = ignoreAspectRatio
                };

                if (outputMimeType.HasValue)
                    image.Format = outputMimeType.Value;

                image.Resize(targetSize);

                return image.ToByteArray();
            }
        }

        /// <summary>
        /// Resizes the image to the given pixel amount
        /// </summary>
        /// <param name="imageData">The image to be resized</param>
        /// <param name="targetPixelAmount">The amount of pixels that the image should be resized to</param>
        /// <param name="resizeIfSmaller">Boolean that dictates if the image should be resized to the target pixel amount, even if the image contains fewer pixels than the target</param>
        public static byte[] ResizeImageToTargetAmountOfPixels(byte[] imageData, int targetPixelAmount, bool resizeIfSmaller = true)
        {
            using (MagickImage image = new MagickImage(imageData))
            {
                var pixelAmount = image.Width * image.Height;

                if (pixelAmount == targetPixelAmount ||
                    (pixelAmount < targetPixelAmount && !resizeIfSmaller))
                    return imageData;

                var pixelReductionRatio = Math.Sqrt(Convert.ToDouble(targetPixelAmount) / pixelAmount);

                // Rounding of the image dimensions will often give an image that exceeds the target pixel amount by a small margin
                var targetWidth = Convert.ToInt32(Math.Round(image.Width * pixelReductionRatio));
                var targetHeight = Convert.ToInt32(Math.Round(image.Height * pixelReductionRatio));

                return Resize(imageData, targetWidth, targetHeight);
            }
        }

        /// <summary>
        /// Rotates an image based on its exif rotation and removes the exif metadata. 
        /// This is useful to do since some image viewers can ignore the exif rotation, causing the image to be rotated incorrectly.
        /// </summary>
        /// <param name="imageData">The image to be rotated</param>
        /// <returns></returns>
        public static byte[] RotateToExifRotation(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                throw new Exception("Can't rotate to exif rotation if no image was sent in as an argument");

            using (MagickImage image = new MagickImage(imageData))
            {
                var profile = image.GetExifProfile();
                if (profile != null)
                {
                    image.AutoOrient();
                    profile.SetValue(ExifTag.Orientation, (ushort)0);
                }

                return image.ToByteArray();
            }
        }

        /// <summary>
        /// Compresses an image to help reduce its file size
        /// </summary>
        /// <param name="imageData">The image to compress</param>
        /// <param name="quality">The quality of the image that the compression will use. 0 is the lowest quality and 100 is the highest quality</param>
        /// <param name="compressionMethod">The compression alghorithm that will be used for compressing the image</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] imageData, int quality = 75, CompressionMethod? compressionMethod = null)
        {
            if (imageData == null || imageData.Length == 0)
                throw new Exception("Can't compress image if no image was sent in as an argument");

            using (MagickImage image = new MagickImage(imageData))
            {
                image.Quality = quality;
                image.Settings.Compression = compressionMethod.HasValue ? compressionMethod.Value : image.Compression;
                return image.ToByteArray();
            }
        }
    }
}
