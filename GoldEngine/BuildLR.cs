using System.Runtime.InteropServices;

namespace GoldEngine
{
    internal sealed class BuildLR
    {
        private static LRConfigSet[] GotoList;
        private static ConflictTableItem[] m_ConflictTable = new ConflictTableItem[(m_ConflictTableCount - 1) + 1];
        private static short m_ConflictTableCount = 3;

        static BuildLR()
        {
            m_ConflictTable[0] = new ConflictTableItem("Reduce-Reduce", LRActionType.Reduce, LRActionType.Reduce, LRConflict.ReduceReduce);
            m_ConflictTable[1] = new ConflictTableItem("Shift-Reduce", LRActionType.Shift, LRActionType.Reduce, LRConflict.ShiftReduce);
            m_ConflictTable[2] = new ConflictTableItem("Accept-Reduce", LRActionType.Accept, LRActionType.Reduce, LRConflict.AcceptReduce);
        }

        public static void Build()
        {
            short tableIndex = BuilderApp.BuildTables.StartSymbol.TableIndex;
            if (tableIndex == -1)
            {
                BuilderApp.Log.Add(SysLogSection.LALR, SysLogAlert.Critical, "Start symbol is invalid or missing");
            }
            else
            {
                Notify.Started("Computing LALR Tables");
                SymbolBuild build = BuilderApp.BuildTables.Symbol[tableIndex];
                BuilderApp.BuildTables.LALR.Clear();
                SetupTempTables();
                if (build.First.Count() >= 1)
                {
                    short count;
                    bool flag;
                    int num2;
                    LRStateBuild build2;
                    BuilderApp.Mode = BuilderApp.ProgramMode.BuildingLALR;
                    Symbol startSymbol = build;
                    build = (SymbolBuild)startSymbol;
                    CreateLRState(CreateInitialState(startSymbol));
                    do
                    {
                        flag = true;
                        count = (short)BuilderApp.BuildTables.LALR.Count;
                        int num4 = count - 1;
                        num2 = 0;
                        while (num2 <= num4)
                        {
                            build2 = BuilderApp.BuildTables.LALR[num2];
                            if (!build2.Expanded)
                            {
                                flag = false;
                                build2.Expanded = true;
                                ComputeLRState(build2);
                            }
                            num2++;
                        }
                    }
                    while (!flag);
                    do
                    {
                        flag = true;
                        count = (short)BuilderApp.BuildTables.LALR.Count;
                        int num5 = count - 1;
                        for (num2 = 0; num2 <= num5; num2++)
                        {
                            build2 = BuilderApp.BuildTables.LALR[num2];
                            if (build2.Modified)
                            {
                                flag = false;
                                build2.Modified = false;
                                RecomputeLRState(build2);
                            }
                        }
                    }
                    while (!flag);
                    ComputeReductions();
                    Notify.Completed("LALR Tables Completed");
                }
                else
                {
                    BuilderApp.Log.Add(SysLogSection.LALR, SysLogAlert.Critical, "Rule <" + build.Name + "> does not produce any terminals.");
                }
                ClearTempTables();
            }
        }

        public static void Clear()
        {
            BuilderApp.BuildTables.LALR.Clear();
        }

        private static void ClearTempTables()
        {
            GotoList = null;
        }

        public static void Closure(LRConfigSet ConfigSet)
        {
            LRConfigSet setB = new LRConfigSet();
            short num3 = (short)(ConfigSet.Count() - 1);
            for (short i = 0; i <= num3; i = (short)(i + 1))
            {
                LRConfig config2 = ConfigSet[i];
                LookaheadSymbolSet set = TotalLookahead(config2);
                SymbolBuild build = config2.NextSymbol(0);
                if ((build != null) && (build.Type == SymbolType.Nonterminal))
                {
                    LRConfigSet partialClosure = build.PartialClosure;
                    short num4 = (short)(partialClosure.Count() - 1);
                    for (short j = 0; j <= num4; j = (short)(j + 1))
                    {
                        LRConfig config = partialClosure[j];
                        LRConfig item = new LRConfig(config.Parent, 0, config.LookaheadSet);
                        if (config.InheritLookahead)
                        {
                            item.LookaheadSet.UnionWith(set);
                        }
                        setB.Add(item);
                    }
                }
            }
            ConfigSet.UnionWith(setB);
        }

