namespace GoldEngine
{
    internal class LRStateBuildList : LRStateList
    {
        public LRStateBuildList()
        {
        }

        public LRStateBuildList(int Size) : base(Size)
        {
        }

        public int Add(LRStateBuild Item)
        {
            LRState item = Item;
            Item = (LRStateBuild)item;
            return base.Add(item);
        }

        public new LRStateBuild this[int Index]
        {
            get { return (LRStateBuild) base[Index]; }
            set { base[Index] = value; }
        }
    }
}