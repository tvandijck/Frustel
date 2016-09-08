using System.Collections;
using System.Linq;

namespace GoldEngine
{
    internal class VariableList : ArrayList
    {
        // Fields
        public string IgnorableMatchChars;

        // Methods
        public VariableList()
        {
            this.IgnorableMatchChars = "";
        }

        public VariableList(string IgnorableMatchChars)
        {
            this.IgnorableMatchChars = IgnorableMatchChars;
        }

        public int Add(Variable NewVar)
        {
            short num2 = (short)this.ItemIndex(NewVar.Name);
            if (num2 == -1)
            {
                return base.Add(NewVar);
            }
            return num2;
        }

        public int Add(string Name, object Value = null)
        {
            Variable newVar = new Variable
            {
                Name = Name,
                Value = Conversions.ToString(Value)
            };
            return this.Add(newVar);
        }

        public void AddList(VariableList List)
        {
            int num2 = List.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Add(List[i]);
            }
        }

        public void ClearValues()
        {
            int num2 = base.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                Variable variable = (Variable)base[i];
                variable.Value = "";
            }
        }

        public bool Contains(string Name)
        {
            return (this.ItemIndex(Name) != -1);
        }

        private bool IsNameMatch(string MainName, string Inquiry)
        {
            return ((Operators.CompareString(MainName.ToUpper(), Inquiry.ToUpper(), true) == 0) | (Operators.CompareString(this.RemoveIgnorableChars(MainName).ToUpper(), Inquiry.ToUpper(), true) == 0));
        }

        public int ItemIndex(string Name)
        {
            short num = -1;
            short num3 = 0;
            while ((num3 < base.Count) & (num == -1))
            {
                Variable variable = (Variable)base[num3];
                if (this.IsNameMatch(variable.Name, Name))
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

        private string RemoveIgnorableChars(string Text)
        {
            string str2 = "";
            int num2 = Text.Count<char>() - 1;
            for (int i = 0; i <= num2; i++)
            {
                char ch = Text[i];
                if (!this.IgnorableMatchChars.Contains(Conversions.ToString(ch)))
                {
                    str2 = str2 + Conversions.ToString(Text[i]);
                }
            }
            return str2;
        }

        // Properties
        public Variable this[string Name]
        {
            get
            {
                int num = this.ItemIndex(Name);
                if (num == -1)
                {
                    num = base.Add(new Variable(Name));
                }
                return (Variable)base[num];
            }
            set
            {
                int num = this.ItemIndex(Name);
                if ((num >= 0) & (num < base.Count))
                {
                    base[num] = value;
                }
            }
        }

        public new Variable this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < base.Count))
                {
                    return (Variable)base[Index];
                }
                return null;
            }
            set
            {
                if ((Index >= 0) & (Index < base.Count))
                {
                    base[Index] = value;
                }
            }
        }
    }
}