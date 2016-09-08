using System.Collections;

namespace GoldEngine
{
    internal class NumberSetList
    {
        // Fields
        private ArrayList m_Array;

        // Methods
        public NumberSetList()
        {
            this.m_Array = new ArrayList();
        }

        public NumberSetList(int Size)
        {
            this.m_Array = new ArrayList(Size);
        }

        public int Add(NumberSet Item)
        {
            return this.m_Array.Add(Item);
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        // Properties
        public NumberSet this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    return (NumberSet)this.m_Array[Index];
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
    }
}