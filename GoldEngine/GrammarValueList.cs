using System.Collections;

namespace GoldEngine
{
    internal class GrammarValueList : ArrayList
    {
        // Methods
        public int Add(GrammarValueList List)
        {
            int num3 = 0;
            int num4 = List.Count - 1;
            for (int i = 0; i <= num4; i++)
            {
                num3 = base.Add(List[i]);
            }
            return num3;
        }

        public int Add(string Text)
        {
            return base.Add(Text);
        }

        public override string ToString()
        {
            if (base.Count == 0)
            {
                return "";
            }
            string left = Conversions.ToString(base[0]);
            int num2 = base.Count - 1;
            for (int i = 1; i <= num2; i++)
            {
                left = Conversions.ToString(Operators.ConcatenateObject(left, Operators.ConcatenateObject(" ", base[i])));
            }
            return left;
        }

        // Properties
        public new string this[int Index]
        {
            get
            {
                return Conversions.ToString(base[Index]);
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}