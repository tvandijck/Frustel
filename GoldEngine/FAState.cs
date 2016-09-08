namespace GoldEngine
{
    internal class FAState
    {
        // Fields
        internal FAEdgeList EdgeList;
        private Symbol m_Accept;
        private short m_TableIndex;

        // Methods
        public FAState()
        {
            this.m_Accept = null;
            this.EdgeList = new FAEdgeList();
        }

        public FAState(Symbol Accept)
        {
            this.m_Accept = Accept;
            this.EdgeList = new FAEdgeList();
        }

        public int AcceptIndex()
        {
            if (this.m_Accept == null)
            {
                return -1;
            }
            return this.m_Accept.TableIndex;
        }

        public void AddEdge(FAEdge Edge)
        {
            if (Edge.Characters.Count() == 0)
            {
                this.EdgeList.Add(Edge);
            }
            else
            {
                short num = -1;
                for (short i = 0; (i < this.EdgeList.Count()) & (num == -1); i = (short)(i + 1))
                {
                    if (this.EdgeList[i].Target == Edge.Target)
                    {
                        num = i;
                    }
                }
                if (num == -1)
                {
                    this.EdgeList.Add(Edge);
                }
                else
                {
                    this.EdgeList[num].Characters.UnionWith(Edge.Characters);
                }
            }
        }

        public void AddEdge(CharacterSet CharSet, int Target)
        {
            this.AddEdge(new FAEdge(CharSet, Target));
        }

        public FAEdgeList Edges()
        {
            return this.EdgeList;
        }

        // Properties
        public Symbol Accept
        {
            get
            {
                return this.m_Accept;
            }
            set
            {
                this.m_Accept = value;
            }
        }
    }
}