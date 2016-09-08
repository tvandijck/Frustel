namespace GoldEngine
{
    public class Reduction : TokenList
    {
        private Production m_parent;
        private object m_tag;

        internal Reduction(int size)
        {
            ReDimension(size);
        }

        internal void ReDimension(int size)
        {
            Clear();
            int num2 = size - 1;
            for (int i = 0; i <= num2; i++)
            {
                Add(null);
            }
        }

        public new object this[int index]
        {
            get { return base[index].Data; }
            set { base[index].Data = value; }
        }

        public Production Parent
        {
            get { return m_parent; }
            internal set { m_parent = value; }
        }

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }
    }
}