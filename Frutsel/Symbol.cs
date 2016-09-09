namespace Frutsel
{
    public abstract class Symbol
    {
        private int m_tableIndex = -1;

        public string Name { get; set; }
        public Group Group { get; set; }

        public int TableIndex
        {
            get { return m_tableIndex; }
            internal set { m_tableIndex = value; }
        }

        public override string ToString()
        {
            return $"{GetType().Name}({Name})";
        }

        public abstract ESymbolType Type { get; }
    }
}
