using System;

namespace GoldEngine
{
    internal class ParseTablesBuild : ParseTables
    {
        // Methods
        public ParseTablesBuild()
        {
            base.m_Properties = new VariableList();
            base.m_Symbol = new SymbolBuildList();
            base.m_CharSet = new CharacterSetBuildList();
            base.m_Group = new GroupBuildList();
            base.m_DFA = new FAStateBuildList();
            base.m_Production = new ProductionBuildList();
            base.m_LALR = new LRStateBuildList();
        }

        public void ComputeCGTMetadata()
        {
            int num;
            int num2 = base.m_Symbol.Count() - 1;
            for (num = 0; num <= num2; num++)
            {
                this.Symbol[num].UsesDFA = false;
            }
            int num3 = this.DFA.Count - 1;
            for (num = 0; num <= num3; num++)
            {
                if (this.DFA[num].Accept != null)
                {
                    this.Symbol[this.DFA[num].Accept.TableIndex].UsesDFA = true;
                }
            }
        }

        public new bool Load(string Path)
        {
            bool flag2;
            SimpleDB.Reader cGT = new SimpleDB.Reader();
            this.Clear();
            cGT.Open(Path);
            string str = cGT.Header();
            if (str == "GOLD Parser Tables/v1.0")
            {
                flag2 = this.LoadVer1(cGT);
            }
            else if (str == "GOLD Parser Tables/v5.0")
            {
                flag2 = this.LoadVer5(cGT);
            }
            else
            {
                flag2 = false;
            }
            this.ComputeCGTMetadata();
            cGT.Close();
            return flag2;
        }

        private bool LoadVer1(SimpleDB.Reader CGT)
        {
            int num3;
            bool flag2 = true;
            while (!(CGT.EndOfFile() | !flag2))
            {
                int num7;
                int num8;
                int num10;
                int num14;
                int num15;
                CGT.GetNextRecord();
                switch (CGT.RetrieveByte())
                {
                case 0x43:
                {
                    int num5 = CGT.RetrieveInt16();
                    CharacterSetBuild build5 = new CharacterSetBuild(CGT.RetrieveString())
                    {
                        TableIndex = num5
                    };
                    base.m_CharSet[num5] = build5;
                    continue;
                }
                case 0x44:
                {
                    FAState state = new FAState();
                    num10 = CGT.RetrieveInt16();
                    bool flag3 = CGT.RetrieveBoolean();
                    int num9 = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    if (!flag3)
                    {
                        goto Label_02DC;
                    }
                    base.m_DFA[num10] = new FAStateBuild((SymbolBuild)base.m_Symbol[num9]);
                    goto Label_0337;
                }
                case 0x49:
                {
                    base.m_DFA.InitialState = (short)CGT.RetrieveInt16();
                    base.m_LALR.InitialState = (short)CGT.RetrieveInt16();
                    continue;
                }
                case 0x4c:
                    num14 = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    base.m_LALR[num14] = new LRStateBuild();
                    goto Label_03BA;

                case 80:
                {
                    base.m_Properties["Name"].Value = CGT.RetrieveString();
                    base.m_Properties["Version"].Value = CGT.RetrieveString();
                    base.m_Properties["Author"].Value = CGT.RetrieveString();
                    base.m_Properties["About"].Value = CGT.RetrieveString();
                    base.m_Properties["Case Sensitive"].Value = Conversions.ToString(CGT.RetrieveBoolean());
                    num3 = CGT.RetrieveInt16();
                    continue;
                }
                case 0x52:
                {
                    num7 = CGT.RetrieveInt16();
                    int num6 = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    base.m_Production[num7] = new ProductionBuild((SymbolBuild)base.m_Symbol[num6], (short)num7);
                    goto Label_0272;
                }
                case 0x53:
                {
                    int tableIndex = CGT.RetrieveInt16();
                    string name = CGT.RetrieveString();
                    SymbolType type = (SymbolType)CGT.RetrieveInt16();
                    base.m_Symbol[tableIndex] = new SymbolBuild(name, type, tableIndex);
                    continue;
                }
                case 0x54:
                {
                    base.m_Symbol = new SymbolBuildList(CGT.RetrieveInt16());
                    base.m_CharSet = new CharacterSetBuildList(CGT.RetrieveInt16());
                    base.m_Production = new ProductionBuildList(CGT.RetrieveInt16());
                    base.m_DFA = new FAStateBuildList(CGT.RetrieveInt16());
                    base.m_LALR = new LRStateBuildList(CGT.RetrieveInt16());
                    continue;
                }
                default:
                    goto Label_03CB;
                }
                Label_0244:
                num8 = CGT.RetrieveInt16();
                base.m_Production[num7].Handle().Add(base.m_Symbol[num8]);
                Label_0272:
                if (!CGT.RecordComplete())
                {
                    goto Label_0244;
                }
                continue;
                Label_02DC:
                base.m_DFA[num10] = new FAStateBuild();
                Label_0337:
                while (!CGT.RecordComplete())
                {
                    int num11 = CGT.RetrieveInt16();
                    int target = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    base.m_DFA[num10].AddEdge(new FAEdgeBuild((CharacterSetBuild)base.m_CharSet[num11], target));
                }
                continue;
                Label_0370:
                num15 = CGT.RetrieveInt16();
                int num13 = CGT.RetrieveInt16();
                int num16 = CGT.RetrieveInt16();
                CGT.RetrieveEntry();
                base.m_LALR[num14].Add(new LRAction(base.m_Symbol[num15], (LRActionType)num13, (short)num16));
                Label_03BA:
                if (!CGT.RecordComplete())
                {
                    goto Label_0370;
                }
                continue;
                Label_03CB:
                flag2 = false;
            }
            this.StartSymbol = base.m_Symbol[num3];
            SymbolBuild build3 = null;
            SymbolBuild build2 = null;
            SymbolBuild build = null;
            int num19 = base.m_Symbol.Count() - 1;
            for (int i = 0; i <= num19; i++)
            {
                SymbolBuild build4 = (SymbolBuild)base.m_Symbol[i];
                switch (((int)build4.Type))
                {
                case 2:
                    if (build == null)
                    {
                        build = build4;
                    }
                    break;

                case 4:
                    build3 = build4;
                    break;

                case 5:
                    build2 = build4;
                    break;
                }
            }
            if (build3 != null)
            {
                GroupBuild item = new GroupBuild();
                int num17 = base.m_Group.Add(item);
                item.TableIndex = 0;
                item.Name = "Comment Block";
                item.Container = build;
                item.Nesting.Add(item.TableIndex);
                item.Advance = AdvanceMode.Token;
                item.Ending = EndingMode.Closed;
                item.Start = build3;
                item.End = build2;
                item.Start.Group = item;
                item.End.Group = item;
                item.NestingNames = "All";
            }
            return flag2;
        }

