namespace GoldEngine
{
    internal class FAStateBuildList : FAStateList
    {
        // Methods
        public FAStateBuildList()
        {
        }

        public FAStateBuildList(int Size) : base(Size)
        {
        }

        public int Add(FAStateBuild Item)
        {
            return base.Add(Item);
        }

        public new FAStateBuild this[int Index]
        {
            get { return (FAStateBuild) base[Index]; }
            set { base[Index] = value; }
        }
    }
}