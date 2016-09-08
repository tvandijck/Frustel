using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoldEngine
{
    internal class CommandLine : ArrayList
    {
        // Methods
        public object Add(CommandLineArgument Item)
        {
            return base.Add(Item);
        }

        private string AddCurrentPath(string FileName)
        {
            if (FileUtility.GetPath(FileName) == "")
            {
                return (Directory.GetCurrentDirectory() + @"\" + FileName);
            }
            return FileName;
        }

        public string Argument(string ID)
        {
            return this.Argument(ID, "", false);
        }

        public string Argument(string ID, bool AddPath)
        {
            return this.Argument(ID, "", AddPath);
        }

        public string Argument(string ID, string DefaultValue)
        {
            return this.Argument(ID, DefaultValue, false);
        }

        public string Argument(string ID, string DefaultValue, bool AddPath)
        {
            string str2;
            int num = Conversions.ToInteger(this.ItemIndex(ID));
            if ((num >= 0) & (num < base.Count))
            {
                CommandLineArgument argument = (CommandLineArgument)base[num];
                str2 = argument.Value;
            }
            else
            {
                str2 = DefaultValue;
            }
            if ((str2 != "") & AddPath)
            {
                str2 = this.AddCurrentPath(str2);
            }
            return str2;
        }

        public int Contains(string ID)
        {
            return Conversions.ToInteger(Operators.CompareObjectNotEqual(this.ItemIndex(ID), -1, false));
        }

        private string GetCh(string Text, int Index)
        {
            if ((Index >= 0) & (Index < Text.Count<char>()))
            {
                return Conversions.ToString(Text[Index]);
            }
            return "";
        }

        public object ItemIndex(string Name)
        {
            int num = -1;
            for (int i = 0; (num == -1) & (i < base.Count); i++)
            {
                CommandLineArgument argument = (CommandLineArgument)base[i];
                if (argument.Name.ToUpper() == Name.ToUpper())
                {
                    num = i;
                }
            }
            return num;
        }

        public void ReadArguments(string CmdLine, int AutoNameStart = 1)
        {
            List<string> list = new List<string>();
            int num = AutoNameStart;
            base.Clear();
            list = this.SplitCommand(CmdLine);
            int num3 = list.Count - 1;
            for (int i = 0; i <= num3; i++)
            {
                string str = list[i];
                if (str != "")
                {
                    CommandLineArgument argument = new CommandLineArgument();
                    switch (((char)(str[0] - '+')))
                    {
                    case '\0':
                    case '\x0004':
                        argument.Name = str.Substring(1);
                        argument.Value = "+";
                        break;

                    case '\x0002':
                        argument.Name = str.Substring(1);
                        argument.Value = "-";
                        break;

                    default:
                        argument.Name = "%" + Conversions.ToString(num);
                        argument.Value = str;
                        num++;
                        break;
                    }
                    argument.Position = i;
                    base.Add(argument);
                }
            }
        }

        private List<string> SplitCommand(string Text)
        {
            List<string> list2 = new List<string>();
            int index = 0;
            while (index < Text.Count<char>())
            {
                string str;
                switch (this.GetCh(Text, index))
                {
                case " ":
                    while ((index < Text.Count<char>()) & (this.GetCh(Text, index) == " "))
                    {
                        index++;
                    }
                    break;

                case "\"":
                    index++;
                    str = "";
                    while ((index < Text.Count<char>()) & (this.GetCh(Text, index) != "\""))
                    {
                        str = str + this.GetCh(Text, index);
                        index++;
                    }
                    index++;
                    list2.Add(str);
                    break;

                default:
                    str = "";
                    while ((index < Text.Count<char>()) & (this.GetCh(Text, index) != " "))
                    {
                        str = str + this.GetCh(Text, index);
                        index++;
                    }
                    list2.Add(str);
                    break;
                }
            }
            return list2;
        }

        // Properties
        public new CommandLineArgument this[int Index]
        {
            get { return (CommandLineArgument) base[Index]; }
            set { base[Index] = value; }
        }
    }
}