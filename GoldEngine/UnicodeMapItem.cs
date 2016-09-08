using System;

namespace GoldEngine
{
    internal class UnicodeMapItem : DictionarySet.IMember
    {
        // Fields
        public int Code;
        public int Map;

        // Methods
        public UnicodeMapItem()
        {
            this.Code = -1;
            this.Map = -1;
        }

        public UnicodeMapItem(int Code)
        {
            this.Code = Code;
            this.Map = -1;
        }

        public UnicodeMapItem(int Code, int Map)
        {
            this.Code = Code;
            this.Map = Map;
        }

        public DictionarySet.MemberResult Difference(DictionarySet.IMember NewObject)
        {
            return null;
        }

        public DictionarySet.MemberResult Intersect(DictionarySet.IMember NewObject)
        {
            return new DictionarySet.MemberResult(this);
        }

        public IComparable Key()
        {
            return (IComparable)this.Code;
        }

        public DictionarySet.MemberResult Union(DictionarySet.IMember NewObject)
        {
            return new DictionarySet.MemberResult(this);
        }
    }
}