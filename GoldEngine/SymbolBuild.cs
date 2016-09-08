namespace GoldEngine
{
    internal class SymbolBuild : Symbol
    {
        // Fields
        public bool Accepted;
        public CreatorType CreatedBy;
        public LookaheadSymbolSet First;
        public string LinkName;
        public bool Nullable;
        public LRConfigSet PartialClosure;
        public bool Reclassified;
        public RegExp RegularExp;
        public bool UsesDFA;
        public bool VariableLength;

        // Methods
        internal SymbolBuild()
        {
            this.First = new LookaheadSymbolSet();
        }

        internal SymbolBuild(string Name, SymbolType Type) : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = this.ImpliedDFAUsage(Type) > SymbolType.Nonterminal;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, RegExp Exp) : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.RegularExp = Exp;
            this.UsesDFA = true;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, bool UsesDFA) : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = UsesDFA;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, int TableIndex) : base(Name, Type, (short)TableIndex)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = true;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, bool UsesDFA, CreatorType CreatedBy) : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = UsesDFA;
            this.CreatedBy = CreatedBy;
            this.Reclassified = false;
        }

        internal SymbolCategory Category()
        {
            switch (this.Type)
            {
                case SymbolType.Nonterminal:
                    return SymbolCategory.Nonterminal;

                case SymbolType.Content:
                case SymbolType.Noise:
                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    return SymbolCategory.Terminal;

                case SymbolType.End:
                case SymbolType.Error:
                    return SymbolCategory.Special;
            }
            return SymbolCategory.Nonterminal;
        }

        internal string CreatedByName()
        {
            switch (this.CreatedBy)
            {
                case CreatorType.Defined:
                    return "Defined in Grammar";

                case CreatorType.Generated:
                    return "Generated";

                case CreatorType.Implicit:
                    return "Implicitly Defined";
            }
            return "";
        }

        private SymbolType ImpliedDFAUsage(SymbolType Type)
        {
            switch (((int)Type))
            {
                case 1:
                case 2:
                case 4:
                case 5:
                    return ~SymbolType.Nonterminal;
            }
            return SymbolType.Nonterminal;
        }

        internal bool IsFormalTerminal()
        {
            switch (((int)this.Type))
            {
                case 1:
                case 4:
                case 5:
                    return true;
            }
            return false;
        }

        public bool IsLessThan(SymbolBuild Symbol2)
        {
            short num = this.SymbolKindValue(base.Type);
            short num2 = this.SymbolKindValue(Symbol2.Type);
            if (num == num2)
            {
                return (Operators.CompareString(base.Name.ToUpper(), Symbol2.Name.ToUpper(), false) < 0);
            }
            return (num < num2);
        }

        public bool NeedsDeclaration()
        {
            return (this.UsesDFA & (this.RegularExp == null));
        }

        private short SymbolKindValue(SymbolType type)
        {
            switch (type)
            {
                case SymbolType.Nonterminal:
                    return 5;

                case SymbolType.Content:
                    return 4;

                case SymbolType.Noise:
                    return 2;

                case SymbolType.End:
                case SymbolType.Error:
                    return 1;

                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    return 3;

                case SymbolType.LEGACYCommentLine:
                    return 0;
            }
            return 0;
        }

        // Properties
        public new GroupBuild Group
        {
            get { return (GroupBuild) base.Group; }
            set { base.Group = value; }
        }
    }
}