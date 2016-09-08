namespace GoldEngine
{
    internal class FAAccept
    {
        // Fields
        private short m_priority;
        private short m_symbolIndex;

        // Methods
        public FAAccept()
        {
            m_symbolIndex = -1;
        }

        public FAAccept(short symbolIndex, short priority)
        {
            m_symbolIndex = symbolIndex;
            m_priority = priority;
        }

        // Properties
        public short Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }

        public short SymbolIndex
        {
            get
            {
                return m_symbolIndex;
            }
            set
            {
                m_symbolIndex = value;
            }
        }
    }
}