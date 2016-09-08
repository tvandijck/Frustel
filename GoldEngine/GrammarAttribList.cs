using System.Collections;

namespace GoldEngine
{
    internal class GrammarAttribList : ArrayList
    {
        // Methods
        public int Add(GrammarAttrib Text)
        {
            return base.Add(Text);
        }

        // Properties
        public new GrammarAttrib this[int Index]
        {
            get
            {
                return (GrammarAttrib)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}