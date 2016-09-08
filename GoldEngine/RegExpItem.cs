using System.Linq;
using System.Runtime.CompilerServices;

namespace GoldEngine
{
    internal class RegExpItem
    {
        // Fields
        private object m_Data;
        private string m_Kleene;

        // Methods
        public RegExpItem()
        {
            this.m_Data = null;
            this.m_Kleene = "";
        }

        public RegExpItem(object Data, string Kleene)
        {
            this.m_Data = RuntimeHelpers.GetObjectValue(Data);
            this.m_Kleene = Kleene;
        }

        public bool IsVariableLength()
        {
            switch (this.m_Kleene)
            {
            case "*":
            case "+":
                return true;
            }
            if (this.m_Data is RegExp)
            {
                RegExp data = (RegExp)this.m_Data;
                return data.IsVariableLength();
            }
            return false;
        }

        public string LiteralFormat(string Source)
        {
            if (Source == "'")
            {
                return "''";
            }
            bool flag = false;
            for (short i = 0; (i < Source.Count<char>()) & !flag; i = (short)(i + 1))
            {
                flag = !char.IsLetter(Source[i]);
            }
            if (flag)
            {
                return ("'" + Source + "'");
            }
            return Source;
        }

        public override string ToString()
        {
            if (this.Data is RegExp)
            {
                return ("(" + this.Data.ToString() + ")" + this.Kleene);
            }
            if (this.Data is SetItem)
            {
                SetItem data = (SetItem)this.Data;
                switch (data.Type)
                {
                case SetItem.SetType.Chars:
                {
                    string rangeChars = "..";
                    string separator = ", ";
                    return ("{" + data.Characters.RangeText(rangeChars, separator, "&", true) + "}" + this.Kleene);
                }
                case SetItem.SetType.Name:
                    return ("{" + data.Text + "}" + this.Kleene);

                case SetItem.SetType.Sequence:
                    return (this.LiteralFormat(data.Text) + this.Kleene);
                }
            }
            return "";
        }

        // Properties
        public object Data
        {
            get
            {
                return this.m_Data;
            }
            set
            {
                this.m_Data = RuntimeHelpers.GetObjectValue(value);
            }
        }

        public string Kleene
        {
            get
            {
                return this.m_Kleene;
            }
            set
            {
                this.m_Kleene = value;
            }
        }
    }
}