using System;
using System.Runtime.InteropServices;

namespace GoldEngine
{
    internal sealed class BuilderUtility
    {
        // Fields
        public const string kDoubleQuote = "\"";
        public const char MidDotChar = '\x0095';
        public const short SW_SHOWNORMAL = 1;

        // Methods
        public static string CharNameShort(char TheChar)
        {
            switch ((TheChar - ' '))
            {
            case '\0':
                return "Space";

            case '\x0001':
                return "Exclam";

            case '\x0002':
                return "Quote";

            case '\x0003':
                return "Num";

            case '\x0004':
                return "Dollar";

            case '\x0005':
                return "Percent";

            case '\x0006':
                return "Amp";

            case '\a':
                return "Apost";

            case '\b':
                return "LParen";

            case '\t':
                return "RParen";

            case '\n':
                return "Times";

            case '\v':
                return "Plus";

            case '\f':
                return "Comma";

            case '\r':
                return "Minus";

            case '\x000e':
                return "Dot";

            case '\x000f':
                return "Div";

            case '\x001a':
                return "Colon";

            case '\x001b':
                return "Semi";

            case '\x001c':
                return "Lt";

            case '\x001d':
                return "Eq";

            case '\x001e':
                return "Gt";

            case '\x001f':
                return "Question";

            case ' ':
                return "At";

            case ';':
                return "LBracket";

            case '<':
                return "Backslash";

            case '=':
                return "RBracket";

            case '>':
                return "Caret";

            case '?':
                return "UScore";

            case '@':
                return "Accent";

            case '[':
                return "LBrace";

            case '\\':
                return "Pipe";

            case ']':
                return "RBrace";

            case '^':
                return "Tilde";
            }
            return Conversions.ToString((char)TheChar);
        }

        public static string CleanString(string Text, bool ReplaceSpaceChar = true)
        {
            long num2;
            bool flag2 = ReplaceSpaceChar;
            if (!flag2)
            {
                bool flag = false;
                for (num2 = 1L; !((num2 > Strings.Len(Text)) | flag); num2 += 1L)
                {
                    if (Strings.Mid(Text, (int)num2, 1) != " ")
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    flag2 = true;
                }
            }
            string str4 = "";
            long num3 = Strings.Len(Text);
            for (num2 = 1L; num2 <= num3; num2 += 1L)
            {
                string str3;
                string str = Strings.Mid(Text, (int)num2, 1);
                long num = Strings.AscW(str);
                long num4 = num;
                switch (num4)
                {
                case 9L:
                    str3 = "{HT}";
                    break;

                case 10L:
                    str3 = "{LF}";
                    break;

                case 11L:
                    str3 = "{VT}";
                    break;

                case 12L:
                    str3 = "{FF}";
                    break;

                case 13L:
                    str3 = "{CR}";
                    break;

                case 0x20L:
                    if (flag2)
                    {
                        str3 = "{Space}";
                    }
                    else
                    {
                        str3 = " ";
                    }
                    break;

                default:
                    if (num4 == 160L)
                    {
                        str3 = "{NBSP}";
                    }
                    else if (num4 == 0x20acL)
                    {
                        str3 = "{Euro Sign}";
                    }
                    else if (((num >= 0x20L) & (num <= 0x7eL)) | ((num >= 160L) & (num <= 0xffL)))
                    {
                        str3 = str;
                    }
                    else
                    {
                        str3 = "{#" + Conversions.ToString(num) + "}";
                    }
                    break;
                }
                str4 = str4 + str3;
            }
            return str4;
        }

        public static string CreateFileName(string Label, string AdditionalChars = "")
        {
            string str3 = "";
            string str4 = "\\/:*?<>|\"" + AdditionalChars;
            short num2 = (short)(Label.Length - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                string str = Conversions.ToString(Label[i]);
                if (str4.IndexOf(str) == -1)
                {
                    str3 = str3 + str;
                }
            }
            return str3;
        }

        private static string DisplayChar(int CharCode, bool ReplaceSpaceChar)
        {
            switch (CharCode)
            {
            case 9:
                return "{HT}";

            case 10:
                return "{LF}";

            case 11:
                return "{VT}";

            case 12:
                return "{FF}";

            case 13:
                return "{CR}";

            case 0x20:
                return Conversions.ToString(ReplaceSpaceChar ? "{Space}" : " ");

            case 160:
                return "{NBSP}";

            case 0x20ac:
                return "{Euro Sign}";
            }
            if (((CharCode >= 0x20) & (CharCode <= 0x7e)) | ((CharCode >= 160) & (CharCode <= 0xff)))
            {
                return Conversions.ToString(Strings.ChrW(CharCode));
            }
            return ("{#" + Conversions.ToString(CharCode) + "}");
        }

