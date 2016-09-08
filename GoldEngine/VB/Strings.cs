using System;

namespace GoldEngine
{
    public enum CompareMethod
    {
        Binary,
        Text
    }

    internal class Strings
    {
        public static char ChrW(int CharCode)
        {
            if ((CharCode < -32768) || (CharCode > 0xffff))
            {
                throw new ArgumentException("Argument_RangeTwoBytes1");
            }
            return Convert.ToChar((int)(CharCode & 0xffff));
        }

        public static string RTrim(string str)
        {
            throw new NotImplementedException();
        }

        public static long Len(string text)
        {
            return text.Length;
        }

        public static string Mid(string text, int num2, int p2)
        {
            throw new NotImplementedException();
        }

        public static int AscW(string str)
        {
            throw new NotImplementedException();
        }

        public static string Space(int i)
        {
            throw new NotImplementedException();
        }

        public static string UCase(string value)
        {
            throw new NotImplementedException();
        }

        public static string[] Split(string value, string s, int i, CompareMethod binary)
        {
            throw new NotImplementedException();
        }
    }
}