        private bool LoadVer5(SimpleDB.Reader EGT)
        {
            bool flag2;
            try
            {
                flag2 = true;
                while (!(EGT.EndOfFile() | !flag2))
                {
                    GroupBuild build;
                    int num3;
                    int num4;
                    CharacterSetBuild build2;
                    int num5;
                    int num7;
                    int num8;
                    int num10;
                    int num14;
                    int num15;
                    GroupBuild build3;
                    int num17;
                    EGT.GetNextRecord();
                    ParseTables.EGTRecord record = (ParseTables.EGTRecord)EGT.RetrieveByte();
                    switch (((byte)(((int)record) - 0x44)))
                    {
                    case 0:
                    {
                        num10 = EGT.RetrieveInt16();
                        bool flag3 = EGT.RetrieveBoolean();
                        int num9 = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        if (!flag3)
                        {
                            goto Label_0423;
                        }
                        base.m_DFA[num10] = new FAStateBuild((SymbolBuild)base.m_Symbol[num9]);
                        goto Label_0483;
                    }
                    case 5:
                    {
                        base.m_DFA.InitialState = (short)EGT.RetrieveInt16();
                        base.m_LALR.InitialState = (short)EGT.RetrieveInt16();
                        continue;
                    }
                    case 8:
                        num14 = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        base.m_LALR[num14] = new LRStateBuild();
                        goto Label_0506;

                    case 14:
                    {
                        num7 = EGT.RetrieveInt16();
                        int num6 = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        base.m_Production[num7] = new ProductionBuild((SymbolBuild)base.m_Symbol[num6], (short)num7);
                        goto Label_03C0;
                    }
                    case 15:
                    {
                        int tableIndex = EGT.RetrieveInt16();
                        string name = EGT.RetrieveString();
                        SymbolType type = (SymbolType)EGT.RetrieveInt16();
                        base.m_Symbol[tableIndex] = new SymbolBuild(name, type, tableIndex);
                        continue;
                    }
                    case 0x1f:
                        build2 = new CharacterSetBuild();
                        num5 = EGT.RetrieveInt16();
                        EGT.RetrieveInt16();
                        EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        goto Label_0323;

                    case 0x23:
                        build = new GroupBuild();
                        build3 = build;
                        num3 = EGT.RetrieveInt16();
                        build3.Name = EGT.RetrieveString();
                        build3.Container = base.m_Symbol[EGT.RetrieveInt16()];
                        build3.Start = base.m_Symbol[EGT.RetrieveInt16()];
                        build3.End = base.m_Symbol[EGT.RetrieveInt16()];
                        build3.Advance = (AdvanceMode)EGT.RetrieveInt16();
                        build3.Ending = (EndingMode)EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        num17 = EGT.RetrieveInt16();
                        num4 = 1;
                        goto Label_029B;

                    case 0x2c:
                    {
                        EGT.RetrieveInt16();
                        string str = EGT.RetrieveString();
                        string str2 = EGT.RetrieveString();
                        base.m_Properties.Add(str, str2);
                        continue;
                    }
                    case 0x30:
                    {
                        base.m_Symbol = new SymbolBuildList(EGT.RetrieveInt16());
                        base.m_CharSet = new CharacterSetBuildList(EGT.RetrieveInt16());
                        base.m_Production = new ProductionBuildList(EGT.RetrieveInt16());
                        base.m_DFA = new FAStateBuildList(EGT.RetrieveInt16());
                        base.m_LALR = new LRStateBuildList(EGT.RetrieveInt16());
                        base.m_Group = new GroupBuildList(EGT.RetrieveInt16());
                        continue;
                    }
                    default:
                        goto Label_0517;
                    }
                    Label_0281:
                    build3.Nesting.Add(EGT.RetrieveInt16());
                    num4++;
                    Label_029B:
                    if (num4 <= num17)
                    {
                        goto Label_0281;
                    }
                    build3 = null;
                    build.Container.Group = build;
                    build.Start.Group = build;
                    build.End.Group = build;
                    base.m_Group[num3] = build;
                    continue;
                    Label_030E:
                    build2.AddRange(EGT.RetrieveInt16(), EGT.RetrieveInt16());
                    Label_0323:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_030E;
                    }
                    build2.TableIndex = num5;
                    base.m_CharSet[num5] = build2;
                    continue;
                    Label_0392:
                    num8 = EGT.RetrieveInt16();
                    base.m_Production[num7].Handle().Add(base.m_Symbol[num8]);
                    Label_03C0:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_0392;
                    }
                    continue;
                    Label_0423:
                    base.m_DFA[num10] = new FAStateBuild();
                    Label_0483:
                    while (!EGT.RecordComplete())
                    {
                        int num11 = EGT.RetrieveInt16();
                        int target = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        base.m_DFA[num10].Edges().Add(new FAEdgeBuild((CharacterSetBuild)base.m_CharSet[num11], target));
                    }
                    continue;
                    Label_04BC:
                    num15 = EGT.RetrieveInt16();
                    int num13 = EGT.RetrieveInt16();
                    int num16 = EGT.RetrieveInt16();
                    EGT.RetrieveEntry();
                    base.m_LALR[num14].Add(new LRAction(base.m_Symbol[num15], (LRActionType)num13, (short)num16));
                    Label_0506:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_04BC;
                    }
                    continue;
                    Label_0517:
                    flag2 = false;
                    throw new ParserException("File Error. A record of type '" + Conversions.ToString(Strings.ChrW((int)record)) + "' was read. This is not a valid code.");
                }
                EGT.Close();
            }
            catch (Exception exception1)
            {
                Exception inner = exception1;
                throw new ParserException(inner.Message, inner, "LoadTables");
            }
            return flag2;
        }

        // Properties
        public new CharacterSetBuildList CharSet
        {
            get { return (CharacterSetBuildList) base.m_CharSet; }
            set { base.m_CharSet = value; }
        }

        public new FAStateBuildList DFA
        {
            get { return (FAStateBuildList) base.m_DFA; }
            set { base.m_DFA = value; }
        }

        public new GroupBuildList Group
        {
            get { return (GroupBuildList) base.m_Group; }
            set { base.m_Group = value; }
        }

        public new LRStateBuildList LALR
        {
            get { return (LRStateBuildList) base.m_LALR; }
            set { base.m_LALR = value; }
        }

        public new ProductionBuildList Production
        {
            get { return (ProductionBuildList) base.m_Production; }
            set { base.m_Production = value; }
        }

        public new VariableList Properties
        {
            get { return base.m_Properties; }
            set { base.m_Properties = value; }
        }

        public new SymbolBuildList Symbol
        {
            get { return (SymbolBuildList) base.m_Symbol; }
            set { base.m_Symbol = value; }
        }
    }
}