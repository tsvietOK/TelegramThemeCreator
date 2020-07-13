namespace TelegramThemeCreator
{
    public class ThemeColor
    {
        private string link;

        public ThemeColor(string name, string value)
        {
            Name = name;

            if (ColorUtils.IsColor(value))
            {
                IsColor = true;
                IsStandardColor = ColorUtils.IsStandardColor(value);

                if (value.Length == 9)
                {
                    Value = new UniColor(value, HexFormat.RGBA);
                }
                else
                {
                    Value = new UniColor(value, HexFormat.RGB);
                }
            }
            else
            {
                link = value;
                IsColor = false;
                IsStandardColor = false;
            }
        }

        public string Name { get; set; }


        public UniColor Value { get; set; }

        public bool IsColor { get; set; }

        public bool IsStandardColor { get; set; }

        public string GetColor()
        {
            return IsColor ? Value.ToHex() : link;
        }
    }
}
