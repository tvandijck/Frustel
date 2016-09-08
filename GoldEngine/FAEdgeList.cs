using System.Collections;

namespace GoldEngine
{
    internal class FAEdgeList
    {
        // Fields
        private ArrayList m_List = new ArrayList();

        // Methods
        public int Add(FAEdge Edge)
        {
            return this.m_List.Add(Edge);
        }

        public int Count()
        {
            return this.m_List.Count;
        }

        // Properties
        public FAEdge this[int Index]
        {
            get
            {
                return (FAEdge)this.m_List[Index];
            }
            set
            {
                this.m_List[Index] = value;
            }
        }
    }
}