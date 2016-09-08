namespace GoldEngine
{
    internal class LRStateBuild : LRState
    {
        // Fields
        public LRConfigSet ConfigSet = new LRConfigSet();
        public LRConflictItemList ConflictList = new LRConflictItemList();
        public bool Expanded = false;
        public bool Modified = false;
        public string Note = "";
        public NumberSet PriorStates = new NumberSet(new int[0]);
        public LRStatus Status = LRStatus.Info;

        // Methods
        private LRConflict ActionAdd(SymbolBuild TheSymbol, LRActionType Type, short Value = 0)
        {
            bool flag2 = false;
            bool flag = false;
            LRConflict none = LRConflict.None;
            for (short i = 0; ((i < base.Count) & !flag2) & !flag; i = (short)(i + 1))
            {
                LRAction action = base[i];
                if (action.Symbol.IsEqualTo(TheSymbol))
                {
                    if ((action.Type() == Type) & (action.Value() == Value))
                    {
                        flag = true;
                    }
                    else
                    {
                        none = BuildLR.GetConflict(action.Type(), Type);
                        flag2 = true;
                    }
                }
            }
            if (!flag)
            {
                base.Add(new LRAction(TheSymbol, Type, Value));
            }
            return none;
        }

        public new LRConflict Add(LRAction Action)
        {
            LRAction action = Action;
            SymbolBuild symbol = (SymbolBuild)action.Symbol;
            action.Symbol = symbol;
            return this.ActionAdd(symbol, Action.Type(), Action.Value());
        }

        public LRConflict ConflictForAction(SymbolBuild TheSymbol, LRActionType Type, short Value)
        {
            bool flag = false;
            LRConflict none = LRConflict.None;
            for (short i = 0; (i < base.Count) & !flag; i = (short)(i + 1))
            {
                LRAction action = base[i];
                if (action.Symbol.IsEqualTo(TheSymbol))
                {
                    if ((action.Type() == Type) & (action.Value() == Value))
                    {
                        none = LRConflict.None;
                    }
                    else
                    {
                        none = BuildLR.GetConflict(action.Type(), Type);
                    }
                    flag = true;
                }
            }
            return none;
        }
    }
}