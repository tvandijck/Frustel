using System.Collections;

namespace GoldEngine
{
    internal class GrammarAttribAssignList : ArrayList
    {
        // Methods
        public int Add(GrammarAttribAssign Item)
        {
            return base.Add(Item);
        }

        public int ItemIndex(GrammarAttribAssign Item)
        {
            int num = -1;
            for (int i = 0; (i < base.Count) & (num == -1); i++)
            {
                GrammarAttribAssign assign = (GrammarAttribAssign)base[i];
                if (assign.Name == Item.Name)
                {
                    num = i;
                }
            }
            return num;
        }

        // Properties
        public new GrammarAttribAssign this[int Index]
        {
            get
            {
                return (GrammarAttribAssign)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}