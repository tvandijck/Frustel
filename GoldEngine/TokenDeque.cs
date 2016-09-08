using System.Collections;

namespace GoldEngine
{
    internal class TokenDeque
    {
        // Fields
        private ArrayList m_Items = new ArrayList();

        // Methods
        public void Clear()
        {
            this.m_Items.Clear();
        }

        public Token Dequeue()
        {
            Token token2 = (Token)this.m_Items[0];
            this.m_Items.RemoveAt(0);
            return token2;
        }

        public void Enqueue(Token TheToken)
        {
            this.m_Items.Add(TheToken);
        }

        public Token Pop()
        {
            return this.Dequeue();
        }

        public void Push(Token TheToken)
        {
            this.m_Items.Insert(0, TheToken);
        }

        public Token Top()
        {
            if (this.m_Items.Count >= 1)
            {
                return (Token)this.m_Items[0];
            }
            return null;
        }

        // Properties
        internal int Count
        {
            get
            {
                return this.m_Items.Count;
            }
        }
    }
}