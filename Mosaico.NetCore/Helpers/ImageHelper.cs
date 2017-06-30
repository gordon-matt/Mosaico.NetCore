using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mosaico.NetCore.Helpers
{
    public enum AnchorPosition : byte
    {
        Center,
        Top,
        Bottom,
        Left,
        Right
    }

    public static class ImageHelper
    {
        public static Image Resize(Image image, int width, int height)
        {
            var destinationRectangle = new Rectangle(0, 0, width, height);
            var destinationImage = new Bitmap(width, height);

            destinationImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(destinationImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var imageAttributes = new ImageAttributes())
                {
                    imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, destinationRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                }
            }

            return destinationImage;
        }

        // Based on: https://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
        public static Image Crop(Image image, int width, int height, AnchorPosition anchorPosition)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destinationX = 0;
            int destinationY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = width / (float)sourceWidth;
            nPercentH = height / (float)sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (anchorPosition)
                {
                    case AnchorPosition.Top: destinationY = 0; break;
                    case AnchorPosition.Bottom: destinationY = (int)(height - (sourceHeight * nPercent)); break;
                    default: destinationY = (int)((height - (sourceHeight * nPercent)) / 2); break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (anchorPosition)
                {
                    case AnchorPosition.Left: destinationX = 0; break;
                    case AnchorPosition.Right: destinationX = (int)(width - (sourceWidth * nPercent)); break;
                    default: destinationX = (int)((width - (sourceWidth * nPercent)) / 2); break;
                }
            }

            int destinationWidth = (int)(sourceWidth * nPercent);
            int destinationHeight = (int)(sourceHeight * nPercent);

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(
                    image,
                    new Rectangle(destinationX, destinationY, destinationWidth, destinationHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }

            return bitmap;
        }
    }
}