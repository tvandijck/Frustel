namespace GoldEngine
{
    public class Production
    {
        // Fields
        protected SymbolList MyHandle;
        protected Symbol MyHead;
        protected short MyTableIndex;

        // Methods
        internal Production()
        {
            this.MyHandle = new SymbolList();
            this.MyTableIndex = -1;
        }

        internal Production(Symbol Head)
        {
            this.MyHead = Head;
            this.MyHandle = new SymbolList();
            this.MyTableIndex = -1;
        }

        internal Production(Symbol Head, short TableIndex)
        {
            this.MyHead = Head;
            this.MyHandle = new SymbolList();
            this.MyTableIndex = TableIndex;
        }

        internal bool ContainsOneNonTerminal()
        {
            return ((this.MyHandle.Count() == 1) && (this.MyHandle[0].Type == SymbolType.Nonterminal));
        }

        internal string Definition()
        {
            string str = "";
            short num2 = (short)(this.MyHandle.Count() - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                str = str + this.MyHandle[i].Text(false) + " ";
            }
            return Strings.RTrim(str);
        }

        internal bool Equals(Production SecondRule)
        {
            if ((this.MyHandle.Count() == SecondRule.Handle().Count()) & this.MyHead.IsEqualTo(SecondRule.Head))
            {
                bool flag = true;
                for (short i = 0; flag & (i < this.MyHandle.Count()); i = (short)(i + 1))
                {
                    flag = this.MyHandle[i].IsEqualTo(SecondRule.Handle()[i]);
                }
                return flag;
            }
            return false;
        }

        public SymbolList Handle()
        {
            return this.MyHandle;
        }

        internal string Name()
        {
            return ("<" + this.MyHead.Name + ">");
        }

        internal void SetHandle(SymbolList Symbols)
        {
            this.MyHandle = Symbols;
        }

        internal void SetHead(Symbol NonTerminal)
        {
            this.MyHead = NonTerminal;
        }

        internal void SetTableIndex(short Value)
        {
            this.MyTableIndex = Value;
        }

        public string Text()
        {
            return (this.Name() + " ::= " + this.Definition());
        }

        public override string ToString()
        {
            return this.Text();
        }

        // Properties
        public Symbol Head
        {
            get
            {
                return this.MyHead;
            }
        }

        public short TableIndex
        {
            get
            {
                return this.MyTableIndex;
            }
        }
    }
}