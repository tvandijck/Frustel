using System.Collections;

namespace GoldEngine
{
    internal class GroupList : ArrayList
    {
        // Methods
        public GroupList()
        {
        }

        internal GroupList(int Size)
        {
            this.ReDimension(Size);
        }

        public int Add(Group Item)
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
        public new Group this[int Index]
        {
            get
            {
                return (Group)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}