using System.Collections;

namespace GoldEngine
{
    internal class LRConflictItemList : ArrayList
    {
        internal int Add(LRConflictItem item)
        {
            return base.Add(item);
        }

        internal new LRConflictItem this[int index]
        {
            get { return (LRConflictItem) base[index]; }
            set { base[index] = value; }
        }
    }
}