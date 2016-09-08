using System;
using System.IO;

namespace GoldEngine
{
    internal class ParseTables
    {
        // Fields
        protected CharacterSetList m_CharSet = new CharacterSetList();
        protected FAStateList m_DFA = new FAStateList();
        protected GroupList m_Group = new GroupList();
        protected LRStateList m_LALR = new LRStateList();
        protected ProductionList m_Production = new ProductionList();
        protected VariableList m_Properties = new VariableList();
        protected Symbol m_StartSymbol;
        protected SymbolList m_Symbol = new SymbolList();

        // Methods
        public void Clear()
        {
            this.m_Properties.Clear();
            this.m_Symbol.Clear();
            this.m_CharSet.Clear();
            this.m_Group.Clear();
            this.m_DFA.Clear();
            this.m_Production.Clear();
            this.m_LALR.Clear();
        }

        public bool IsLoaded()
        {
            return ((this.m_DFA.Count >= 1) & (this.m_LALR.Count >= 1));
        }

        public bool Load(string Path)
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
            cGT.Close();
            return flag2;
        }

        private bool LoadVer1(SimpleDB.Reader CGT)
        {
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
                    CharacterSet set = new CharacterSet(CGT.RetrieveString())
                    {
                        TableIndex = num5
                    };
                    this.m_CharSet[num5] = set;
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
                        goto Label_02D3;
                    }
                    this.m_DFA[num10] = new FAState(this.m_Symbol[num9]);
                    goto Label_0329;
                }
                case 0x49:
                {
                    this.m_DFA.InitialState = (short)CGT.RetrieveInt16();
                    this.m_LALR.InitialState = (short)CGT.RetrieveInt16();
                    continue;
                }
                case 0x4c:
                    num14 = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    this.m_LALR[num14] = new LRState();
                    goto Label_03AC;

                case 80:
                {
                    this.m_Properties["Name"].Value = CGT.RetrieveString();
                    this.m_Properties["Version"].Value = CGT.RetrieveString();
                    this.m_Properties["Author"].Value = CGT.RetrieveString();
                    this.m_Properties["About"].Value = CGT.RetrieveString();
                    this.m_Properties["Case Sensitive"].Value = Conversions.ToString(CGT.RetrieveBoolean());
                    int num3 = CGT.RetrieveInt16();
                    continue;
                }
                case 0x52:
                {
                    num7 = CGT.RetrieveInt16();
                    int num6 = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    this.m_Production[num7] = new Production(this.m_Symbol[num6], (short)num7);
                    goto Label_026E;
                }
                case 0x53:
                {
                    int num4 = CGT.RetrieveInt16();
                    string name = CGT.RetrieveString();
                    SymbolType type = (SymbolType)CGT.RetrieveInt16();
                    this.m_Symbol[num4] = new Symbol(name, type, (short)num4);
                    continue;
                }
                case 0x54:
                {
                    this.m_Symbol = new SymbolList(CGT.RetrieveInt16());
                    this.m_CharSet = new CharacterSetList(CGT.RetrieveInt16());
                    this.m_Production = new ProductionList(CGT.RetrieveInt16());
                    this.m_DFA = new FAStateList(CGT.RetrieveInt16());
                    this.m_LALR = new LRStateList(CGT.RetrieveInt16());
                    continue;
                }
                default:
                    goto Label_03BD;
                }
                Label_0240:
                num8 = CGT.RetrieveInt16();
                this.m_Production[num7].Handle().Add(this.m_Symbol[num8]);
                Label_026E:
                if (!CGT.RecordComplete())
                {
                    goto Label_0240;
                }
                continue;
                Label_02D3:
                this.m_DFA[num10] = new FAState();
                Label_0329:
                while (!CGT.RecordComplete())
                {
                    int num11 = CGT.RetrieveInt16();
                    int target = CGT.RetrieveInt16();
                    CGT.RetrieveEntry();
                    this.m_DFA[num10].AddEdge(new FAEdge(this.m_CharSet[num11], target));
                }
                continue;
                Label_0362:
                num15 = CGT.RetrieveInt16();
                int num13 = CGT.RetrieveInt16();
                int num16 = CGT.RetrieveInt16();
                CGT.RetrieveEntry();
                this.m_LALR[num14].Add(new LRAction(this.m_Symbol[num15], (LRActionType)num13, (short)num16));
                Label_03AC:
                if (!CGT.RecordComplete())
                {
                    goto Label_0362;
                }
                continue;
                Label_03BD:
                flag2 = false;
            }
            Symbol symbol3 = null;
            Symbol symbol2 = null;
            Symbol symbol = null;
            int num19 = this.m_Symbol.Count() - 1;
            for (int i = 0; i <= num19; i++)
            {
                Symbol symbol4 = this.m_Symbol[i];
                switch (((int)symbol4.Type))
                {
                case 2:
                    if (symbol == null)
                    {
                        symbol = symbol4;
                    }
                    break;

                case 4:
                    symbol3 = symbol4;
                    break;

                case 5:
                    symbol2 = symbol4;
                    break;
                }
            }
            if (symbol3 != null)
            {
                Group item = new Group();
                int num17 = this.m_Group.Add(item);
                item.TableIndex = 0;
                item.Name = "Comment Block";
                item.Container = symbol;
                item.Nesting.Add(item.TableIndex);
                item.Advance = AdvanceMode.Token;
                item.Ending = EndingMode.Closed;
                item.Start = symbol3;
                item.End = symbol2;
                item.Start.Group = item;
                item.End.Group = item;
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
                    Group group;
                    int num3;
                    int num4;
                    CharacterSet set;
                    int num5;
                    int num7;
                    int num8;
                    int num10;
                    int num14;
                    int num15;
                    Group group2;
                    int num17;
                    EGT.GetNextRecord();
                    EGTRecord record = (EGTRecord)EGT.RetrieveByte();
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
                            goto Label_041A;
                        }
                        this.m_DFA[num10] = new FAState(this.m_Symbol[num9]);
                        goto Label_0475;
                    }
                    case 5:
                    {
                        this.m_DFA.InitialState = (short)EGT.RetrieveInt16();
                        this.m_LALR.InitialState = (short)EGT.RetrieveInt16();
                        continue;
                    }
                    case 8:
                        num14 = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        this.m_LALR[num14] = new LRState();
                        goto Label_04F8;

                    case 14:
                    {
                        num7 = EGT.RetrieveInt16();
                        int num6 = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        this.m_Production[num7] = new Production(this.m_Symbol[num6], (short)num7);
                        goto Label_03BC;
                    }
                    case 15:
                    {
                        int num = EGT.RetrieveInt16();
                        string name = EGT.RetrieveString();
                        SymbolType type = (SymbolType)EGT.RetrieveInt16();
                        this.m_Symbol[num] = new Symbol(name, type, (short)num);
                        continue;
                    }
                    case 0x1f:
                        set = new CharacterSet();
                        num5 = EGT.RetrieveInt16();
                        EGT.RetrieveInt16();
                        EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        goto Label_0324;

                    case 0x23:
                        group = new Group();
                        group2 = group;
                        num3 = EGT.RetrieveInt16();
                        group2.Name = EGT.RetrieveString();
                        group2.Container = this.m_Symbol[EGT.RetrieveInt16()];
                        group2.Start = this.m_Symbol[EGT.RetrieveInt16()];
                        group2.End = this.m_Symbol[EGT.RetrieveInt16()];
                        group2.Advance = (AdvanceMode)EGT.RetrieveInt16();
                        group2.Ending = (EndingMode)EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        num17 = EGT.RetrieveInt16();
                        num4 = 1;
                        goto Label_029C;

                    case 0x2c:
                    {
                        EGT.RetrieveInt16();
                        string str = EGT.RetrieveString();
                        string str2 = EGT.RetrieveString();
                        this.m_Properties.Add(str, str2);
                        continue;
                    }
                    case 0x30:
                    {
                        this.m_Symbol = new SymbolList(EGT.RetrieveInt16());
                        this.m_CharSet = new CharacterSetList(EGT.RetrieveInt16());
                        this.m_Production = new ProductionList(EGT.RetrieveInt16());
                        this.m_DFA = new FAStateList(EGT.RetrieveInt16());
                        this.m_LALR = new LRStateList(EGT.RetrieveInt16());
                        this.m_Group = new GroupList(EGT.RetrieveInt16());
                        continue;
                    }
                    default:
                        goto Label_0509;
                    }
                    Label_0282:
                    group2.Nesting.Add(EGT.RetrieveInt16());
                    num4++;
                    Label_029C:
                    if (num4 <= num17)
                    {
                        goto Label_0282;
                    }
                    group2 = null;
                    group.Container.Group = group;
                    group.Start.Group = group;
                    group.End.Group = group;
                    this.m_Group[num3] = group;
                    continue;
                    Label_030F:
                    set.AddRange(EGT.RetrieveInt16(), EGT.RetrieveInt16());
                    Label_0324:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_030F;
                    }
                    set.TableIndex = num5;
                    this.m_CharSet[num5] = set;
                    continue;
                    Label_038E:
                    num8 = EGT.RetrieveInt16();
                    this.m_Production[num7].Handle().Add(this.m_Symbol[num8]);
                    Label_03BC:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_038E;
                    }
                    continue;
                    Label_041A:
                    this.m_DFA[num10] = new FAState();
                    Label_0475:
                    while (!EGT.RecordComplete())
                    {
                        int num11 = EGT.RetrieveInt16();
                        int target = EGT.RetrieveInt16();
                        EGT.RetrieveEntry();
                        this.m_DFA[num10].Edges().Add(new FAEdge(this.m_CharSet[num11], target));
                    }
                    continue;
                    Label_04AE:
                    num15 = EGT.RetrieveInt16();
                    int num13 = EGT.RetrieveInt16();
                    int num16 = EGT.RetrieveInt16();
                    EGT.RetrieveEntry();
                    this.m_LALR[num14].Add(new LRAction(this.m_Symbol[num15], (LRActionType)num13, (short)num16));
                    Label_04F8:
                    if (!EGT.RecordComplete())
                    {
                        goto Label_04AE;
                    }
                    continue;
                    Label_0509:
                    flag2 = false;
                    throw new Exception("File Error. A record of type '" + Conversions.ToString(Strings.ChrW((int)record)) + "' was read. This is not a valid code.");
                }
                EGT.Close();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                throw exception;
            }
            return flag2;
        }

        internal bool SaveVer1(string Path)
        {
            short num5;
            bool flag2;
            SimpleDB.Writer writer = new SimpleDB.Writer();
            CharacterSet setB = new CharacterSet(new int[] { 0 });
            int tableIndex = -1;
            int num3 = -1;
            int num = -1;
            short num6 = (short)(this.m_Group.Count - 1);
            for (num5 = 0; num5 <= num6; num5 = (short)(num5 + 1))
            {
                Group group = this.m_Group[num5];
                if (group.Name.ToUpper() == "COMMENT LINE")
                {
                    tableIndex = group.Start.TableIndex;
                }
                else if (group.Name.ToUpper() == "COMMENT BLOCK")
                {
                    num3 = group.Start.TableIndex;
                    num = group.End.TableIndex;
                }
                group = null;
            }
            try
            {
                short num4;
                writer.Open(Path, "GOLD Parser Tables/v1.0");
                writer.NewRecord();
                writer.StoreByte(80);
                writer.StoreString(this.Properties["Name"].Value);
                writer.StoreString(this.Properties["Version"].Value);
                writer.StoreString(this.Properties["Author"].Value);
                writer.StoreString(this.Properties["About"].Value);
                writer.StoreBoolean(Strings.UCase(this.Properties["Case Sensitive"].Value) == "TRUE");
                writer.StoreInt16(this.StartSymbol.TableIndex);
                writer.NewRecord();
                writer.StoreByte(0x54);
                writer.StoreInt16(this.m_Symbol.Count());
                writer.StoreInt16(this.m_CharSet.Count);
                writer.StoreInt16(this.m_Production.Count());
                writer.StoreInt16(this.m_DFA.Count);
                writer.StoreInt16(this.m_LALR.Count);
                writer.NewRecord();
                writer.StoreByte(0x49);
                writer.StoreInt16(this.m_DFA.InitialState);
                writer.StoreInt16(this.m_LALR.InitialState);
                short num7 = (short)(this.m_CharSet.Count - 1);
                for (num5 = 0; num5 <= num7; num5 = (short)(num5 + 1))
                {
                    CharacterSet set = new CharacterSet(this.m_CharSet[num5]);
                    set.DifferenceWith(setB);
                    writer.NewRecord();
                    writer.StoreByte(0x43);
                    writer.StoreInt16(num5);
                    writer.StoreString(set.ToString());
                }
                short num8 = (short)(this.m_Symbol.Count() - 1);
                for (num5 = 0; num5 <= num8; num5 = (short)(num5 + 1))
                {
                    SymbolType lEGACYCommentLine;
                    Symbol symbol = this.m_Symbol[num5];
                    switch (((int)symbol.Type))
                    {
                    case 4:
                        if (num5 != tableIndex)
                        {
                            break;
                        }
                        lEGACYCommentLine = SymbolType.LEGACYCommentLine;
                        goto Label_034D;

                    case 5:
                        if (num5 != num)
                        {
                            goto Label_033C;
                        }
                        lEGACYCommentLine = SymbolType.GroupEnd;
                        goto Label_034D;

                    default:
                        lEGACYCommentLine = symbol.Type;
                        goto Label_034D;
                    }
                    if (num5 == num3)
                    {
                        lEGACYCommentLine = SymbolType.GroupStart;
                    }
                    else
                    {
                        lEGACYCommentLine = SymbolType.Content;
                    }
                    goto Label_034D;
                    Label_033C:
                    lEGACYCommentLine = SymbolType.Content;
                    Label_034D:
                    writer.NewRecord();
                    writer.StoreByte(0x53);
                    writer.StoreInt16(num5);
                    Symbol symbol2 = symbol;
                    string name = symbol2.Name;
                    writer.StoreString(name);
                    symbol2.Name = name;
                    writer.StoreInt16((int)lEGACYCommentLine);
                    symbol = null;
                }
                short num9 = (short)(this.m_Production.Count() - 1);
                for (num5 = 0; num5 <= num9; num5 = (short)(num5 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x52);
                    writer.StoreInt16(num5);
                    writer.StoreInt16(this.m_Production[num5].Head.TableIndex);
                    writer.StoreEmpty();
                    short num10 = (short)(this.m_Production[num5].Handle().Count() - 1);
                    num4 = 0;
                    while (num4 <= num10)
                    {
                        writer.StoreInt16(this.m_Production[num5].Handle()[num4].TableIndex);
                        num4 = (short)(num4 + 1);
                    }
                }
                short num11 = (short)(this.DFA.Count - 1);
                for (num5 = 0; num5 <= num11; num5 = (short)(num5 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x44);
                    writer.StoreInt16(num5);
                    if (this.DFA[num5].Accept == null)
                    {
                        writer.StoreBoolean(false);
                        writer.StoreInt16(-1);
                    }
                    else
                    {
                        writer.StoreBoolean(true);
                        writer.StoreInt16(this.DFA[num5].Accept.TableIndex);
                    }
                    writer.StoreEmpty();
                    short num12 = (short)(this.DFA[num5].Edges().Count() - 1);
                    num4 = 0;
                    while (num4 <= num12)
                    {
                        writer.StoreInt16(this.DFA[num5].Edges()[num4].Characters.TableIndex);
                        writer.StoreInt16(this.DFA[num5].Edges()[num4].Target);
                        writer.StoreEmpty();
                        num4 = (short)(num4 + 1);
                    }
                }
                short num13 = (short)(this.LALR.Count - 1);
                for (num5 = 0; num5 <= num13; num5 = (short)(num5 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x4c);
                    writer.StoreInt16(num5);
                    writer.StoreEmpty();
                    short num14 = (short)(this.LALR[num5].Count - 1);
                    for (num4 = 0; num4 <= num14; num4 = (short)(num4 + 1))
                    {
                        writer.StoreInt16(this.LALR[num5][num4].SymbolIndex());
                        writer.StoreInt16((int)this.LALR[num5][num4].Type());
                        writer.StoreInt16(this.LALR[num5][num4].Value());
                        writer.StoreEmpty();
                    }
                }
                writer.Close();
                flag2 = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag2 = false;
            }
            return flag2;
        }

        internal bool SaveVer5(string Path)
        {
            bool flag2;
            SimpleDB.Writer writer = new SimpleDB.Writer();
            try
            {
                short num;
                short num2;
                writer.Open(Path, "GOLD Parser Tables/v5.0");
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(0);
                string name = "Name";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Name"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(1);
                name = "Version";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Version"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(2);
                name = "Author";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Author"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(3);
                name = "About";
                writer.StoreString(name);
                writer.StoreString(this.Properties["About"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(4);
                name = "Character Set";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Character Set"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(5);
                name = "Character Mapping";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Character Mapping"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(6);
                name = "Generated By";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Generated By"].Value);
                writer.NewRecord();
                writer.StoreByte(0x70);
                writer.StoreInt16(7);
                name = "Generated Date";
                writer.StoreString(name);
                writer.StoreString(this.Properties["Generated Date"].Value);
                writer.NewRecord();
                writer.StoreByte(0x74);
                writer.StoreInt16(this.m_Symbol.Count());
                writer.StoreInt16(this.m_CharSet.Count);
                writer.StoreInt16(this.m_Production.Count());
                writer.StoreInt16(this.m_DFA.Count);
                writer.StoreInt16(this.m_LALR.Count);
                writer.StoreInt16(this.m_Group.Count);
                writer.NewRecord();
                writer.StoreByte(0x49);
                writer.StoreInt16(this.m_DFA.InitialState);
                writer.StoreInt16(this.m_LALR.InitialState);
                short num3 = (short)(this.m_CharSet.Count - 1);
                for (num2 = 0; num2 <= num3; num2 = (short)(num2 + 1))
                {
                    NumberRangeList list = this.m_CharSet[num2].RangeList();
                    writer.NewRecord();
                    writer.StoreByte(0x63);
                    writer.StoreInt16(num2);
                    writer.StoreInt16(0);
                    writer.StoreInt16(list.Count);
                    writer.StoreEmpty();
                    short num4 = (short)(list.Count - 1);
                    num = 0;
                    while (num <= num4)
                    {
                        writer.StoreInt16(list[num].First);
                        writer.StoreInt16(list[num].Last);
                        num = (short)(num + 1);
                    }
                }
                short num5 = (short)(this.m_Symbol.Count() - 1);
                for (num2 = 0; num2 <= num5; num2 = (short)(num2 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x53);
                    writer.StoreInt16(num2);
                    Symbol symbol = this.m_Symbol[num2];
                    Symbol symbol2 = symbol;
                    name = symbol2.Name;
                    writer.StoreString(name);
                    symbol2.Name = name;
                    writer.StoreInt16((int)symbol.Type);
                    symbol = null;
                }
                short num6 = (short)(this.m_Group.Count - 1);
                for (num2 = 0; num2 <= num6; num2 = (short)(num2 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x67);
                    Group group = this.m_Group[num2];
                    writer.StoreInt16(num2);
                    writer.StoreString(group.Name);
                    writer.StoreInt16(group.Container.TableIndex);
                    writer.StoreInt16(group.Start.TableIndex);
                    writer.StoreInt16(group.End.TableIndex);
                    writer.StoreInt16((int)group.Advance);
                    writer.StoreInt16((int)group.Ending);
                    writer.StoreEmpty();
                    writer.StoreInt16(group.Nesting.Count);
                    short num7 = (short)(group.Nesting.Count - 1);
                    num = 0;
                    while (num <= num7)
                    {
                        writer.StoreInt16(group.Nesting[num]);
                        num = (short)(num + 1);
                    }
                    group = null;
                }
                short num8 = (short)(this.m_Production.Count() - 1);
                for (num2 = 0; num2 <= num8; num2 = (short)(num2 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x52);
                    writer.StoreInt16(num2);
                    writer.StoreInt16(this.m_Production[num2].Head.TableIndex);
                    writer.StoreEmpty();
                    short num9 = (short)(this.m_Production[num2].Handle().Count() - 1);
                    num = 0;
                    while (num <= num9)
                    {
                        writer.StoreInt16(this.m_Production[num2].Handle()[num].TableIndex);
                        num = (short)(num + 1);
                    }
                }
                short num10 = (short)(this.DFA.Count - 1);
                for (num2 = 0; num2 <= num10; num2 = (short)(num2 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x44);
                    writer.StoreInt16(num2);
                    if (this.DFA[num2].Accept != null)
                    {
                        writer.StoreBoolean(true);
                        writer.StoreInt16(this.DFA[num2].Accept.TableIndex);
                    }
                    else
                    {
                        writer.StoreBoolean(false);
                        writer.StoreInt16(0);
                    }
                    writer.StoreEmpty();
                    short num11 = (short)(this.DFA[num2].Edges().Count() - 1);
                    num = 0;
                    while (num <= num11)
                    {
                        writer.StoreInt16(this.DFA[num2].Edges()[num].Characters.TableIndex);
                        writer.StoreInt16(this.DFA[num2].Edges()[num].Target);
                        writer.StoreEmpty();
                        num = (short)(num + 1);
                    }
                }
                short num12 = (short)(this.LALR.Count - 1);
                for (num2 = 0; num2 <= num12; num2 = (short)(num2 + 1))
                {
                    writer.NewRecord();
                    writer.StoreByte(0x4c);
                    writer.StoreInt16(num2);
                    writer.StoreEmpty();
                    short num13 = (short)(this.LALR[num2].Count - 1);
                    for (num = 0; num <= num13; num = (short)(num + 1))
                    {
                        writer.StoreInt16(this.LALR[num2][num].SymbolIndex());
                        writer.StoreInt16((int)this.LALR[num2][num].Type());
                        writer.StoreInt16(this.LALR[num2][num].Value());
                        writer.StoreEmpty();
                    }
                }
                writer.Close();
                flag2 = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag2 = false;
            }
            return flag2;
        }

        public bool SaveXML1(string FileName)
        {
            int num3;
            bool flag;
            int tableIndex = -1;
            int num4 = this.m_Group.Count - 1;
            for (num3 = 0; num3 <= num4; num3++)
            {
                Group group = this.m_Group[num3];
                if (group.Name.ToUpper() == "COMMENT LINE")
                {
                    tableIndex = group.Start.TableIndex;
                }
                group = null;
            }
            string str = Strings.Space(8);
            try
            {
                int num2;
                TextWriter writer = new StreamWriter(FileName, false);
                writer.WriteLine("<?GOLDParserTables version={0}1.0{0}?>", '"');
                writer.WriteLine("<Tables>");
                writer.WriteLine("{0}<Parameters>", str);
                writer.WriteLine("{0}{0}<Parameter Name={1}Name{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_Properties["Name"].Value));
                writer.WriteLine("{0}{0}<Parameter Name={1}Author{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_Properties["Author"].Value));
                writer.WriteLine("{0}{0}<Parameter Name={1}Version{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_Properties["Version"].Value));
                writer.WriteLine("{0}{0}<Parameter Name={1}About{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_Properties["About"].Value));
                writer.WriteLine("{0}{0}<Parameter Name={1}Case Sensitive{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_Properties["Case Sensitive"].Value));
                writer.WriteLine("{0}{0}<Parameter Name={1}Start Symbol{1} Value={1}{2}{1}/>", str, '"', BuilderUtility.XMLText(this.m_StartSymbol.Text(false)));
                writer.WriteLine("{0}</Parameters>", str);
                writer.WriteLine("{0}<SymbolTable Count={1}{2}{1}>", str, '"', this.m_Symbol.Count());
                int num5 = this.m_Symbol.Count() - 1;
                for (num3 = 0; num3 <= num5; num3++)
                {
                    SymbolType lEGACYCommentLine;
                    Symbol symbol = this.m_Symbol[num3];
                    if (num3 == tableIndex)
                    {
                        lEGACYCommentLine = SymbolType.LEGACYCommentLine;
                    }
                    else
                    {
                        lEGACYCommentLine = symbol.Type;
                    }
                    writer.WriteLine("{0}{0}<Symbol Index={1}{2}{1} Name={1}{3}{1} Kind={1}{4}{1}/>", new object[] { str, '"', num3, BuilderUtility.XMLText(symbol.Name), Conversion.Int((int)lEGACYCommentLine) });
                    symbol = null;
                }
                writer.WriteLine("{0}</SymbolTable>", str);
                writer.WriteLine("{0}<RuleTable Count={1}{2}{1}>", str, '"', this.m_Production.Count());
                int num6 = this.m_Production.Count() - 1;
                for (num3 = 0; num3 <= num6; num3++)
                {
                    Production production = this.m_Production[num3];
                    writer.WriteLine("{0}{0}<Rule Index={1}{2}{1} NonTerminalIndex={1}{3}{1} SymbolCount={1}{4}{1}>", new object[] { str, '"', num3, production.Head.TableIndex, production.Handle().Count() });
                    int num7 = production.Handle().Count() - 1;
                    num2 = 0;
                    while (num2 <= num7)
                    {
                        writer.WriteLine("{0}{0}{0}<RuleSymbol SymbolIndex={1}{2}{1}/>", str, '"', production.Handle()[num2].TableIndex);
                        num2++;
                    }
                    writer.WriteLine("{0}{0}</Rule>", str);
                    production = null;
                }
                writer.WriteLine("{0}</RuleTable>", str);
                writer.WriteLine("{0}<CharSetTable Count={1}{2}{1}>", str, '"', this.m_CharSet.Count);
                int num8 = this.m_CharSet.Count - 1;
                for (num3 = 0; num3 <= num8; num3++)
                {
                    CharacterSet set = this.m_CharSet[num3];
                    writer.WriteLine("{0}{0}<CharSet Index={1}{2}{1} Count={1}{3}{1}>", new object[] { str, '"', num3, set.Count() });
                    int num9 = set.Count() - 1;
                    num2 = 0;
                    while (num2 <= num9)
                    {
                        writer.WriteLine("{0}{0}{0}<Char UnicodeIndex={1}{2}{1}/>", str, '"', set[num2]);
                        num2++;
                    }
                    writer.WriteLine("{0}{0}</CharSet>", str);
                    set = null;
                }
                writer.WriteLine("{0}</CharSetTable>", str);
                writer.WriteLine("{0}<DFATable Count={1}{2}{1} InitialState={1}{3}{1}>", new object[] { str, '"', this.m_DFA.Count, this.DFA.InitialState });
                int num10 = this.DFA.Count - 1;
                for (num3 = 0; num3 <= num10; num3++)
                {
                    FAState state = this.DFA[num3];
                    writer.WriteLine("{0}{0}<DFAState Index={1}{2}{1} EdgeCount={1}{3}{1} AcceptSymbol={1}{4}{1}>", new object[] { str, '"', num3, state.Edges().Count(), state.AcceptIndex() });
                    int num11 = state.Edges().Count() - 1;
                    num2 = 0;
                    while (num2 <= num11)
                    {
                        writer.WriteLine("{0}{0}{0}<DFAEdge CharSetIndex={1}{2}{1} Target={1}{3}{1}/>", new object[] { str, '"', state.Edges()[num2].Characters.TableIndex, state.Edges()[num2].Target });
                        num2++;
                    }
                    writer.WriteLine("{0}{0}</DFAState>", str);
                    state = null;
                }
                writer.WriteLine("{0}</DFATable>", str);
                writer.WriteLine("{0}<LALRTable Count={1}{2}{1} InitialState={1}{3}{1}>", new object[] { str, '"', this.LALR.Count, this.LALR.InitialState });
                int num12 = this.LALR.Count - 1;
                for (num3 = 0; num3 <= num12; num3++)
                {
                    LRState state2 = this.LALR[num3];
                    writer.WriteLine("{0}{0}<LALRState Index={1}{2}{1} ActionCount={1}{3}{1}>", new object[] { str, '"', num3, state2.Count });
                    int num13 = state2.Count - 1;
                    for (num2 = 0; num2 <= num13; num2++)
                    {
                        writer.WriteLine("{0}{0}{0}<LALRAction SymbolIndex={1}{2}{1} Action={1}{3}{1} Value={1}{4}{1}/>", new object[] { str, '"', state2[(short)num2].SymbolIndex(), Conversion.Int((int)state2[(short)num2].Type()), state2[(short)num2].Value() });
                    }
                    writer.WriteLine("{0}{0}</LALRState>", str);
                    state2 = null;
                }
                writer.WriteLine("{0}</LALRTable>", str);
                writer.WriteLine("</Tables>");
                writer.Close();
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = false;
            }
            return flag;
        }

        public bool SaveXML5(string FileName)
        {
            bool flag;
            string str = Strings.Space(8);
            try
            {
                int num;
                int num2;
                TextWriter writer = new StreamWriter(FileName, false);
                writer.WriteLine("<?GOLDParserTables version={0}5.0{0}?>", '"');
                writer.WriteLine("<Tables>");
                writer.WriteLine("{0}<Properties>", str);
                writer.WriteLine("{0}{0}<Property Index=\"0\" Name=\"Name\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Name"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"1\" Name=\"Author\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Author"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"2\" Name=\"Version\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Version"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"3\" Name=\"About\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["About"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"4\" Name=\"Character Set\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Character Set"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"5\" Name=\"Character Mapping\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Character Mapping"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"6\" Name=\"Generated By\" Value=\"{1}\"/>", str, BuilderUtility.XMLText(this.m_Properties["Generated By"].Value));
                writer.WriteLine("{0}{0}<Property Index=\"7\" Name=\"Generated Date\" Value=\"{1}\"/>", str, this.m_Properties["Generated Date"].Value);
                writer.WriteLine("{0}</Properties>", str);
                writer.WriteLine("{0}<m_Symbol Count=\"{1}\">", str, this.m_Symbol.Count());
                int num3 = this.m_Symbol.Count() - 1;
                for (num2 = 0; num2 <= num3; num2++)
                {
                    Symbol symbol = this.m_Symbol[num2];
                    writer.WriteLine("{0}{0}<Symbol Index=\"{1}\" Name=\"{2}\" Type=\"{3}\"/>", new object[] { str, num2, BuilderUtility.XMLText(symbol.Name), Conversion.Int((int)symbol.Type) });
                    symbol = null;
                }
                writer.WriteLine("{0}</m_Symbol>", str);
                writer.WriteLine("{0}<m_Group Count=\"{1}\">", str, this.m_Group.Count);
                int num4 = this.m_Group.Count - 1;
                for (num2 = 0; num2 <= num4; num2++)
                {
                    Group group = this.m_Group[num2];
                    writer.WriteLine("{0}{0}<Group Index=\"{1}\" Name=\"{2}\" ContainerIndex=\"{3}\" StartIndex=\"{4}\" EndIndex=\"{5}\" Advance=\"{6}\" Ending=\"{7}\" NestingCount=\"{8}\">", new object[] { str, num2, BuilderUtility.XMLText(group.Name), group.Container.TableIndex, group.Start.TableIndex, group.End.TableIndex, Conversion.Int((int)group.Advance), Conversion.Int((int)group.Ending), group.Nesting.Count });
                    int num5 = group.Nesting.Count - 1;
                    num = 0;
                    while (num <= num5)
                    {
                        writer.WriteLine("{0}{0}{0}<NestedGroup Index=\"{1}\"/>", str, group.Nesting[num]);
                        num++;
                    }
                    writer.WriteLine("{0}{0}</Group>", str);
                    group = null;
                }
                writer.WriteLine("{0}</m_Group>", str);
                writer.WriteLine("{0}<m_Production Count=\"{1}\">", str, this.m_Production.Count());
                int num6 = this.m_Production.Count() - 1;
                for (num2 = 0; num2 <= num6; num2++)
                {
                    Production production = this.m_Production[num2];
                    writer.WriteLine("{0}{0}<Production Index=\"{1}\" NonTerminalIndex=\"{2}\" SymbolCount=\"{3}\">", new object[] { str, num2, production.Head.TableIndex, production.Handle().Count() });
                    int num7 = production.Handle().Count() - 1;
                    num = 0;
                    while (num <= num7)
                    {
                        writer.WriteLine("{0}{0}{0}<ProductionSymbol SymbolIndex=\"{1}\"/>", str, production.Handle()[num].TableIndex);
                        num++;
                    }
                    writer.WriteLine("{0}{0}</Production>", str);
                    production = null;
                }
                writer.WriteLine("{0}</m_Production>", str);
                writer.WriteLine("{0}<m_CharSet Count=\"{1}\">", str, this.m_CharSet.Count);
                int num8 = this.m_CharSet.Count - 1;
                for (num2 = 0; num2 <= num8; num2++)
                {
                    CharacterSet set = this.m_CharSet[num2];
                    writer.WriteLine("{0}{0}<CharSet Index=\"{1}\" Count=\"{2}\">", str, num2, set.Count());
                    int num9 = set.Count() - 1;
                    num = 0;
                    while (num <= num9)
                    {
                        writer.WriteLine("{0}{0}{0}<Char UnicodeIndex=\"{1}\"/>", str, set[num]);
                        num++;
                    }
                    writer.WriteLine("{0}{0}</CharSet>", str);
                    set = null;
                }
                writer.WriteLine("{0}</m_CharSet>", str);
                writer.WriteLine("{0}<DFATable Count=\"{1}\" InitialState=\"{2}\">", str, this.m_DFA.Count, this.m_DFA.InitialState);
                int num10 = this.DFA.Count - 1;
                for (num2 = 0; num2 <= num10; num2++)
                {
                    FAState state = this.DFA[num2];
                    writer.WriteLine("{0}{0}<DFAState Index=\"{1}\" EdgeCount=\"{2}\" AcceptSymbol=\"{3}\">", new object[] { str, num2, state.Edges().Count(), state.AcceptIndex() });
                    int num11 = state.Edges().Count() - 1;
                    num = 0;
                    while (num <= num11)
                    {
                        writer.WriteLine("{0}{0}{0}<DFAEdge CharSetIndex=\"{1}\" Target=\"{2}\"/>", str, state.Edges()[num].Characters.TableIndex, state.Edges()[num].Target);
                        num++;
                    }
                    writer.WriteLine("{0}{0}</DFAState>", str);
                    state = null;
                }
                writer.WriteLine("{0}</DFATable>", str);
                writer.WriteLine("{0}<LALRTable Count=\"{1}\" InitialState=\"{2}\">", str, this.m_LALR.Count, this.m_LALR.InitialState);
                int num12 = this.m_LALR.Count - 1;
                for (num2 = 0; num2 <= num12; num2++)
                {
                    LRState state2 = this.m_LALR[num2];
                    writer.WriteLine("{0}{0}<LALRState Index=\"{1}\" ActionCount=\"{2}\">", str, num2, state2.Count);
                    int num13 = state2.Count - 1;
                    for (num = 0; num <= num13; num++)
                    {
                        writer.WriteLine("{0}{0}{0}<LALRAction SymbolIndex=\"{1}\" Action=\"{2}\" Value=\"{3}\"/>", new object[] { str, state2[(short)num].SymbolIndex(), Conversion.Int((int)state2[(short)num].Type()), state2[(short)num].Value() });
                    }
                    writer.WriteLine("{0}{0}</LALRState>", str);
                    state2 = null;
                }
                writer.WriteLine("{0}</LALRTable>", str);
                writer.WriteLine("</Tables>");
                writer.Close();
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = false;
            }
            return flag;
        }

        // Properties
        public CharacterSetList CharSet
        {
            get
            {
                return this.m_CharSet;
            }
            set
            {
                this.m_CharSet = value;
            }
        }

        public FAStateList DFA
        {
            get
            {
                return this.m_DFA;
            }
            set
            {
                this.m_DFA = value;
            }
        }

        public GroupList Group
        {
            get
            {
                return this.m_Group;
            }
            set
            {
                this.m_Group = value;
            }
        }

        public LRStateList LALR
        {
            get
            {
                return this.m_LALR;
            }
            set
            {
                this.m_LALR = value;
            }
        }

        public ProductionList Production
        {
            get
            {
                return this.m_Production;
            }
            set
            {
                this.m_Production = value;
            }
        }

        public VariableList Properties
        {
            get
            {
                return this.m_Properties;
            }
            set
            {
                this.m_Properties = value;
            }
        }

        public Symbol StartSymbol
        {
            get
            {
                return this.m_StartSymbol;
            }
            set
            {
                this.m_StartSymbol = value;
            }
        }

        public SymbolList Symbol
        {
            get
            {
                return this.m_Symbol;
            }
            set
            {
                this.m_Symbol = value;
            }
        }

        // Nested Types
        protected enum EGTProperty
        {
            Name,
            Version,
            Author,
            About,
            CharacterSet,
            CharacterMapping,
            GeneratedBy,
            GeneratedDate
        }

        protected enum EGTRecord : byte
        {
            CharRanges = 0x63,
            CharSetLiteral = 0x43,
            Counts_1 = 0x54,
            DFAState = 0x44,
            Group = 0x67,
            InitialStates = 0x49,
            LRState = 0x4c,
            ParamRecord = 80,
            Production = 0x52,
            Property = 0x70,
            Symbol = 0x53,
            TableCounts = 0x74
        }
    }
}