namespace TelegramThemeCreator
{
    public static class ColorUtils
    {
        public static bool IsColor(string color)
        {
            return color.StartsWith("#");
        }

        public static bool IsStandartColor(string hexColor)
        {
            return hexColor.StartsWith("#ffffff")
                   || hexColor.StartsWith("#000000")
                   || hexColor.StartsWith("#ffffffff")
                   || hexColor.StartsWith("#00000000");
        }
    }
}
