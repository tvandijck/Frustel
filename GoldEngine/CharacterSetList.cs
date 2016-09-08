using System.Collections;

namespace GoldEngine
{
    internal class CharacterSetList : ArrayList
    {
        // Methods
        public CharacterSetList()
        {
        }

        internal CharacterSetList(int Size)
        {
            this.ReDimension(Size);
        }

        public int Add(CharacterSet Item)
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

        // Properties
        public new CharacterSet this[int Index]
        {
            get
            {
                return (CharacterSet)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}