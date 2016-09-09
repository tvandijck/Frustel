using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel
{
    public class Production
    {
        private Symbol m_head;
        private Symbol[] m_handles;
        private int m_tableIndex = -1;

        public Production()
        {
        }

        public Production(Symbol head, params Symbol[] handles)
        {
            m_head = head;
            m_handles = handles;
        }

        public Symbol Head
        {
            get { return m_head; }
        }

        public Symbol[] Handles
        {
            get { return m_handles; }
        }

        public int TableIndex
        {
            get { return m_tableIndex; }
            internal set { m_tableIndex = value; }
        }
    }
}
