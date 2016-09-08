using System.Collections;
using System.Linq;

namespace GoldEngine
{
    internal class RegExp
    {
        // Fields
        private ArrayList m_Array = new ArrayList();

        // Methods
        public void Add(RegExpSeq Item)
        {
            this.m_Array.Add(Item);
        }

        public void AddTextExp(string Expression)
        {
            int num3;
            RegExpSeq seq = new RegExpSeq();
            string text = "";
            string[] source = Strings.Split(Expression, "|", -1, CompareMethod.Binary);
            int num4 = source.Count<string>() - 1;
            for (num3 = 0; num3 <= num4; num3++)
            {
                source[num3] = source[num3].Trim();
            }
            int num5 = source.Count<string>() - 1;
            for (num3 = 0; num3 <= num5; num3++)
            {
                string str3 = source[num3];
                int startIndex = 0;
                seq = new RegExpSeq();
                while (startIndex < str3.Count<char>())
                {
                    char ch = str3[startIndex];
                    if (ch == '{')
                    {
                        int index = str3.IndexOf("}", startIndex);
                        text = str3.Substring(startIndex + 1, (index - startIndex) - 1);
                        startIndex = index + 1;
                    }
                    string kleene = "";
                    if (startIndex < str3.Count<char>())
                    {
                        switch (str3.Substring(startIndex, 1))
                        {
                            case "+":
                            case "?":
                            case "*":
                                kleene = str3.Substring(startIndex, 1);
                                startIndex++;
                                break;
                        }
                    }
                    RegExpItem item = new RegExpItem(new SetItem(SetItem.SetType.Name, text), kleene);
                    seq.Add(item);
                }
                this.m_Array.Add(seq);
            }
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        public bool IsVariableLength()
        {
            bool flag2 = false;
            for (short i = 0; (i < this.m_Array.Count) & !flag2; i = (short)(i + 1))
            {
                RegExpSeq seq = (RegExpSeq)this.m_Array[i];
                if (seq.IsVariableLength())
                {
                    flag2 = true;
                }
            }
            return flag2;
        }

        public override string ToString()
        {
            string str = "";
            if (this.m_Array.Count >= 1)
            {
                str = this.m_Array[0].ToString();
                int num2 = this.m_Array.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    str = str + " | " + this.m_Array[i].ToString();
                }
            }
            return str;
        }

        // Properties
        public RegExpSeq this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Array.Count))
                {
                    return (RegExpSeq)this.m_Array[Index];
                }
                return null;
            }
            set
            {
                this.m_Array[Index] = value;
            }
        }
    }
}