using System.Collections;

namespace GoldEngine
{
    internal class DefinedSetNameList : ArrayList
    {
        public int Add(string item)
        {
            return base.Add(item);
        }

        public bool Contains(string search)
        {
            bool flag2 = false;
            for (int i = 0; (i < base.Count) & !flag2; i++)
            {
                if (this[i].ToUpper() == search.ToUpper())
                {
                    flag2 = true;
                }
            }
            return flag2;
        }

        public new string this[int index]
        {
            get { return Conversions.ToString(base[index]); }
            set { base[index] = value; }
        }
    }
}