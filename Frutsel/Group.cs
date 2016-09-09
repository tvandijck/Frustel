using System.Collections.Generic;

namespace Frutsel
{
    public class Group
    {
        private readonly List<Group> m_nesting = new List<Group>();
        private int m_tableIndex = -1;

        public string Name { get; set; }
        public Symbol Container { get; set; }
        public Symbol Start { get; set; }
        public Symbol End { get; set; }
        public EAdvanceMode Advance { get; set; }
        public EEndingMode Ending { get; set; }

        public List<Group> Nesting
        {
            get { return m_nesting; }
        }

        public int TableIndex
        {
            get { return m_tableIndex; }
            internal set { m_tableIndex = value; }
        }
    }
}
