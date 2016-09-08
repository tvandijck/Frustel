namespace GoldEngine
{
    internal class GrammarAttrib
    {
        // Fields
        public bool IsSet;
        public GrammarValueList List = new GrammarValueList();
        public string Name;

        // Methods
        public override string ToString()
        {
            return this.Value(", ");
        }

        public string Value(string Separator = ", ")
        {
            string str;
            if (this.List.Count == 0)
            {
                str = "";
            }
            else
            {
                str = this.List[0];
                int num2 = this.List.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    str = str + Separator + this.List[i];
                }
            }
            if (this.IsSet)
            {
                return ("{" + str + "}");
            }
            return str;
        }
    }




}