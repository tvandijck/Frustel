namespace GoldEngine
{
    internal class CharacterSetBuildList : CharacterSetList
    {
        // Methods
        public CharacterSetBuildList()
        {
        }

        public CharacterSetBuildList(int Size) : base(Size)
        {
        }

        public int Add(CharacterSetBuild Item)
        {
            CharacterSet item = Item;
            Item = (CharacterSetBuild)item;
            return base.Add(item);
        }

        // Properties
        public new CharacterSetBuild this[int Index]
        {
            get
            {
                return (CharacterSetBuild)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}