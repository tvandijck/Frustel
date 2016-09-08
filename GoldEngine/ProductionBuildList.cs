namespace GoldEngine
{
    internal class ProductionBuildList : ProductionList
    {
        public ProductionBuildList()
        {
        }

        public ProductionBuildList(int size)
            : base(size)
        {
        }

        public int Add(ProductionBuild item)
        {
            return base.Add(item);
        }

        public new ProductionBuild this[int index]
        {
            get { return (ProductionBuild) base[index]; }
            set { base[index] = value; }
        }
    }
}