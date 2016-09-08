using System;

namespace GoldEngine
{
    internal sealed class UnicodeTable
    {
        // Fields
        private static UnicodeMapTable m_LowerCaseTable = new UnicodeMapTable();
        private static UnicodeMapTable m_UpperCaseTable = new UnicodeMapTable();
        private static UnicodeMapTable m_Win1252Table = new UnicodeMapTable();

        // Methods
        private static void AddCase(int UppercaseCode, int LowercaseCode, string Name)
        {
            m_LowerCaseTable.Add(LowercaseCode, UppercaseCode);
            m_UpperCaseTable.Add(UppercaseCode, LowercaseCode);
        }

        private static void AddWin1252(int CharCode, int Mapping)
        {
            m_Win1252Table.Add(CharCode, Mapping);
            m_Win1252Table.Add(Mapping, CharCode);
        }

        public static bool IsWin1252(int CharCode)
        {
            return (m_Win1252Table.Contains(CharCode) || (((CharCode >= 0x20) & (CharCode <= 0x7e)) | ((CharCode >= 160) & (CharCode <= 0xff))));
        }

        private static void LoadMapping()
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            try
            {
                reader.Open("mapping.dat");
                if (Operators.CompareString(reader.Header(), "GOLD Character Mapping", true) != 0)
                {
                    //BuilderApp.Log.Add(SysLogSection.Internal, SysLogAlert.Critical, "The file 'mapping.dat' is invalid.");
                }
                else
                {
                    while (!reader.EndOfFile())
                    {
                        reader.GetNextRecord();
                        string str = reader.RetrieveString();
                        int uppercaseCode = reader.RetrieveInt16();
                        int lowercaseCode = reader.RetrieveInt16();
                        string left = str;
                        if (Operators.CompareString(left, "C", true) == 0)
                        {
                            AddCase(uppercaseCode, lowercaseCode, "");
                        }
                        else if (Operators.CompareString(left, "W", true) == 0)
                        {
                            AddWin1252(uppercaseCode, lowercaseCode);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                //BuilderApp.Log.Add(SysLogSection.Internal, SysLogAlert.Critical, exception.Message);
                //ProjectData.ClearProjectError();
            }
        }

        public static void Setup()
        {
            LoadMapping();
        }

        public static int ToLowerCase(int CharCode)
        {
            int index = m_UpperCaseTable.IndexOf(CharCode);
            if (index == -1)
            {
                return CharCode;
            }
            return m_UpperCaseTable[index].Map;
        }

        public static int ToUpperCase(int CharCode)
        {
            int index = m_LowerCaseTable.IndexOf(CharCode);
            if (index == -1)
            {
                return CharCode;
            }
            return m_LowerCaseTable[index].Map;
        }

        public static int ToWin1252(int CharCode)
        {
            int index = m_Win1252Table.IndexOf(CharCode);
            if (index == -1)
            {
                return CharCode;
            }
            return m_Win1252Table[index].Map;
        }
    }
}