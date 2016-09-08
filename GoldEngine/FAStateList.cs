using System.Collections;

namespace GoldEngine
{
    internal class FAStateList : ArrayList
    {
        // Fields
        public Symbol EndSymbol;
        public Symbol ErrorSymbol;
        public short InitialState;

        // Methods
        public FAStateList()
        {
        }

        internal FAStateList(int Size)
        {
            this.ReDimension(Size);
        }

        public int Add(FAState Item)
        {
            return base.Add(Item);
        }

        internal void ReDimension(int Size)
        {
            base.Clear();
            int num2 = Size - 1;
            for (int i = 0; i <= num2; i++)
            {
                base.Add(null);
            }
        }

        public new FAState this[int Index]
        {
            get { return (FAState) base[Index]; }
            set { base[Index] = value; }
        }
    }
}