using System;
using System.Globalization;
using System.Threading;

namespace GoldEngine
{
    internal static class Utils
    {
        internal static CultureInfo GetCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public static Array CopyArray(Array arySrc, Array aryDest)
        {
            if (arySrc != null)
            {
                int length = arySrc.Length;
                if (length == 0)
                {
                    return aryDest;
                }
                if (aryDest.Rank != arySrc.Rank)
                {
                    throw new InvalidCastException("Array_RankMismatch");
                }
                int num8 = aryDest.Rank - 2;
                for (int i = 0; i <= num8; i++)
                {
                    if (aryDest.GetUpperBound(i) != arySrc.GetUpperBound(i))
                    {
                        throw new ArrayTypeMismatchException("Array_TypeMismatch");
                    }
                }
                if (length > aryDest.Length)
                {
                    length = aryDest.Length;
                }
                if (arySrc.Rank > 1)
                {
                    int rank = arySrc.Rank;
                    int num7 = arySrc.GetLength(rank - 1);
                    int num6 = aryDest.GetLength(rank - 1);
                    if (num6 != 0)
                    {
                        int num5 = Math.Min(num7, num6);
                        int num9 = (arySrc.Length / num7) - 1;
                        for (int j = 0; j <= num9; j++)
                        {
                            Array.Copy(arySrc, j * num7, aryDest, j * num6, num5);
                        }
                    }
                    return aryDest;
                }
                Array.Copy(arySrc, aryDest, length);
            }
            return aryDest;
        }
    }
}