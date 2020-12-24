using System.Drawing;
using System.Drawing.Imaging;

namespace TelegramThemeCreator.Extensions
{
    /// <summary>
    /// Provides extensions methods for <see cref="Bitmap" />.
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// Saves <see cref="Bitmap" /> image to specified folder with required name and image format.
        /// </summary>
        /// <param name="bitmap">Bitmap which is will be saved.</param>
        /// <param name="path">A string that contains path where the image will be saved.</param>
        /// <param name="filename">A string that contains image file name.</param>
        /// <param name="format">A format in which image will be saved.</param>
        public static void SaveImage(this Bitmap bitmap, string path, string filename, ImageFormat format)
        {
            bitmap.Save(path + filename, format);
        }
    }
}
