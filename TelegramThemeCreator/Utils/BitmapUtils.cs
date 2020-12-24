using System.Drawing;

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
    }
}