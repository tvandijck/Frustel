using System.Collections;

namespace GoldEngine
{
    internal class FAAcceptList
    {
        // Fields
        private ArrayList m_Array = new ArrayList();

        // Methods
        public int Add(FAAccept Item)
        {
            return this.m_Array.Add(Item);
        }

        public int Add(short SymbolIndex, short Priority)
        {
            return this.m_Array.Add(new FAAccept(SymbolIndex, Priority));
        }

        public void Clear()
        {
            this.m_Array.Clear();
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        // Properties
        public FAAccept this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    return (FAAccept)this.m_Array[Index];
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