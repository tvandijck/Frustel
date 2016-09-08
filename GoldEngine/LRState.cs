using System.Collections;

namespace GoldEngine
{
    internal class LRState : ArrayList
    {
        // Methods
        public void Add(LRAction Action)
        {
            base.Add(Action);
        }

        public short IndexOf(Symbol Item)
        {
            short num = 0;
            short num3 = 0;
            bool flag = false;
            while (!flag & (num3 < base.Count))
            {
                if (Item.IsEqualTo(((LRAction)base[num]).Symbol))
                {
                    num = num3;
                    flag = true;
                }
                num3 = (short)(num3 + 1);
            }
            if (flag)
            {
                return num;
            }
            return -1;
        }

        // Properties
        public LRAction this[short Index]
        {
            get
            {
                return (LRAction)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }

        public LRAction this[Symbol Sym]
        {
            get
            {
                int index = this.IndexOf(Sym);
                if (index != -1)
                {
                    return (LRAction)base[index];
                }
                return null;
            }
            set
            {
                int index = this.IndexOf(Sym);
                if (index != -1)
                {
                    base[index] = value;
                }
            }
        }
    }
}