        private static string DisplayCodeText(int Codepoint)
        {
            string str2 = Conversion.Hex(Codepoint);
            if ((str2.Length % 2) == 1)
            {
                str2 = "0" + str2;
            }
            return str2;
        }

        public static string DisplayRangeListText(NumberRangeList Ranges)
        {
            string str2 = "";
            str2 = DisplayRangeText(Ranges[0]);
            int num2 = Ranges.Count - 1;
            for (int i = 1; i <= num2; i++)
            {
                NumberRange range = Ranges[i];
                str2 = str2 + ", " + DisplayRangeText(Ranges[i]);
                range = null;
            }
            return str2;
        }

        public static string DisplayRangeText(NumberRange Range)
        {
            string str2 = "";
            NumberRange range = Range;
            if (range.First == range.Last)
            {
                str2 = "&" + DisplayCodeText(range.First);
            }
            else if ((range.Last - range.First) == 1)
            {
                str2 = "&" + DisplayCodeText(range.First) + ", &" + DisplayCodeText(range.Last);
            }
            else
            {
                str2 = "&" + DisplayCodeText(range.First) + " .. &" + DisplayCodeText(range.Last);
            }
            range = null;
            return str2;
        }

        public static string DisplayText(CharacterSet CharSet, bool ReplaceSpaceChar = true, int MaxSize = 0x400, string OversizeMessage = "", short BreakWidth = -1)
        {
            int num;
            string str2 = "";
            NumberRangeList list = CharSet.RangeList();
            bool flag = true;
            for (num = 0; (num < list.Count) & flag; num++)
            {
                if (!IsDisplayRange(list[num].First, list[num].Last))
                {
                    flag = false;
                }
            }
            if (flag)
            {
                int num2 = CharSet.Count() - 1;
                for (num = 0; num <= num2; num++)
                {
                    str2 = str2 + DisplayChar(CharSet[num], true);
                }
                return str2;
            }
            return DisplayRangeListText(CharSet.RangeList());
        }

        public static string DisplayText(string Text, bool ReplaceSpaceChar = true, int MaxSize = 0x400, string OversizeMessage = "", short BreakWidth = -1)
        {
            if (Text.Length > MaxSize)
            {
                if (OversizeMessage == "")
                {
                    return ("The text is too large to view: " + Conversions.ToString(Text.Length) + " characters");
                }
                return OversizeMessage;
            }
            short num = 0;
            string str3 = "";
            int num3 = Text.Length - 1;
            for (int i = 0; i <= num3; i++)
            {
                string str2 = DisplayChar(Text[i], ReplaceSpaceChar);
                if (BreakWidth != -1)
                {
                    num = (short)(num + str2.Length);
                    if (num > BreakWidth)
                    {
                        str3 = str3 + " ";
                        num = 0;
                    }
                }
                str3 = str3 + str2;
            }
            return str3;
        }

        public static string HTMLChar(int CharCode, bool SpaceToNBSP = false)
        {
            switch (CharCode)
            {
            case 10:
                return "<br/>";

            case 13:
                return "";

            case 0x20:
                return Conversions.ToString(Interaction.IIf(SpaceToNBSP, "&nbsp;", " "));

            case 0x22:
                return "&quot;";

            case 60:
                return "&lt;";

            case 0x3e:
                return "&gt;";

            case 0x26:
                return "&amp;";

            case 0x95:
                return "&middot;";

            case 160:
                return "&nbsp;";
            }
            if (CharCode > 0xff)
            {
                return ("&#" + Conversions.ToString(CharCode) + ";");
            }
            return Conversions.ToString(Strings.ChrW(CharCode));
        }

        public static string HTMLText(string Text, bool EmptyToNBSP = true, bool SpaceToNBSP = false)
        {
            string str2 = "";
            if (Text == "")
            {
                return Conversions.ToString(EmptyToNBSP ? "&nbsp;" : "");
            }
            short num2 = (short)(Text.Length - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                str2 = str2 + HTMLChar(Text[i], SpaceToNBSP);
            }
            return str2;
        }

        private static string IdentifierCase(string Source, bool CapitializeFirst)
        {
            bool flag = CapitializeFirst;
            string str3 = "";
            short num2 = (short)(Source.Length - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                string str = Conversions.ToString(Source[i]);
                if (str == " ")
                {
                    str3 = str3 + " ";
                    flag = true;
                }
                else if (flag)
                {
                    str3 = str3 + str.ToUpper();
                    flag = false;
                }
                else
                {
                    str3 = str3 + str.ToLower();
                }
            }
            return str3;
        }

