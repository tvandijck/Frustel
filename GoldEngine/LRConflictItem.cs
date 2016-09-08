namespace GoldEngine
{
    internal class LRConflictItem
    {
        // Fields
        public LRConflict Conflict;
        public LRConfigSet Reduces;
        public LRConfigSet Shifts;
        public SymbolBuild Symbol;

        // Methods
        public LRConflictItem(SymbolBuild Symbol)
        {
            this.Symbol = Symbol;
            this.Conflict = LRConflict.None;
            this.Shifts = new LRConfigSet();
            this.Reduces = new LRConfigSet();
        }

        public LRConflictItem(LRConflictItem Item, LRConflict Status)
        {
            this.Symbol = Item.Symbol;
            this.Conflict = Status;
            this.Shifts = Item.Shifts;
            this.Reduces = Item.Reduces;
        }
    }
}