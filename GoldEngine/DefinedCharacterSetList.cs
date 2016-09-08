using System.Collections;

namespace GoldEngine
{
    internal class DefinedCharacterSetList : ArrayList
    {
        // Methods
        public DefinedCharacterSetList()
        {
        }

        public DefinedCharacterSetList(int Size) : base(Size)
        {
        }

        public int Add(DefinedCharacterSet Item)
        {
            return base.Add(Item);
        }

        public int Add(string Name, params object[] Values)
        {
            DefinedCharacterSet item = new DefinedCharacterSet(Name, Values);
            return this.Add(item);
        }

        public bool Contains(DefinedCharacterSet Item)
        {
            return (this.ItemIndex(Item.Name) != -1);
        }

        public int ItemIndex(string Name)
        {
            short num = -1;
            short num3 = 0;
            while ((num3 < base.Count) & (num == -1))
            {
                if (Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(base[num3], null, "Name", new object[0], null, null, null), null, "ToUpper", new object[0], null, null, null), Name.ToUpper(), false))
                {
                    num = num3;
                }
                else
                {
                    num3 = (short)(num3 + 1);
                }
            }
            return num;
        }

        // Properties
        public new DefinedCharacterSet this[int Index]
        {
            get { return (DefinedCharacterSet) base[Index]; }
            set { base[Index] = value; }
        }

        public DefinedCharacterSet this[string Name]
        {
            get
            {
                int num = this.ItemIndex(Name);
                return (DefinedCharacterSet)base[num];
            }
            set
            {
                int num = this.ItemIndex(Name);
                base[num] = value;
            }
        }
    }
}