using System.Globalization;

namespace GoldEngine
{
    internal static class Operators
    {
        public static int CompareString(string left, string right, bool textCompare)
        {
            int num2;
            if (left == right)
            {
                return 0;
            }
            if (left == null)
            {
                if (right.Length == 0)
                {
                    return 0;
                }
                return -1;
            }
            if (right == null)
            {
                if (left.Length == 0)
                {
                    return 0;
                }
                return 1;
            }
            if (textCompare)
            {
                num2 = Utils.GetCultureInfo().CompareInfo.Compare(left, right, CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase);
            }
            else
            {
                num2 = string.CompareOrdinal(left, right);
            }
            if (num2 == 0)
            {
                return 0;
            }
            if (num2 > 0)
            {
                return 1;
            }
            return -1;

        }

        public static object ConcatenateObject(object left, object concatenateObject)
        {
            throw new System.NotImplementedException();
        }

        public static object CompareObjectNotEqual(object itemIndex, int i, bool b)
        {
            throw new System.NotImplementedException();
        }
    }
}