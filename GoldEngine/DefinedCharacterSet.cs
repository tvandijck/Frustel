using System.Runtime.CompilerServices;

namespace GoldEngine
{
    internal class DefinedCharacterSet : CharacterSetBuild
    {
        // Fields
        internal NumberSet Dependacy;
        internal ISetExpression Exp;
        private string m_Comment;
        private string m_Definition;
        private string m_Name;
        private string m_Type;

        // Methods
        public DefinedCharacterSet()
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
        }

        public DefinedCharacterSet(string Name)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, CharacterSetBuild CharSet) : base(CharSet)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, string CharSet) : base(CharSet)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, params object[] Values)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            int upperBound = Values.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                if (Values[i] is CharacterSetRange)
                {
                    base.AddRange(Conversions.ToInteger(((CharacterSetRange)Values[i]).First), Conversions.ToInteger(((CharacterSetRange)Values[i]).Last));
                }
                else
                {
                    object[] objArray2 = new object[1];
                    object[] objArray = Values;
                    int index = i;
                    objArray2[0] = RuntimeHelpers.GetObjectValue(objArray[index]);
                    object[] arguments = objArray2;
                    bool[] copyBack = new bool[] { true };
                    Add(arguments);
                    if (copyBack[0])
                    {
                        objArray[index] = RuntimeHelpers.GetObjectValue(arguments[0]);
                    }
                }
            }
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, int StartValue, int EndValue)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            base.AddRange(StartValue, EndValue);
            this.m_Name = Name.Trim();
        }

        // Properties
        public string Comment
        {
            get
            {
                return this.m_Comment;
            }
            set
            {
                this.m_Comment = value;
            }
        }

        public string Definition
        {
            get
            {
                return this.m_Definition;
            }
            set
            {
                this.m_Definition = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public string Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
    }
}