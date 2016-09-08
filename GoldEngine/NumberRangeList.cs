using System.Collections;

namespace GoldEngine
{
    internal class NumberRangeList : ArrayList
    {
        public int Add(NumberRange Range)
        {
            return base.Add(Range);
        }

        public new NumberRange this[int Index]
        {
            get { return (NumberRange)base[Index]; }
            set { base[Index] = value; }
        }
    }
}