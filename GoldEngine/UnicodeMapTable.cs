using System;

namespace GoldEngine
{
    internal class UnicodeMapTable : DictionarySet
    {
        // Methods
        public void Add(UnicodeMapItem Item)
        {
            base.Add(new DictionarySet.IMember[] { Item });
        }

        public void Add(int Code, int Map)
        {
            base.Add(new DictionarySet.IMember[] { new UnicodeMapItem(Code, Map) });
        }

        public bool Contains(int Code)
        {
            return base.Contains(Code);
        }

        public int IndexOf(int Code)
        {
            return base.IndexOf((IComparable)Code);
        }

        // Properties
        public new UnicodeMapItem this[int Index]
        {
            get
            {
                return (UnicodeMapItem)base[Index];
            }
        }
    }
}