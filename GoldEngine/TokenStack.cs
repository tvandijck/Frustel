using System.Collections;

namespace GoldEngine
{
    internal class TokenStack
    {
        // Fields
        private Stack m_Stack = new Stack();

        // Methods
        public void Clear()
        {
            this.m_Stack.Clear();
        }

        public Token Pop()
        {
            return (Token)this.m_Stack.Pop();
        }

        public void Push(Token TheToken)
        {
            this.m_Stack.Push(TheToken);
        }

        public Token Top()
        {
            return (Token)this.m_Stack.Peek();
        }

        // Properties
        internal int Count
        {
            get
            {
                return this.m_Stack.Count;
            }
        }
    }
}