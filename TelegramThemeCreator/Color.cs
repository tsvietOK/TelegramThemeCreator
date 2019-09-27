namespace TelegramThemeCreator
{
    public class Color
    {
        private string name;
        private UniColor value;
        private string link;
        private bool isColor;
        private bool isStandartColor;

        public Color(string name, string value)
        {
            this.name = name;

            if (ColorUtils.IsColor(value))
            {
                isColor = true;
                isStandartColor = ColorUtils.IsStandartColor(value);

                if (value.Length == 9)
                {
                    this.value = new UniColor(value, HexFormat.RGBA);
                }
                else
                {
                    this.value = new UniColor(value, HexFormat.RGB);
                }
            }
            else
            {
                link = value;
                isColor = false;
                isStandartColor = false;
            }
        }

        public string GetName()
        {
            return name;
        }

        public UniColor GetValue()
        {
            return value;
        }

        public string GetColor()
        {
            return isColor ? value.ToHex() : link;
        }

        public bool IsColor()
        {
            return isColor;
        }

        public bool IsStandartColor()
        {
            return isStandartColor;
        }

        public void ChangeColor(UniColor color)
        {
            value = color;
        }
    }
}
