using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoldEngine
{
    public class Token
    {
        private object m_data;
        private Symbol m_parent;
        private Position m_position;
        private short m_state;

        internal Token()
        {
            m_position = new Position();
            m_parent = null;
            m_data = null;
            m_state = 0;
        }

        public Token(Symbol parent, object data)
        {
            m_position = new Position();
            m_parent = parent;
            m_data = data;
            m_state = 0;
        }

        internal Group Group()
        {
            return m_parent.Group;
        }

        public string Name()
        {
            return m_parent.Name;
        }

        public short TableIndex()
        {
            return m_parent.TableIndex;
        }

        public string Text()
        {
            return m_parent.Text(false);
        }

        public SymbolType Type()
        {
            return m_parent.Type;
        }

        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public Symbol Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public Position Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        internal short State
        {
            get { return m_state; }
            set { m_state = value; }
        }
    }
}