        private static void ComputeLRState(LRStateBuild State)
        {
            short num2;
            SymbolBuild build2;
            short num3 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num3; num2 = (short)(num2 + 1))
            {
                GotoList[num2] = null;
            }
            short num4 = (short)(State.ConfigSet.Count() - 1);
            for (num2 = 0; num2 <= num4; num2 = (short)(num2 + 1))
            {
                LRConfig config = State.ConfigSet[num2];
                build2 = config.NextSymbol(0);
                if (build2 != null)
                {
                    short tableIndex = build2.TableIndex;
                    if (GotoList[tableIndex] == null)
                    {
                        GotoList[tableIndex] = new LRConfigSet();
                    }
                    LRConfig item = new LRConfig(config.Parent, config.Position + 1, config.LookaheadSet);
                    GotoList[tableIndex].Add(item);
                }
            }
            short num5 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num5; num2 = (short)(num2 + 1))
            {
                if (GotoList[num2] == null)
                {
                    continue;
                }
                build2 = BuilderApp.BuildTables.Symbol[num2];
                Closure(GotoList[num2]);
                LRStateBuild state = new LRStateBuild
                {
                    ConfigSet = GotoList[num2]
                };
                switch (build2.Type)
                {
                case SymbolType.Nonterminal:
                {
                    State.Add(new LRAction(build2, LRActionType.Goto, CreateLRState(state)));
                    Notify.Analyzed++;
                    continue;
                }
                case SymbolType.End:
                {
                    State.Add(new LRAction(build2, LRActionType.Accept));
                    continue;
                }
                }
                State.Add(new LRAction(build2, LRActionType.Shift, CreateLRState(state)));
                Notify.Analyzed++;
            }
        }

        private static void ComputePartialClosures()
        {
            short num2;
            short num3 = 0;
            short num4 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num4; num2 = (short)(num2 + 1))
            {
                if (BuilderApp.BuildTables.Symbol[num2].Type == SymbolType.Nonterminal)
                {
                    num3 = (short)(num3 + 1);
                }
            }
            short num = 0;
            short num5 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num5; num2 = (short)(num2 + 1))
            {
                SymbolBuild sym = BuilderApp.BuildTables.Symbol[num2];
                if (sym.Type == SymbolType.Nonterminal)
                {
                    num = (short)(num + 1);
                    Notify.Text = Conversions.ToString((int)num) + " of " + Conversions.ToString((int)num3);
                    sym.PartialClosure = GetClosureConfigSet(sym);
                }
            }
        }

        private static void ComputeReductions()
        {
            LRConflictItem[] itemArray = new LRConflictItem[BuilderApp.BuildTables.Symbol.Count() + 1];
            short num4 = (short)(BuilderApp.BuildTables.LALR.Count - 1);
            for (short i = 0; i <= num4; i = (short)(i + 1))
            {
                LRConflict shiftReduce;
                LRStateBuild build = BuilderApp.BuildTables.LALR[i];
                short num5 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
                short index = 0;
                while (index <= num5)
                {
                    itemArray[index] = new LRConflictItem(BuilderApp.BuildTables.Symbol[index]);
                    index = (short)(index + 1);
                }
                short num6 = (short)(build.ConfigSet.Count() - 1);
                short num2 = 0;
                while (num2 <= num6)
                {
                    short num7;
                    LRConfig item = build.ConfigSet[num2];
                    switch (((int)item.NextAction()))
                    {
                    case 1:
                        itemArray[item.NextSymbol(0).TableIndex].Shifts.Add(item);
                        goto Label_0137;

                    case 2:
                        num7 = (short)(item.LookaheadSet.Count() - 1);
                        index = 0;
                        goto Label_012D;

                    default:
                        goto Label_0137;
                    }
                    Label_0100:
                    itemArray[item.LookaheadSet[index].Parent.TableIndex].Reduces.Add(item);
                    index = (short)(index + 1);
                    Label_012D:
                    if (index <= num7)
                    {
                        goto Label_0100;
                    }
                    Label_0137:
                    num2 = (short)(num2 + 1);
                }
                short num8 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
                index = 0;
                while (index <= num8)
                {
                    if ((itemArray[index].Shifts.Count() >= 1) & (itemArray[index].Reduces.Count() >= 1))
                    {
                        short num9 = (short)(itemArray[index].Shifts.Count() - 1);
                        num2 = 0;
                        while (num2 <= num9)
                        {
                            itemArray[index].Shifts[num2].Status = LRStatus.Warning;
                            num2 = (short)(num2 + 1);
                        }
                        short num10 = (short)(itemArray[index].Reduces.Count() - 1);
                        num2 = 0;
                        while (num2 <= num10)
                        {
                            itemArray[index].Reduces[num2].Status = LRStatus.Warning;
                            num2 = (short)(num2 + 1);
                        }
                        build.Status = LRStatus.Warning;
                        build.Note = "Shift-Reduce Conflict";
                        build.ConflictList.Add(new LRConflictItem(itemArray[index], LRConflict.ShiftReduce));
                        shiftReduce = LRConflict.ShiftReduce;
                        BuilderApp.Log.Add(SysLogSection.LALR, SysLogAlert.Warning, "A Shift-Reduce Conflict was fixed", BuilderApp.BuildTables.Symbol[index].Text(false) + " can follow a completed rule and also be shifted." + GetConflictResolvedDesc(shiftReduce), Conversions.ToString((int)i));
                    }
                    index = (short)(index + 1);
                }
                short num11 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
                for (index = 0; index <= num11; index = (short)(index + 1))
                {
                    if ((itemArray[index].Reduces.Count() == 1) & (itemArray[index].Shifts.Count() == 0))
                    {
                        SymbolBuild theSymbol = BuilderApp.BuildTables.Symbol[index];
                        build.Add(new LRAction(theSymbol, LRActionType.Reduce, itemArray[index].Reduces[0].Parent.TableIndex));
                    }
                    else if (itemArray[index].Reduces.Count() > 1)
                    {
                        short num12 = (short)(itemArray[index].Reduces.Count() - 1);
                        for (num2 = 0; num2 <= num12; num2 = (short)(num2 + 1))
                        {
                            itemArray[index].Reduces[num2].Status = LRStatus.Critical;
                        }
                        build.Status = LRStatus.Critical;
                        build.Note = "Reduce-Reduce Conflict";
                        build.ConflictList.Add(new LRConflictItem(itemArray[index], LRConflict.ReduceReduce));
                        shiftReduce = LRConflict.ReduceReduce;
                        BuilderApp.Log.Add(SysLogSection.LALR, SysLogAlert.Critical, "Reduce-Reduce Conflict", BuilderApp.BuildTables.Symbol[index].Text(false) + " can follow more than one completed rule. " + GetConflictDesc(shiftReduce), Conversions.ToString((int)i));
                    }
                }
            }
        }

        private static LRStateBuild CreateInitialState(Symbol StartSymbol)
        {
            ProductionBuild rule = new ProductionBuild();
            SymbolBuild nonTerminal = new SymbolBuild
            {
                Type = SymbolType.Nonterminal,
                Name = "S'"
            };
            rule.SetHead(nonTerminal);
            rule.Handle().Add((SymbolBuild)StartSymbol);
            rule.Handle().Add((SymbolBuild)BuilderApp.BuildTables.Symbol.GetFirstOfType(SymbolType.End));
            LRConfig item = new LRConfig(rule);
            LRStateBuild build3 = new LRStateBuild();
            build3.ConfigSet.Add(item);
            build3.Expanded = false;
            Closure(build3.ConfigSet);
            return build3;
        }

        private static short CreateLRState(LRStateBuild State)
        {
            LRConfigCompare unEqual;
            short tableIndex = (short)LRConfigSetLookup.get_TableIndex(State.ConfigSet);
            if (tableIndex != -1)
            {
                unEqual = State.ConfigSet.CompareCore(BuilderApp.BuildTables.LALR[tableIndex].ConfigSet);
            }
            else
            {
                unEqual = LRConfigCompare.UnEqual;
            }
            switch (((int)unEqual))
            {
            case 0:
            case 1:
                return tableIndex;

            case 2:
            {
                LRStateBuild build2 = BuilderApp.BuildTables.LALR[tableIndex];
                build2.ConfigSet.UnionWith(State.ConfigSet);
                build2.Modified = true;
                build2 = null;
                return tableIndex;
            }
            case 3:
            {
                LRStateBuild build = State;
                build.Expanded = false;
                build.Modified = true;
                build = null;
                tableIndex = (short)BuilderApp.BuildTables.LALR.Add(State);
                LRConfigSetLookup.Add(State.ConfigSet, tableIndex);
                Notify.Counter++;
                return tableIndex;
            }
            }
            return tableIndex;
        }

        private static LRConfigSet GetClosureConfigSet(SymbolBuild Sym)
        {
            bool flag;
            LRConfig config;
            short num2;
            SymbolBuild build;
            LRConfig config2;
            ProductionBuild build2;
            LRConfigSet setB = new LRConfigSet();
            LRConfigSet set = new LRConfigSet();
            short num3 = (short)(BuilderApp.BuildTables.Production.Count() - 1);
            short num = 0;
            while (num <= num3)
            {
                build2 = BuilderApp.BuildTables.Production[num];
                if (build2.Head.IsEqualTo(Sym))
                {
                    config2 = new LRConfig
                    {
                        Position = 0,
                        Modified = true,
                        LookaheadSet = new LookaheadSymbolSet(),
                        Parent = build2,
                        InheritLookahead = true
                    };
                    set.Add(config2);
                }
                num = (short)(num + 1);
            }
            do
            {
                setB.Clear();
                flag = false;
                short num4 = (short)(set.Count() - 1);
                num2 = 0;
                while (num2 <= num4)
                {
                    config = set[num2];
                    if (!config.IsComplete() & config.Modified)
                    {
                        build = config.NextSymbol(0);
                        if (build.Type == SymbolType.Nonterminal)
                        {
                            short num5 = (short)(BuilderApp.BuildTables.Production.Count() - 1);
                            num = 0;
                            while (num <= num5)
                            {
                                build2 = BuilderApp.BuildTables.Production[num];
                                if (build2.Head.IsEqualTo(build))
                                {
                                    config2 = new LRConfig(build2, 0, TotalLookahead(config));
                                    setB.Add(config2);
                                }
                                num = (short)(num + 1);
                            }
                        }
                        set[num2].Modified = false;
                    }
                    num2 = (short)(num2 + 1);
                }
            }
            while (set.UnionWith(setB));
            do
            {
                flag = false;
                short num6 = (short)(set.Count() - 1);
                for (num2 = 0; num2 <= num6; num2 = (short)(num2 + 1))
                {
                    config = set[num2];
                    build = config.NextSymbol(0);
                    if ((build != null) && ((config.InheritLookahead & PopulateLookahead(config)) & (build.Type == SymbolType.Nonterminal)))
                    {
                        short num7 = (short)(set.Count() - 1);
                        for (num = 0; num <= num7; num = (short)(num + 1))
                        {
                            config = set[num];
                            if (((config.Position == 0) & config.Parent.Head.IsEqualTo(build)) & !config.InheritLookahead)
                            {
                                config.InheritLookahead = true;
                                flag = true;
                            }
                        }
                    }
                }
            }
            while (flag);
            return set;
        }

        public static LRConflict GetConflict(LRActionType Action1, LRActionType Action2)
        {
            LRConflict none = LRConflict.None;
            bool flag = false;
            for (short i = 0; (i < m_ConflictTableCount) & !flag; i = (short)(i + 1))
            {
                ConflictTableItem[] conflictTable = m_ConflictTable;
                int index = i;
                if (((conflictTable[index].Action1 == Action1) & (conflictTable[index].Action2 == Action2)) | ((conflictTable[index].Action1 == Action2) & (conflictTable[index].Action2 == Action1)))
                {
                    none = m_ConflictTable[i].Conflict;
                    flag = true;
                }
            }
            return none;
        }

        public static string GetConflictDesc(LRConflict Conflict)
        {
            string str2;
            switch (((int)Conflict))
            {
            case 0:
                return "There is no conflict.";

            case 1:
                str2 = "";
                return (((str2 + "A Shift-Reduce Conflict is caused when the system cannot " + "determine whether to advance (shift) one rule or complete ") + "(reduce) another. This means that the *same* text can be parsed " + "into two or more distrinct trees at the same time. ") + "The grammar is ambigious. " + "Please see the documentation for more information.");

            case 2:
                str2 = "";
                return (((str2 + "A Reduce-Reduce error is a caused when a grammar allows ") + "two or more rules to be reduced at the same time, for the " + "same token. ") + "The grammar is ambigious. " + "Please see the documentation for more information.");

            case 3:
                return "This NEVER happens";
            }
            return "Unknown";
        }

        public static string GetConflictName(LRConflict Conflict)
        {
            switch (((int)Conflict))
            {
            case 0:
                return "None";

            case 1:
                return "Shift-Reduce";

            case 2:
                return "Reduce-Reduce";

            case 3:
                return "Accept-Reduce";
            }
            return "Unknown";
        }

        public static string GetConflictResolvedDesc(LRConflict Conflict)
        {
            switch (((int)Conflict))
            {
            case 0:
                return "";

            case 1:
            {
                string str2 = "";
                return ((str2 + "The conflict was resolved by selecting the 'shift' action over the 'reduce'. ") + "Be careful, some parts grammar may not be accessable. " + "It is recommended that you attempt to remove all conflicts.");
            }
            }
            return "This conflict cannot be automatically resolved";
        }

        private static StateTableInfoType GetStateInfo(LRStateBuild State)
        {
            StateTableInfoType type2;
            type2.Index = -1;
            type2.Compare = LRConfigCompare.UnEqual;
            short num = 0;
            while ((num < BuilderApp.BuildTables.LALR.Count) & (type2.Index == -1))
            {
                if (State.ConfigSet.EqualCore(BuilderApp.BuildTables.LALR[num].ConfigSet))
                {
                    LRConfigCompare compare = State.ConfigSet.CompareCore(BuilderApp.BuildTables.LALR[num].ConfigSet);
                    type2.Index = num;
                    type2.Compare = compare;
                }
                else
                {
                    num = (short)(num + 1);
                }
            }
            return type2;
        }

        private static LRStateBuild GotoSymbol(LRStateBuild State, SymbolBuild TheSymbol)
        {
            LRStateBuild build2 = new LRStateBuild();
            short num2 = (short)(State.ConfigSet.Count() - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                LRConfig config = State.ConfigSet[i];
                Symbol symbol = config.NextSymbol(0);
                if ((symbol != null) && symbol.IsEqualTo(TheSymbol))
                {
                    LRConfig item = new LRConfig(config.Parent, config.Position + 1, config.LookaheadSet);
                    build2.ConfigSet.Add(item);
                }
            }
            if (build2.ConfigSet.Count() >= 1)
            {
                Closure(build2.ConfigSet);
            }
            return build2;
        }

        private static bool PopulateLookahead(LRConfig Config)
        {
            if (Config.InheritLookahead)
            {
                bool flag;
                for (short i = 0; (i < Config.CheckaheadCount()) & !flag; i = (short)(i + 1))
                {
                    flag = !Config.Checkahead(i).Nullable;
                }
                return !flag;
            }
            return false;
        }

        private static void RecomputeLRState(LRStateBuild State)
        {
            short num2 = (short)(State.Count - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                LRAction action = State[i];
                switch (((int)action.Type()))
                {
                case 1:
                case 3:
                {
                    Symbol symbol = action.Symbol;
                    SymbolBuild theSymbol = (SymbolBuild)symbol;
                    symbol = theSymbol;
                    LRStateBuild build = GotoSymbol(State, theSymbol);
                    LRStateBuild build2 = BuilderApp.BuildTables.LALR[action.Value()];
                    if (build.ConfigSet.CompareCore(build2.ConfigSet) == LRConfigCompare.EqualCore)
                    {
                        build2.ConfigSet.UnionWith(build.ConfigSet);
                        build2.Modified = true;
                    }
                    Notify.Analyzed++;
                    break;
                }
                }
            }
        }

        private static void SetupFirstTable()
        {
            bool flag;
            short num2;
            SymbolBuild build2;
            SetupNullableTable();
            short num4 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num4; num2 = (short)(num2 + 1))
            {
                BuilderApp.BuildTables.Symbol[num2].First.Clear();
            }
            short num5 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            num2 = 0;
            while (num2 <= num5)
            {
                build2 = BuilderApp.BuildTables.Symbol[num2];
                if (build2.Type != SymbolType.Nonterminal)
                {
                    build2.First.Add(new LookaheadSymbol(build2));
                }
                num2 = (short)(num2 + 1);
            }
            do
            {
                flag = false;
                short num6 = (short)(BuilderApp.BuildTables.Production.Count() - 1);
                for (num2 = 0; num2 <= num6; num2 = (short)(num2 + 1))
                {
                    ProductionBuild build = BuilderApp.BuildTables.Production[num2];
                    short num3 = 0;
                    for (bool flag2 = false; (num3 < build.Handle().Count()) & !flag2; flag2 = !build2.Nullable)
                    {
                        build2 = build.Handle()[num3];
                        short tableIndex = build2.TableIndex;
                        if (build.Head.First.UnionWith(build2.First))
                        {
                            flag = true;
                        }
                        num3 = (short)(num3 + 1);
                    }
                }
            }
            while (flag);
        }

        private static void SetupNullableTable()
        {
            bool flag;
            short num3 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            short num = 0;
            while (num <= num3)
            {
                BuilderApp.BuildTables.Symbol[num].Nullable = false;
                num = (short)(num + 1);
            }
            do
            {
                flag = false;
                short num4 = (short)(BuilderApp.BuildTables.Production.Count() - 1);
                for (num = 0; num <= num4; num = (short)(num + 1))
                {
                    ProductionBuild build = BuilderApp.BuildTables.Production[num];
                    bool flag2 = true;
                    for (short i = 0; (i < build.Handle().Count()) & flag2; i = (short)(i + 1))
                    {
                        SymbolBuild build2 = build.Handle()[i];
                        if (!build2.Nullable)
                        {
                            flag2 = false;
                        }
                    }
                    if (flag2 & !build.Head.Nullable)
                    {
                        build.Head.Nullable = true;
                        flag = true;
                    }
                }
            }
            while (flag);
        }

        private static void SetupTempTables()
        {
            LRConfigSetLookup.Clear();
            BuilderApp.Mode = BuilderApp.ProgramMode.BuildingFirstSets;
            SetupFirstTable();
            BuilderApp.Mode = BuilderApp.ProgramMode.BuildingLALRClosure;
            ComputePartialClosures();
            GotoList = new LRConfigSet[BuilderApp.BuildTables.Symbol.Count() + 1];
        }

        private static void StateNumber(LRStateBuild State, short Number, LRConfigCompare Status)
        {
            LRConfigCompare compare;
            short num = -1;
            for (short i = 0; (i < BuilderApp.BuildTables.LALR.Count) & (num == -1); i = (short)(i + 1))
            {
                compare = State.ConfigSet.CompareCore(BuilderApp.BuildTables.LALR[i].ConfigSet);
                if (compare != LRConfigCompare.UnEqual)
                {
                    num = i;
                }
            }
            Number = num;
            if (num == -1)
            {
                Status = LRConfigCompare.UnEqual;
            }
            else
            {
                Status = compare;
            }
        }

        private static LookaheadSymbolSet TotalLookahead(LRConfig Config)
        {
            bool flag;
            LookaheadSymbolSet set = new LookaheadSymbolSet();
            for (int i = 0; (i < Config.CheckaheadCount()) & !flag; i++)
            {
                short offset = (short)i;
                i = offset;
                SymbolBuild build = Config.Checkahead(offset);
                ConfigTrackSource source = (ConfigTrackSource)Conversions.ToInteger(Interaction.IIf(build.Type == SymbolType.Nonterminal, ConfigTrackSource.First, ConfigTrackSource.Config));
                int num4 = build.First.Count() - 1;
                for (int j = 0; j <= num4; j++)
                {
                    LookaheadSymbol item = new LookaheadSymbol(build.First[j]);
                    if (item.Parent.Type != SymbolType.Nonterminal)
                    {
                        item.Configs.Add(new ConfigTrack(Config, source));
                        set.Add(item);
                    }
                }
                flag = !build.Nullable;
            }
            if (!flag)
            {
                set.UnionWith(Config.LookaheadSet);
            }
            return set;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConflictTableItem
        {
            public string Name;
            public LRActionType Action1;
            public LRActionType Action2;
            public LRConflict Conflict;
            public ConflictTableItem(string NewName, LRActionType NewAction1, LRActionType NewAction2, LRConflict NewConflict)
            {
                this = new BuildLR.ConflictTableItem();
                this.Name = NewName;
                this.Action1 = NewAction1;
                this.Action2 = NewAction2;
                this.Conflict = NewConflict;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StateTableInfoType
        {
            public int Index;
            public LRConfigCompare Compare;
        }
    }
}