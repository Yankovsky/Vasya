using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Vasya.Utilities
{
    static class ImageCreator
    {
        public static BitmapSource CreateBitmap(byte[] imageData, int width, int height, PixelFormat pixelFormat)
        {
            double dpiX = 96d;
            double dpiY = 96d;
            int bytesPerPixel = (pixelFormat.BitsPerPixel + 7) / 8; ; 
            int stride = bytesPerPixel*width; 

            return BitmapSource.Create(width, height, dpiX, dpiY, pixelFormat, null, imageData, stride);
        }
    }
}