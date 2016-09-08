using System.Collections;

namespace GoldEngine
{
    public class SymbolList
    {
        // Fields
        private ArrayList m_Array;

        // Methods
        internal SymbolList()
        {
            this.m_Array = new ArrayList();
        }

        internal SymbolList(int Size)
        {
            this.m_Array = new ArrayList();
            this.ReDimension(Size);
        }

        internal int Add(Symbol Item)
        {
            return this.m_Array.Add(Item);
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        internal Symbol GetFirstOfType(SymbolType Type)
        {
            for (short i = 0; i < m_Array.Count; i = (short)(i + 1))
            {
                Symbol symbol3 = (Symbol)this.m_Array[i];
                if (symbol3.Type == Type)
                {
                    return symbol3;
                }
            }
            return null;
        }

        internal string IndexList(string Separator = ", ")
        {
            string left = "";
            if (this.m_Array.Count >= 1)
            {
                int num = 0;
                left = Conversions.ToString(((Symbol)this.m_Array[num]).TableIndex);
                int num2 = this.m_Array.Count - 1;
                for (num = 1; num <= num2; num++)
                {
                    left = Conversions.ToString(Operators.ConcatenateObject(left, Operators.ConcatenateObject(Separator, ((Symbol)this.m_Array[num]).TableIndex)));
                }
            }
            return left;
        }

        internal void ReDimension(int Size)
        {
            this.m_Array.Clear();
            int num2 = Size - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.m_Array.Add(null);
            }
        }

        public string Text(string Separator = ", ", bool AlwaysDelimitTerminals = false)
        {
            string left = "";
            int num2 = this.m_Array.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                Symbol symbol = (Symbol)this.m_Array[i];
                left = Conversions.ToString(Operators.ConcatenateObject(left, Operators.ConcatenateObject(i == 0 ? "" : Separator, symbol.Text(AlwaysDelimitTerminals))));
            }
            return left;
        }

        // Properties
        public Symbol this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    return (Symbol)this.m_Array[Index];
                }
                return null;
            }
            internal set
            {
                this.m_Array[Index] = value;
            }
        }
    }
}