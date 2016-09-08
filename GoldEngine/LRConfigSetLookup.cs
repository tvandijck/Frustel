using System;
using System.Runtime.InteropServices;

namespace GoldEngine
{
    internal sealed class LRConfigSetLookup
    {
        // Fields
        private static int m_Count;
        private static ConfigSetLookupItem[] m_List;

        // Methods
        public static void Add(LRConfigSet Key, int TableIndex = 0)
        {
            int index = InsertIndex(Key);
            if (index != -1)
            {
                ConfigSetLookupItem item;
                item.Key = Key;
                item.TableIndex = TableIndex;
                m_Count++;
                m_List = (ConfigSetLookupItem[])Utils.CopyArray((Array)m_List, new ConfigSetLookupItem[(m_Count - 1) + 1]);
                int num3 = index + 1;
                for (int i = m_Count - 1; i >= num3; i += -1)
                {
                    m_List[i] = m_List[i - 1];
                }
                m_List[index] = item;
            }
        }

        public static void Clear()
        {
            m_List = null;
            m_Count = 0;
        }

        public static int Count()
        {
            return m_Count;
        }

        private static int InsertIndex(LRConfigSet Key)
        {
            if (m_Count == 0)
            {
                return 0;
            }
            if (Key.IsLessThan(m_List[0].Key))
            {
                return 0;
            }
            if (Key.IsGreaterThan(m_List[m_Count - 1].Key))
            {
                return m_Count;
            }
            int num5 = m_Count - 1;
            int num3 = 0;
            int num = -1;
            bool flag = false;
            do
            {
                int index = (int)Math.Round((double)(((double)(num3 + num5)) / 2.0));
                if (num3 > num5)
                {
                    num = num3;
                    flag = true;
                }
                else if (m_List[index].Key.IsEqualTo(Key))
                {
                    num = -1;
                    flag = true;
                }
                else if (m_List[index].Key.IsLessThan(Key))
                {
                    num3 = index + 1;
                }
                else
                {
                    num5 = index - 1;
                }
            }
            while (!flag);
            return num;
        }

        private static int ItemIndex(LRConfigSet Key)
        {
            if (m_Count == 0)
            {
                return -1;
            }
            if (Key.IsLessThan(m_List[0].Key) | Key.IsGreaterThan(m_List[m_Count - 1].Key))
            {
                return -1;
            }
            int num5 = m_Count - 1;
            int num3 = 0;
            int num = -1;
            bool flag = false;
            do
            {
                int index = (int)Math.Round((double)(((double)(num3 + num5)) / 2.0));
                if (num3 > num5)
                {
                    flag = true;
                }
                else if (m_List[index].Key.IsEqualTo(Key))
                {
                    num = index;
                    flag = true;
                }
                else if (num3 == num5)
                {
                    flag = true;
                }
                else if (m_List[index].Key.IsLessThan(Key))
                {
                    num3 = index + 1;
                }
                else
                {
                    num5 = index - 1;
                }
            }
            while (!flag);
            return num;
        }

        // Properties
        public int this[LRConfigSet Key]
        {
            get
            {
                int index = ItemIndex(Key);
                if (index != -1)
                {
                    return m_List[index].TableIndex;
                }
                return -1;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct ConfigSetLookupItem
        {
            public LRConfigSet Key;
            public int TableIndex;
        }
    }
}