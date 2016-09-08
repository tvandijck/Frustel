using System.Collections;

namespace GoldEngine
{
    internal class LRStateList : ArrayList
    {
        public short InitialState;

        public LRStateList()
        {
        }

        internal LRStateList(int Size)
        {
            this.ReDimension(Size);
        }

        public int Add(LRState Item)
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

        public new LRState this[int Index]
        {
            get { return (LRState) base[Index]; }
            set { base[Index] = value; }
        }
    }
}