namespace GoldEngine
{
    internal class FAEdge
    {
        // Fields
        private CharacterSet m_Chars;
        private int m_Target;

        // Methods
        public FAEdge()
        {
        }

        public FAEdge(CharacterSet CharSet, int Target)
        {
            this.m_Chars = CharSet;
            this.m_Target = Target;
        }

        public bool Contains(int CharCode)
        {
            return this.Characters.Contains(CharCode);
        }

        // Properties
        public CharacterSet Characters
        {
            get
            {
                return this.m_Chars;
            }
            set
            {
                this.m_Chars = value;
            }
        }

        public int Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }
    }
}