namespace GoldEngine
{
    internal class SetItem : ISetExpression
    {
        // Fields
        public CharacterSetBuild m_Characters;
        public string m_Text;
        public SetType m_Type;

        // Methods
        public SetItem()
        {
            this.m_Type = SetType.Name;
        }

        public SetItem(CharacterSetBuild CharSet)
        {
            this.m_Type = SetType.Chars;
            this.m_Characters = CharSet;
        }

        public SetItem(SetType Type, string Text)
        {
            this.m_Type = Type;
            this.m_Text = Text;
        }

        public CharacterSetBuild Evaluate()
        {
            switch (this.m_Type)
            {
            case SetType.Chars:
                return this.m_Characters;

            case SetType.Name:
            {
                CharacterSetBuild characterSet = (CharacterSetBuild)BuilderApp.GetCharacterSet(this.m_Text);
                if (characterSet != null)
                {
                    return new CharacterSetBuild(characterSet);
                }
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Character set is not defined", "The character set {" + this.m_Text + "} was not defined in the grammar.", "");
                return new CharacterSetBuild();
            }
            }
            return new CharacterSetBuild();
        }

        public NumberSet UsedDefinedSets()
        {
            NumberSet set = new NumberSet(new int[0]);
            if (this.m_Type == SetType.Name)
            {
                int num = BuilderApp.UserDefinedSets.ItemIndex(this.m_Text);
                if (num != -1)
                {
                    set.Add(new int[] { num });
                }
            }
            return set;
        }

        // Properties
        public CharacterSet Characters
        {
            get
            {
                return this.m_Characters;
            }
            set
            {
                this.m_Characters = (CharacterSetBuild)value;
            }
        }

        public string Text
        {
            get
            {
                return this.m_Text;
            }
            set
            {
                this.m_Text = value;
            }
        }

        public SetType Type
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

        // Nested Types
        public enum SetType
        {
            Chars,
            Name,
            Sequence
        }
    }
}