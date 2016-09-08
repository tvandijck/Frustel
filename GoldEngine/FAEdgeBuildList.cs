namespace GoldEngine
{
    internal class FAEdgeBuildList : FAEdgeList
    {
        // Methods
        public int Add(FAEdgeBuild Item)
        {
            return base.Add(Item);
        }

        // Properties
        public new FAEdgeBuild this[int Index]
        {
            get
            {
                return (FAEdgeBuild)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}