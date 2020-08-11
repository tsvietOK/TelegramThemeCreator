using System.Drawing;
using System.Drawing.Imaging;

namespace TelegramThemeCreator.Utils
{
    /// <summary>
    /// Provides static methods for bitmap processing.
    /// </summary>
    public static class BitmapUtils
    {
        /// <summary>
        /// Generates an image with specified width, height and filled with color.
        /// </summary>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="color">Color of image.</param>
        /// <returns>New <see cref="Bitmap" /> with specified parameters.</returns>
        public static Bitmap GenerateImage(int width, int height, Color color)
        {
            var bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(color);
            return bitmap;
        }

        /// <summary>
        /// Saves <see cref="Bitmap" /> image to specified folder with required name and image fromat.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="path">A string that contains path where the image will be saved.</param>
        /// <param name="filename">A string that contains image file name.</param>
        /// <param name="format">A format in which image will be saved.</param>
        public static void SaveImage(this Bitmap bitmap, string path, string filename, ImageFormat format)
        {
            bitmap.Save(path + filename, format);
        }
    }
}
