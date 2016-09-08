namespace GoldEngine
{
    internal class LRAction
    {
        // Fields
        private Symbol m_Symbol;
        private LRActionType m_Type;
        private short m_Value;

        // Methods
        public LRAction()
        {
        }

        public LRAction(Symbol TheSymbol, LRActionType Type)
        {
            this.m_Symbol = TheSymbol;
            this.m_Type = Type;
            this.m_Value = 0;
        }

        public LRAction(Symbol TheSymbol, LRActionType Type, short Value)
        {
            this.m_Symbol = TheSymbol;
            this.m_Type = Type;
            this.m_Value = Value;
        }

        public LRConflict ConflictWith(LRActionType TypeB)
        {
            if ((this.m_Type == LRActionType.Reduce) & (TypeB == LRActionType.Reduce))
            {
                return LRConflict.ReduceReduce;
            }
            if ((this.m_Type == LRActionType.Shift) & (TypeB == LRActionType.Reduce))
            {
                return LRConflict.ShiftReduce;
            }
            if ((this.m_Type == LRActionType.Reduce) & (TypeB == LRActionType.Shift))
            {
                return LRConflict.ShiftReduce;
            }
            if ((this.m_Type == LRActionType.Accept) & (TypeB == LRActionType.Shift))
            {
                return LRConflict.AcceptReduce;
            }
            if ((this.m_Type == LRActionType.Reduce) & (TypeB == LRActionType.Accept))
            {
                return LRConflict.AcceptReduce;
            }
            return LRConflict.None;
        }

        public string Name()
        {
            switch (((int)this.m_Type))
            {
            case 1:
                return "Shift to State";

            case 2:
                return "Reduce Production";

            case 3:
                return "Go to State";

            case 4:
                return "Accept";

            case 5:
                return "Error";
            }
            return "";
        }

        public string NameShort()
        {
            switch (((int)this.m_Type))
            {
            case 1:
                return "s";

            case 2:
                return "r";

            case 3:
                return "g";

            case 4:
                return "a";

            case 5:
                return "Error";
            }
            return "";
        }

        public short SymbolIndex()
        {
            return this.m_Symbol.TableIndex;
        }

        public string Text()
        {
            switch (((int)this.m_Type))
            {
            case 1:
            case 2:
            case 3:
                return (this.m_Symbol.Text(false) + " " + this.Name() + " " + Conversions.ToString((int)this.m_Value));
            }
            return (this.m_Symbol.Text(false) + " " + this.Name());
        }

        public string TextShort()
        {
            switch (((int)this.m_Type))
            {
            case 1:
            case 2:
            case 3:
                return (this.m_Symbol.Text(false) + " " + this.NameShort() + " " + Conversions.ToString((int)this.m_Value));
            }
            return (this.m_Symbol.Text(false) + " " + this.NameShort());
        }

        public LRActionType Type()
        {
            return this.m_Type;
        }

        public short Value()
        {
            return this.m_Value;
        }

        // Properties
        public Symbol Symbol
        {
            get
            {
                return this.m_Symbol;
            }
            set
            {
                this.m_Symbol = value;
            }
        }
    }
}