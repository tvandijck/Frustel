using System.Collections;

namespace GoldEngine
{
    internal class RegExpSeq
    {
        // Fields
        private ArrayList m_Array = new ArrayList();
        private short m_Priority;

        // Methods
        public void Add(RegExpItem Item)
        {
            this.m_Array.Add(Item);
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        public bool IsVariableLength()
        {
            bool flag2 = false;
            for (short i = 0; (i < this.m_Array.Count) & !flag2; i = (short)(i + 1))
            {
                RegExpItem item = (RegExpItem)this.m_Array[i];
                if (item.IsVariableLength())
                {
                    flag2 = true;
                }
            }
            return flag2;
        }

        public override string ToString()
        {
            string str = "";
            if (this.m_Array.Count >= 1)
            {
                str = this.m_Array[0].ToString();
                int num2 = this.m_Array.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    str = str + " " + this.m_Array[i].ToString();
                }
            }
            return str;
        }

        // Properties
        public RegExpItem this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    return (RegExpItem)this.m_Array[Index];
                }
                return null;
            }
            set
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    this.m_Array[Index] = value;
                }
            }
        }

        internal short Priority
        {
            get
            {
                return this.m_Priority;
            }
            set
            {
                this.m_Priority = value;
            }
        }
    }
}