        public static bool IsAlphaNumeric(string Text)
        {
            bool flag = false;
            for (int i = 0; (i < Text.Length) & !flag; i++)
            {
                if (!LikeOperator.LikeString(Conversions.ToString(Text[i]), "[a-zA-Z0-9_]", CompareMethod.Binary))
                {
                    flag = true;
                }
            }
            return !flag;
        }

        private static bool IsDisplayRange(int First, int Last)
        {
            return (((((First >= 0x20) & (First <= 0x7f)) & (Last >= 0x20)) & (Last <= 0x7f)) | ((((First >= 160) & (First <= 0xff)) & (Last >= 160)) & (Last <= 0xff)));
        }

        public static int OpenWebsite(int hwndOwner, string URL)
        {
            int num;
            string lpOperation = "Open";
            int num2 = ShellExecute(hwndOwner, lpOperation, URL, Conversions.ToString(0), Conversions.ToString(0), 1);
            return num;
            Information.Err().Clear();
            return 0;
        }

        public static string RemoveNull(string Text)
        {
            string str3 = "";
            short num2 = (short)Strings.Len(Text);
            for (short i = 1; i <= num2; i = (short)(i + 1))
            {
                string str = Strings.Mid(Text, i, 1);
                if (str != "\0")
                {
                    str3 = str3 + str;
                }
            }
            return str3;
        }

        [DllImport("shell32.dll", EntryPoint = "ShellExecuteA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int ShellExecute(int hwnd, [MarshalAs(UnmanagedType.VBByRefStr)] string lpOperation, [MarshalAs(UnmanagedType.VBByRefStr)] string lpFile, [MarshalAs(UnmanagedType.VBByRefStr)] string lpParameters, [MarshalAs(UnmanagedType.VBByRefStr)] string lpDirectory, int nShowCmd);
        public static string TimeDiffString(DateTime StartTime, DateTime EndTime)
        {
            return (Conversions.ToString(Conversion.Int((double)(((double)DateTime.DateDiff(DateInterval.Minute, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1)) / 60.0))) + " Hours " + Conversions.ToString(Conversion.Int((long)(DateTime.DateDiff(DateInterval.Minute, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) % 60L))) + " Minutes " + Conversions.ToString(Conversion.Int((long)(DateTime.DateDiff(DateInterval.Second, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) % 60L))) + " Seconds");
        }

        public static string ToIdentifier(string Label, IDTypeCase TypeCase = IDTypeCase.Propercase, string ConvertSpaceChar = "", bool RemoveDashes = false)
        {
            string source = "";
            string str3 = Label.Trim();
            if (str3 == "")
            {
                source = "";
            }
            else
            {
                char ch;
                string text = Conversions.ToString(str3[0]);
                if (IsAlphaNumeric(text))
                {
                    source = text;
                }
                else
                {
                    ch = Conversions.ToChar(text);
                    text = Conversions.ToString(ch);
                    source = CharNameShort(ch);
                }
                short num2 = (short)(str3.Length - 1);
                for (short i = 1; i <= num2; i = (short)(i + 1))
                {
                    text = Conversions.ToString(str3[i]);
                    if (IsAlphaNumeric(text) | (text == "_"))
                    {
                        source = source + text;
                    }
                    else if (!((text == "-") & RemoveDashes))
                    {
                        if (text == " ")
                        {
                            source = source + ConvertSpaceChar;
                        }
                        else
                        {
                            ch = Conversions.ToChar(text);
                            text = Conversions.ToString(ch);
                            source = source + CharNameShort(ch);
                        }
                    }
                }
            }
            switch (((int)TypeCase))
            {
            case 1:
                return source.ToUpper();

            case 2:
                return source.ToLower();

            case 3:
                return IdentifierCase(source, true);

            case 4:
                return IdentifierCase(source, false);
            }
            return source;
        }

        public static string XMLText(string Text)
        {
            string str2 = "";
            int num3 = Text.Length - 1;
            for (int i = 0; i <= num3; i++)
            {
                string str = Conversions.ToString(Text[i]);
                string str4 = str;
                if (str4 == "<")
                {
                    str2 = str2 + "&lt;";
                }
                else if (str4 == ">")
                {
                    str2 = str2 + "&gt;";
                }
                else if (str4 == "&")
                {
                    str2 = str2 + "&amp;";
                }
                else if (str4 == "\"")
                {
                    str2 = str2 + "&quot;";
                }
                else
                {
                    int num = Strings.AscW(str);
                    if ((num >= 0x20) & (num <= 0x7e))
                    {
                        str2 = str2 + str;
                    }
                    else
                    {
                        str2 = str2 + "&#" + Conversions.ToString(num) + ";";
                    }
                }
            }
            return str2;
        }

        // Nested Types
        public enum IDTypeCase
        {
            AsIs,
            Uppercase,
            Lowercase,
            Propercase,
            Camelcase
        }
    }
}
