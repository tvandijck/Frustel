namespace GoldEngine
{
    internal class SymbolBuildList : SymbolList
    {
        // Methods
        public SymbolBuildList()
        {
        }

        public SymbolBuildList(int Size) : base(Size)
        {
        }

        public int Add(SymbolBuild Item)
        {
            return base.Add(Item);
        }

        public SymbolBuild AddUnique(SymbolBuild Item)
        {
            short num = this.ItemIndex(Item);
            if (num == -1)
            {
                num = (short)base.Add(Item);
            }
            else
            {
                SymbolBuild build2 = (SymbolBuild)base[num];
                if (build2.RegularExp == null)
                {
                    build2.RegularExp = Item.RegularExp;
                    build2.Type = Item.Type;
                }
            }
            return (SymbolBuild)base[num];
        }

        internal bool Contains(string Name)
        {
            return (this.ItemIndex(Name) != -1);
        }

        internal short ItemIndex(SymbolBuild Search)
        {
            return this.ItemIndex(Search.Name, Search.Type);
        }

        internal short ItemIndex(string Name)
        {
            int num = -1;
            for (int i = 0; (i < base.Count()) & (num == -1); i++)
            {
                Symbol symbol = base[i];
                if (symbol.Name.ToUpper() == Name.ToUpper())
                {
                    num = i;
                }
            }
            return (short)num;
        }

        internal short ItemIndex(string Name, SymbolType Type)
        {
            int num = -1;
            for (int i = 0; (i < base.Count()) & (num == -1); i++)
            {
                SymbolBuild build = (SymbolBuild)base[i];
                if ((build.Name.ToUpper() == Name.ToUpper()) & (build.Type == Type))
                {
                    num = i;
                }
            }
            return (short)num;
        }

        internal short ItemIndexCategory(string Name, SymbolCategory Category)
        {
            int num = -1;
            for (int i = 0; (i < base.Count()) & (num == -1); i++)
            {
                SymbolBuild build = (SymbolBuild)base[i];
                if ((build.Name.ToUpper() == Name.ToUpper()) & (build.Category() == Category))
                {
                    num = i;
                }
            }
            return (short)num;
        }

        public string Names(string Separator = ", ", string NamePrefix = "'", string NamePostfix = "'")
        {
            string name = "";
            if (base.Count() >= 1)
            {
                name = base[0].Name;
                int num2 = base.Count() - 1;
                for (int i = 1; i <= num2; i++)
                {
                    name = name + Separator + base[i].Name;
                }
            }
            return name;
        }

        internal short NonterminalIndex(string Name)
        {
            int num = -1;
            for (int i = 0; (i < base.Count()) & (num == -1); i++)
            {
                Symbol symbol = base[i];
                if ((symbol.Name.ToUpper() == Name.ToUpper()) & (symbol.Type == SymbolType.Nonterminal))
                {
                    num = i;
                }
            }
            return (short)num;
        }

        internal short TerminalIndex(string Name)
        {
            int num = -1;
            for (int i = 0; (i < base.Count()) & (num == -1); i++)
            {
                Symbol symbol = base[i];
                if ((symbol.Name.ToUpper() == Name.ToUpper()) & (symbol.Type != SymbolType.Nonterminal))
                {
                    num = i;
                }
            }
            return (short)num;
        }

        public override string ToString()
        {
            return this.Text(", ", false);
        }

        // Properties
        public new SymbolBuild this[int Index]
        {
            get { return (SymbolBuild) base[Index]; }
            set { base[Index] = value; }
        }
    }
}