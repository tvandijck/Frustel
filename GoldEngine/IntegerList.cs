using System.Collections;

namespace GoldEngine
{
    internal class IntegerList : ArrayList
    {
        // Methods
        public int Add(int Value)
        {
            if (base.IndexOf(Value) == -1)
            {
                base.Add(Value);
                base.Sort();
            }
            return base.Count;
        }

        public bool Contains(int Item)
        {
            return base.Contains(Item);
        }

        public string Text(string Separator = ", ")
        {
            string str = "";
            if (base.Count >= 1)
            {
                str = Conversions.ToString(base[0]);
                int num2 = base.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    str = str + Separator + Conversions.ToString(base[i]);
                }
            }
            return str;
        }

        public override string ToString()
        {
            string separator = ", ";
            return this.Text(separator);
        }

        // Properties
        public new int this[int Index]
        {
            get { return Conversions.ToInteger(base[Index]); }
            set { base[Index] = value; }
        }
    }
}