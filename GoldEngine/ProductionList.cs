using System.Collections;
using System.Collections.Generic;

namespace GoldEngine
{
    public class ProductionList
    {
        private readonly List<Production> m_array;

        internal ProductionList()
        {
            m_array = new List<Production>();
        }

        internal ProductionList(int size)
        {
            m_array = new List<Production>();
            ReDimension(size);
        }

        public int Add(Production item)
        {
            m_array.Add(item);
            return m_array.Count - 1;
        }

        internal void Clear()
        {
            m_array.Clear();
        }

        public int Count()
        {
            return m_array.Count;
        }

        internal void ReDimension(int size)
        {
            m_array.Clear();
            for (int i = 0; i <= size; i++)
            {
                m_array.Add(null);
            }
        }

        public Production this[int index]
        {
            get { return m_array[index]; }
            set { m_array[index] = value; }
        }
    }
}