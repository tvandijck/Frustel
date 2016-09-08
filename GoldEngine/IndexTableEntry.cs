namespace GoldEngine
{
    public class IndexTableEntry
    {
        // Fields
        private object m_Data;
        private long m_Key;

        // Methods
        public IndexTableEntry()
        {
            this.m_Key = 0L;
        }

        public IndexTableEntry(int Key, object Data)
        {
            this.m_Key = Key;
            this.m_Data = Data;
        }

        // Properties
        public object Data
        {
            get { return this.m_Data; }
            set { this.m_Data = value; }
        }

        public int Key
        {
            get { return (int) this.m_Key; }
            set { this.m_Key = value; }
        }
    }
}