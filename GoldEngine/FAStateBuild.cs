namespace GoldEngine
{
    internal class FAStateBuild : FAState
    {
        // Fields
        public FAAcceptList AcceptList;
        public NumberSet NFAClosure;
        public NumberSet NFAStates;
        public NumberSet PriorStates;
        public short TableIndex;

        // Methods
        public FAStateBuild()
        {
            this.NFAStates = new NumberSet(new int[0]);
            this.AcceptList = new FAAcceptList();
            this.PriorStates = new NumberSet(new int[0]);
            base.EdgeList = new FAEdgeBuildList();
        }

        public FAStateBuild(SymbolBuild Accept) : base(Accept)
        {
            this.NFAStates = new NumberSet(new int[0]);
            this.AcceptList = new FAAcceptList();
            this.PriorStates = new NumberSet(new int[0]);
            base.EdgeList = new FAEdgeBuildList();
        }

        public void AddEdge(FAEdgeBuild Edge)
        {
            base.AddEdge(Edge);
        }

        public void AddEdge(CharacterSetBuild CharSet, int Target)
        {
            base.AddEdge(new FAEdgeBuild(CharSet, Target));
        }

        public void AddLambdaEdge(int Target)
        {
            base.AddEdge(new FAEdgeBuild(new CharacterSetBuild(), Target));
        }

        public void CaseClosure()
        {
            int num5 = base.Edges().Count() - 1;
            for (int i = 0; i <= num5; i++)
            {
                NumberSet setB = new NumberSet(new int[0]);
                CharacterSet characters = base.Edges()[i].Characters;
                int num6 = characters.Count() - 1;
                for (int j = 0; j <= num6; j++)
                {
                    int charCode = characters[j];
                    int num3 = UnicodeTable.ToLowerCase(charCode);
                    if (charCode != num3)
                    {
                        setB.Add(new int[] { num3 });
                    }
                    num3 = UnicodeTable.ToUpperCase(charCode);
                    if (charCode != num3)
                    {
                        setB.Add(new int[] { num3 });
                    }
                }
                characters.UnionWith(setB);
                characters = null;
            }
        }

        public new FAEdgeBuildList Edges()
        {
            return (FAEdgeBuildList)base.Edges();
        }

        public void MappingClosure(BuilderApp.CharMappingMode Mapping)
        {
            int num5 = base.Edges().Count() - 1;
            for (int i = 0; i <= num5; i++)
            {
                NumberSet setB = new NumberSet(new int[0]);
                CharacterSet characters = base.Edges()[i].Characters;
                int num6 = characters.Count() - 1;
                for (int j = 0; j <= num6; j++)
                {
                    int charCode = characters[j];
                    int num3 = UnicodeTable.ToWin1252(charCode);
                    if (charCode != num3)
                    {
                        setB.Add(new int[] { num3 });
                    }
                }
                characters.UnionWith(setB);
                characters = null;
            }
        }
    }
}