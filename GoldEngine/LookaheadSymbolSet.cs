namespace GoldEngine
{
    internal class LookaheadSymbolSet : DictionarySet
    {
        public LookaheadSymbolSet()
        {
        }

        public LookaheadSymbolSet(LookaheadSymbolSet A, LookaheadSymbolSet B)
            : base(A, B)
        {
        }

        public bool Add(LookaheadSymbol item)
        {
            return base.Add(item);
        }

        public LookaheadSymbol ByKey(short TableIndex)
        {
            return (LookaheadSymbol)base.ByKey(TableIndex);
        }

        public string Text()
        {
            string str = "";
            if (base.Count() >= 1)
            {
                str = this[0].Parent.Text(false);
                short num2 = (short)(base.Count() - 1);
                for (short i = 1; i <= num2; i = (short)(i + 1))
                {
                    str = str + " " + this[i].Parent.Text(false);
                }
            }
            return str;
        }

        public LookaheadSymbolSet Union(LookaheadSymbolSet SetB)
        {
            return new LookaheadSymbolSet(this, SetB);
        }

        public bool UnionWith(LookaheadSymbolSet SetB)
        {
            return base.UnionWith(SetB);
        }

        public new LookaheadSymbol this[int Index]
        {
            get { return (LookaheadSymbol)base[Index]; }
        }
    }


}