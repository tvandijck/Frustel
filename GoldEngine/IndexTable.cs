using System;
using System.Runtime.CompilerServices;

namespace GoldEngine
{
    public class IndexTable
    {
        // Fields
        private int m_Count;
        private IndexTableEntry[] m_List;

        // Methods
        public void Add(int Key, object Data)
        {
            bool flag;
            short num;
            short num2;
            if (this.m_Count == 0)
            {
                num = -1;
                flag = false;
            }
            else if (this.m_List[this.m_Count - 1].Key < Key)
            {
                num = -1;
                flag = false;
            }
            else
            {
                num2 = 0;
                num = -1;
                flag = false;
                while (!(((num2 >= this.m_Count) | (num != -1)) | flag))
                {
                    if (this.m_List[num2].Key == Key)
                    {
                        flag = true;
                    }
                    else if (this.m_List[num2].Key > Key)
                    {
                        num = num2;
                    }
                    num2 = (short)(num2 + 1);
                }
            }
            if (!flag)
            {
                IndexTableEntry entry = new IndexTableEntry(Key, RuntimeHelpers.GetObjectValue(Data));
                this.m_Count++;
                this.m_List = (IndexTableEntry[])Utils.CopyArray((Array)this.m_List, new IndexTableEntry[(this.m_Count - 1) + 1]);
                if (num == -1)
                {
                    this.m_List[this.m_Count - 1] = entry;
                }
                else
                {
                    short num3 = (short)(num + 1);
                    for (num2 = (short)(this.m_Count - 1); num2 >= num3; num2 = (short)(num2 + -1))
                    {
                        this.m_List[num2] = this.m_List[num2 - 1];
                    }
                    this.m_List[num] = entry;
                }
            }
        }

        public void Clear()
        {
            this.m_List = null;
            this.m_Count = 0;
        }

        public bool Contains(int Key)
        {
            return (this.ItemIndex(Key) != -1);
        }

        public int Count()
        {
            return this.m_Count;
        }

        internal IndexTableEntry ItemArrayIndex(int Index)
        {
            return this.m_List[Index];
        }

        public int ItemIndex(int Key)
        {
            if (this.m_Count == 0)
            {
                return -1;
            }
            if ((Key < this.m_List[0].Key) | (Key > this.m_List[this.m_Count - 1].Key))
            {
                return -1;
            }
            int num5 = this.m_Count - 1;
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
                else if (this.m_List[index].Key == Key)
                {
                    num = index;
                    flag = true;
                }
                else if (this.m_List[index].Key < Key)
                {
                    num3 = index + 1;
                }
                else if (this.m_List[index].Key > Key)
                {
                    num5 = index - 1;
                }
            }
            while (!flag);
            return num;
        }

        // Properties
        public IndexTableEntry Item(int Key)
        {
            int index = this.ItemIndex(Key);
            if (index != -1)
            {
                return this.m_List[index];
            }
            return null;
        }

        public IndexTableEntry ItemByIndex(int Index)
        {
            if ((Index >= 0) & (Index < this.m_Count))
            {
                return this.m_List[Index];
            }
            return null;
        }
    }
}