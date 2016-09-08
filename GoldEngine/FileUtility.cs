using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoldEngine
{
    internal sealed class FileUtility
    {
        public static string AppPath()
        {
            try
            {
                return GetPath(Assembly.GetExecutingAssembly().Location);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string BuildPath(params object[] pathItems)
        {
            string str3 = "";
            int num2 = pathItems.Count<object>() - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (pathItems[i] is string)
                {
                    string str2 = Conversions.ToString(pathItems[i]);
                    if (((str3 != "") & !str3.EndsWith(@"\")) & !str2.StartsWith(@"\"))
                    {
                        str3 = str3 + @"\" + str2;
                    }
                    else
                    {
                        str3 = str3 + str2;
                    }
                }
            }
            return str3;
        }

        public static void DeleteFile(string fullPath)
        {
            File.Delete(fullPath);
        }

        public static bool FileExists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public static string GetExtension(string fullPath)
        {
            return Path.GetExtension(fullPath);
        }

        public static string GetFileName(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

        public static string GetFileNameBase(string fullPath)
        {
            return Path.GetFileNameWithoutExtension(fullPath);
        }

        public static string GetPath(string fullPath)
        {
            return Path.GetDirectoryName(fullPath);
        }
    }
}