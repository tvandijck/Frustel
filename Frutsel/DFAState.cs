using System.Collections.Generic;

namespace Frutsel
{
    public struct Edge
    {
        public int Target;
        public CharacterSet Characters;

        public Edge(int target, CharacterSet characterSet)
        {
            Target = target;
            Characters = (characterSet?.Count > 0) ? characterSet : null;
        }
    }

    public class DFAState
    {
        private readonly List<Edge> m_edges = new List<Edge>();
        private int m_tableIndex;
        private Symbol m_accept;

        public int TableIndex
        {
            get { return m_tableIndex; }
            set { m_tableIndex = value; }
        }

        public Symbol Accept
        {
            get { return m_accept; }
            set { m_accept = value; }
        }

        public List<Edge> Edges
        {
            get { return m_edges; }
        }
    }
}