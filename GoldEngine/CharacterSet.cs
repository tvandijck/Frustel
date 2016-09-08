using System.Text;

namespace GoldEngine
{
    internal class CharacterSet : NumberSet
    {
        // Fields
        public int TableIndex;

        // Methods
        public CharacterSet() : base(new int[0])
        {
            base.Blocksize = 0x40;
        }

        public CharacterSet(NumberSet CharCodes) : base(CharCodes)
        {
        }

        public CharacterSet(string Text) : base(new int[0])
        {
            int num2 = Text.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                base.Add(new int[] { Text[i] });
            }
        }

        public CharacterSet(params int[] CharCodes) : base(CharCodes)
        {
        }

        public char Chars(int Index)
        {
            char ch = '\0';
            if ((Index >= 0) & (Index < base.Count()))
            {
                ch = Strings.ChrW(base[Index]);
            }
            return ch;
        }

        public int Length()
        {
            return base.Count();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int num2 = base.Count() - 1;
            for (int i = 0; i <= num2; i++)
            {
                builder.Append(Strings.ChrW(base[i]));
            }
            return builder.ToString();
        }

        public string XMLText()
        {
            string str = "";
            int num3 = base.Count() - 1;
            for (int i = 0; i <= num3; i++)
            {
                int charCode = base[i];
                int num4 = base[i];
                if (num4 == 60)
                {
                    str = str + "&lt;";
                }
                else if (num4 == 0x3e)
                {
                    str = str + "&gt;";
                }
                else if (num4 == 0x26)
                {
                    str = str + "&amp;";
                }
                else if (num4 == 0x22)
                {
                    str = str + "&quot;";
                }
                else if ((charCode >= 0x20) & (charCode <= 0x7e))
                {
                    str = str + Conversions.ToString(Strings.ChrW(charCode));
                }
                else
                {
                    str = str + "&#" + Conversions.ToString(charCode) + ";";
                }
            }
            return str;
        }
    }
}