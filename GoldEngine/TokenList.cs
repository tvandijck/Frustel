using System.Collections;

namespace GoldEngine
{
    public class TokenList
    {
        // Fields
        private ArrayList m_Array = new ArrayList();

        // Methods
        internal TokenList()
        {
        }

        internal int Add(Token Item)
        {
            return this.m_Array.Add(Item);
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        // Properties
        public Token this[int Index]
        {
            get
            {
                return (Token)this.m_Array[Index];
            }
            internal set
            {
                this.m_Array[Index] = value;
            }
        }
    }
}