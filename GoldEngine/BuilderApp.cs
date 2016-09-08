using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace GoldEngine
{
    internal sealed class BuilderApp
    {
        // Fields
        public const string APP_VERSION_FULL = "5.2.0.";
        public const string APP_VERSION_TITLE = "5.2";
        public static SysLog BOOTLOG = new SysLog();
        public static ParseTablesBuild BuildTables = new ParseTablesBuild();
        public static bool CancelTableBuild;
        public static bool FatalLoadError;
        public static string FatalLoadMessage;
        public const bool IN_PROGRESS = false;
        public static SysLog Log = new SysLog();
        public static ProgramMode Mode;
        internal static DefinedCharacterSetList PredefinedSets = new DefinedCharacterSetList();
        public const short RANK_FA_FIXED_LENGTH = 0;
        public const short RANK_FA_UNRANKED = -1;
        public const short RANK_FA_VARIABLE_LENGTH = 0x2711;
        public const short RANK_LR_REDUCE = 0x2712;
        public const short RANK_LR_SHIFT = 0x2711;
        public const short RANK_LR_UNRANKED = -1;
        public static SysLog SaveCGTWarnings = new SysLog();
        public static DefinedCharacterSetList UserDefinedSets = new DefinedCharacterSetList();

        // Methods
        public static string About()
        {
            string str2 = "";
            return (((str2 + "Admittedly, this is not a particularly clever acronym, but it does (in part) represent the history of the greater Sacramento Area.") + "\r\n\r\n" + "This application is shareware. You are completely FREE to use this application for your projects, but I hope that you will SHARE your feedback and suggestions with me!") + "\r\n\r\n" + "Only with your input will this become a respected and useful programmer's tool. Happy programming!");
        }

        public static short AddCharacterSet(CharacterSetBuild Chars)
        {
            short num2 = -1;
            for (short i = 0; (i < BuildTables.CharSet.Count) & (num2 == -1); i = (short)(i + 1))
            {
                if (BuildTables.CharSet[i].IsEqualSet(Chars))
                {
                    num2 = i;
                }
            }
            if (num2 == -1)
            {
                num2 = (short)BuildTables.CharSet.Add(Chars);
                Chars.TableIndex = num2;
            }
            return num2;
        }

        public static bool AddProduction(ProductionBuild NewRule)
        {
            bool flag2 = false;
            for (short i = 0; !flag2 & (i < BuildTables.Production.Count()); i = (short)(i + 1))
            {
                if (BuildTables.Production[i].Equals(NewRule))
                {
                    flag2 = true;
                }
            }
            if (!flag2)
            {
                BuildTables.Production.Add(NewRule);
            }
            return !flag2;
        }

        private static void ApplyGroupAttributes()
        {
            IEnumerator enumerator = null;
            try
            {
                enumerator = Grammar.GroupAttributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GrammarAttribAssign current = (GrammarAttribAssign)enumerator.Current;
                    int num3 = BuildTables.Group.ItemIndex(current.Name);
                    if (num3 == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Attributes set for undefined terminal/group", "\"" + current.Name + "\" was set, but it was not defined.", "");
                    }
                    else
                    {
                        GroupBuild build = BuildTables.Group[num3];
                        int num4 = current.Values.Count - 1;
                        for (int i = 0; i <= num4; i++)
                        {
                            string str4;
                            GrammarAttrib attrib = current.Values[i];
                            bool flag = false;
                            string str = attrib.Name.ToUpper();
                            switch (str)
                            {
                            case "ADVANCE":
                            {
                                string str2 = attrib.Value(", ").ToUpper();
                                if (str2 == "TOKEN")
                                {
                                    build.Advance = AdvanceMode.Token;
                                }
                                else if (str2 == "CHARACTER")
                                {
                                    build.Advance = AdvanceMode.Character;
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                            case "NESTING":
                                int num;
                                switch (attrib.Value(", ").ToUpper())
                                {
                                case "ALL":
                                {
                                    build.NestingNames = "All";
                                    int num5 = BuildTables.Group.Count - 1;
                                    num = 0;
                                    while (num <= num5)
                                    {
                                        build.Nesting.Add(num);
                                        num++;
                                    }
                                    goto Label_0353;
                                }
                                case "SELF":
                                    build.Nesting.Add(build.TableIndex);
                                    build.NestingNames = "Self";
                                    goto Label_0353;

                                case "NONE":
                                    build.NestingNames = "None";
                                    goto Label_0353;
                                }
                                if (attrib.IsSet)
                                {
                                    int num6 = attrib.List.Count - 1;
                                    for (num = 0; num <= num6; num++)
                                    {
                                        num3 = BuildTables.Group.ItemIndex(attrib.List[num]);
                                        if (num3 == -1)
                                        {
                                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined group", "Nesting attribute assignment for the group " + build.Name + " is invalid. The following was specified: " + attrib.List[num], "");
                                        }
                                        else
                                        {
                                            build.Nesting.Add(num3);
                                        }
                                    }
                                    build.NestingNames = attrib.Value(", ");
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;

                            case "ENDING":
                                str4 = attrib.Value(", ").ToUpper();
                                if (str4 == "OPEN")
                                {
                                    build.Ending = EndingMode.Open;
                                }
                                else if (str4 == "CLOSED")
                                {
                                    build.Ending = EndingMode.Closed;
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;

                            default:
                                if (str == "ENDING")
                                {
                                    str4 = attrib.Value(", ").ToUpper();
                                    if (str4 == "OPEN")
                                    {
                                        build.Ending = EndingMode.Open;
                                    }
                                    else if (str4 == "CLOSED")
                                    {
                                        build.Ending = EndingMode.Closed;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                                break;
                            }
                            Label_0353:
                            if (flag)
                            {
                                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid attribute", "In the attribute assignment for '" + current.Name + "', the following was specified: " + attrib.Name + " = " + attrib.Value(", "), Conversions.ToString(current.Line));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void ApplySymbolAttributes()
        {
            IEnumerator enumerator = null;
            try
            {
                enumerator = Grammar.SymbolAttributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GrammarAttribAssign current = (GrammarAttribAssign)enumerator.Current;
                    int num2 = BuildTables.Symbol.ItemIndex(current.Name);
                    if (num2 == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Attributes set for undefined terminal/group", "\"" + current.Name + "\" was set, but it was not defined.", "");
                    }
                    else
                    {
                        SymbolBuild build = BuildTables.Symbol[num2];
                        if (((build.Type == SymbolType.Content) | (build.Type == SymbolType.Noise)) | (build.Type == SymbolType.GroupEnd))
                        {
                            int num3 = current.Values.Count - 1;
                            for (int i = 0; i <= num3; i++)
                            {
                                GrammarAttrib attrib = current.Values[i];
                                bool flag = false;
                                switch (attrib.Name.ToUpper())
                                {
                                case "TYPE":
                                {
                                    string str2 = attrib.Value(", ").ToUpper();
                                    if (str2 == "NOISE")
                                    {
                                        build.Type = SymbolType.Noise;
                                    }
                                    else if (str2 == "CONTENT")
                                    {
                                        build.Type = SymbolType.Content;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                    break;
                                }
                                case "SOURCE":
                                {
                                    string str3 = attrib.Value(", ").ToUpper();
                                    if (str3 == "VIRTUAL")
                                    {
                                        build.UsesDFA = false;
                                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, build.Name + " is a virtual terminal", "This terminal was entered into the symbol table, but not the Deterministic Finite Automata. As a result, tokens must be created at runtime by the developer or a specialized version of the Engine.", "");
                                    }
                                    else if (str3 == "LEXER")
                                    {
                                        build.UsesDFA = true;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                    break;
                                }
                                default:
                                    flag = true;
                                    break;
                                }
                                if (flag)
                                {
                                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid attribute", "In the attribute assignment for '" + current.Name + "', the following was specified: " + attrib.Name + " = " + attrib.Value(", "), Conversions.ToString(current.Line));
                                }
                            }
                        }
                        else
                        {
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Cannot change attributes", "The attributes for '" + current.Name + "' cannot be changed.", "");
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            ApplyVirtualProperty();
        }

        private static void ApplyVirtualProperty()
        {
            if (BuildTables.Properties.Contains("Virtual Terminals"))
            {
                string[] source = Strings.Split(BuildTables.Properties["Virtual Terminals"].Value, " ", -1, CompareMethod.Binary);
                int num2 = source.Count<string>() - 1;
                for (int i = 0; i <= num2; i++)
                {
                    string name = source[i].Trim();
                    if (name != "")
                    {
                        BuildTables.Symbol.AddUnique(new SymbolBuild(name, SymbolType.Content, false)).UsesDFA = false;
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, name + " is a virtual terminal", "This terminal was entered into the symbol table, but not the Deterministic Finite Automata. As a result, tokens must be created at runtime by the developer or a specialized version of the Engine.", "");
                    }
                }
            }
        }

        public static string AppName()
        {
            return "GOLD Parser Builder";
        }

        public static string AppNameDesc()
        {
            return "Grammar Oriented Language Developer";
        }

        public static string AppNameVersionFull()
        {
            return "GOLD Parser Builder 5.2.0.";
        }

        public static string AppNameVersionTitle()
        {
            return "GOLD Parser Builder 5.2";
        }

        public static string AppVersionFull()
        {
            return "5.2.0.";
        }

        private static void AssignPriorities()
        {
            int num3 = BuildTables.Symbol.Count() - 1;
            for (int i = 0; i <= num3; i++)
            {
                SymbolBuild build = BuildTables.Symbol[i];
                if ((build.Category() == SymbolCategory.Terminal) & (build.RegularExp != null))
                {
                    bool flag = false;
                    RegExp regularExp = build.RegularExp;
                    int num4 = regularExp.Count() - 1;
                    for (int j = 0; j <= num4; j++)
                    {
                        RegExpSeq seq = regularExp[j];
                        if (seq.Priority == -1)
                        {
                            if (seq.IsVariableLength())
                            {
                                seq.Priority = 0x2711;
                                flag = true;
                            }
                            else
                            {
                                seq.Priority = 0;
                            }
                        }
                    }
                    build.VariableLength = flag;
                    if (flag)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, build.Name + " is variable length.");
                    }
                }
            }
        }

        private static void AssignTableIndexes()
        {
            short num2;
            short num3 = (short)((BuildTables.Symbol.Count() - 1) - 1);
            for (num2 = 0; num2 <= num3; num2 = (short)(num2 + 1))
            {
                short num4 = (short)(BuildTables.Symbol.Count() - 1);
                for (short i = num2; i <= num4; i = (short)(i + 1))
                {
                    SymbolBuildList list = BuildTables.Symbol;
                    short num5 = num2;
                    SymbolBuild build = list[num5];
                    list[num5] = build;
                    if (BuildTables.Symbol[i].IsLessThan(build))
                    {
                        Symbol symbol = BuildTables.Symbol[num2];
                        BuildTables.Symbol[num2] = BuildTables.Symbol[i];
                        BuildTables.Symbol[i] = (SymbolBuild)symbol;
                    }
                }
            }
            short num6 = (short)(BuildTables.Symbol.Count() - 1);
            for (num2 = 0; num2 <= num6; num2 = (short)(num2 + 1))
            {
                BuildTables.Symbol[num2].SetTableIndex(num2);
            }
            short num7 = (short)(BuildTables.Production.Count() - 1);
            for (num2 = 0; num2 <= num7; num2 = (short)(num2 + 1))
            {
                BuildTables.Production[num2].SetTableIndex(num2);
            }
            short num8 = (short)(BuildTables.Group.Count - 1);
            for (num2 = 0; num2 <= num8; num2 = (short)(num2 + 1))
            {
                BuildTables.Group[num2].TableIndex = num2;
            }
        }

        public static bool CanUseDFA()
        {
            return (BuildTables.DFA.Count >= 1);
        }

        public static bool CanUseLALR()
        {
            return (BuildTables.LALR.Count >= 1);
        }

        public static bool CanUserDefinedSetTable()
        {
            return (UserDefinedSets.Count >= 1);
        }

        public static bool CanUseRuleTable()
        {
            return (BuildTables.Production.Count() >= 1);
        }

        public static bool CanUseSymbolTable()
        {
            return (BuildTables.Symbol.Count() >= 1);
        }

        private static void CheckDuplicateGroupStart()
        {
            short num;
            GroupBuildList[] listArray = new GroupBuildList[BuildTables.Symbol.Count() + 1];
            short num2 = (short)(BuildTables.Symbol.Count() - 1);
            for (num = 0; num <= num2; num = (short)(num + 1))
            {
                listArray[num] = new GroupBuildList();
            }
            short num3 = (short)(BuildTables.Group.Count - 1);
            for (num = 0; num <= num3; num = (short)(num + 1))
            {
                GroupBuild item = BuildTables.Group[num];
                listArray[item.Start.TableIndex].Add(item);
            }
            short num4 = (short)(BuildTables.Symbol.Count() - 1);
            for (num = 0; num <= num4; num = (short)(num + 1))
            {
                if (listArray[num].Count >= 2)
                {
                    SymbolBuild build2 = BuildTables.Symbol[num];
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Symbol used to start multiple groups", "The symbol '" + build2.Name + "' is used in the following groups: " + listArray[num].ToString(), "");
                }
            }
        }

        private static void CheckIfUsed(bool[] Checked, int NonTerminalIndex)
        {
            short num3 = (short)(BuildTables.Production.Count() - 1);
            for (short i = 0; i <= num3; i = (short)(i + 1))
            {
                ProductionBuild build2 = BuildTables.Production[i];
                if (build2.Head.TableIndex == NonTerminalIndex)
                {
                    Checked[NonTerminalIndex] = true;
                    short num4 = (short)(build2.Handle().Count() - 1);
                    for (short j = 0; j <= num4; j = (short)(j + 1))
                    {
                        SymbolBuild build = build2.Handle()[j];
                        switch (build.Type)
                        {
                        case SymbolType.Nonterminal:
                            if (!Checked[build.TableIndex])
                            {
                                CheckIfUsed(Checked, build.TableIndex);
                            }
                            break;

                        case SymbolType.Content:
                            Checked[build.TableIndex] = true;
                            break;
                        }
                    }
                }
            }
        }

        private static void CheckIllegalSymbols()
        {
            short num3 = (short)(BuildTables.Production.Count() - 1);
            for (short i = 0; i <= num3; i = (short)(i + 1))
            {
                ProductionBuild build2 = BuildTables.Production[i];
                short num4 = (short)(build2.Handle().Count() - 1);
                for (short j = 0; j <= num4; j = (short)(j + 1))
                {
                    SymbolBuild build = build2.Handle()[j];
                    switch (build.Type)
                    {
                    case SymbolType.Nonterminal:
                    case SymbolType.Content:
                    case SymbolType.GroupEnd:
                    {
                        continue;
                    }
                    case SymbolType.Noise:
                    {
                        if (build.Reclassified)
                        {
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Noise terminal used in a production.", "The symbol '" + build.Name + "' is was changed by 'Noise' by the system. This is done for the terminal's named 'Whitespace' and 'Comment'. It was used in the production " + build2.Text(), "");
                        }
                        else
                        {
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Noise terminal used in a production.", "The symbol '" + build.Name + "' is declared as Noise. This means it is ignored by the parser. It was used in the production " + build2.Text(), "");
                        }
                        continue;
                    }
                    case SymbolType.GroupStart:
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Cannot use a group start symbol in a production.", "The symbol '" + build.Name + "' is the start of a lexical group. The lexer will use this symbol, and the matching end, to create a single container token. It was used in the production " + build2.Text(), "");
                        continue;
                    }
                    }
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Illegal symbol", "The symbol '" + build.Name + "' is not allowed  in the production " + build2.Text(), "");
                }
            }
        }

        private static void CheckRuleRecursion()
        {
            int num5 = BuildTables.Production.Count() - 1;
            for (int i = 0; i <= num5; i++)
            {
                short num;
                short tableIndex;
                ProductionBuild build = BuildTables.Production[i];
                if (build.Handle().Count() == 1)
                {
                    tableIndex = build.Head.TableIndex;
                    num = build.Handle()[0].TableIndex;
                    if (tableIndex == num)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "The rule " + build.Name() + " is defined as itself", "This rule is defined using the form <A> ::= <A>. This will eventually lead to a shift-reduce error.", "");
                    }
                }
                else if (build.Handle().Count() >= 2)
                {
                    tableIndex = build.Head.TableIndex;
                    num = build.Handle()[0].TableIndex;
                    short num3 = build.Handle()[build.Handle().Count() - 1].TableIndex;
                    if ((tableIndex == num) & (tableIndex == num3))
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "The rule " + build.Name() + " is both left and right recursive.", "This rule is defined using the form <A> ::= <A> .. <A>. This will eventually lead to a shift-reduce error.", "");
                    }
                }
            }
        }

        private static void CheckUnusedSymbols()
        {
            short num;
            Symbol symbol;
            bool[] flagArray = new bool[(BuildTables.Symbol.Count() - 1) + 1];
            short num2 = (short)(BuildTables.Symbol.Count() - 1);
            for (num = 0; num <= num2; num = (short)(num + 1))
            {
                flagArray[num] = false;
            }
            CheckIfUsed(flagArray, BuildTables.StartSymbol.TableIndex);
            short num3 = (short)(BuildTables.Symbol.Count() - 1);
            for (num = 0; num <= num3; num = (short)(num + 1))
            {
                symbol = BuildTables.Symbol[num];
                if (!flagArray[num] & (symbol.Type == SymbolType.Nonterminal))
                {
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Unreachable rule: <" + symbol.Name + ">", "The rule <" + symbol.Name + "> cannot be reached from the \"Start Symbol\".", "");
                }
            }
            if (!LoggedCriticalError())
            {
                short num4 = (short)(BuildTables.Symbol.Count() - 1);
                for (num = 0; num <= num4; num = (short)(num + 1))
                {
                    symbol = BuildTables.Symbol[num];
                    if (!flagArray[num] & (symbol.Type == SymbolType.Content))
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Unused terminal: " + symbol.Name, "The terminal " + symbol.Name + " is defined but not used.", "");
                    }
                }
            }
        }

        private static void Clear()
        {
            UserDefinedSets.Clear();
            Log.Clear();
            Grammar.Clear();
            SaveCGTWarnings.Clear();
            BuildTables.Clear();
        }

        public static void ComputeComplete()
        {
            if (!BuildTables.Properties.Contains("Generated By"))
            {
                BuildTables.Properties.Add("Generated By", AppNameVersionFull());
            }
            if (!BuildTables.Properties.Contains("Generated Date"))
            {
                BuildTables.Properties.Add("Generated Date", Strings.Format(DateAndTime.Now, "yyyy-MM-dd HH:mm"));
            }
            PopulateSaveCGTWarningTable();
        }

        public static void ComputeDFA()
        {
            if (!LoggedCriticalError())
            {
                CancelTableBuild = false;
                DateTime now = DateAndTime.Now;
                BuildDFA.Build();
                DateTime endTime = DateAndTime.Now;
                Log.Add(SysLogSection.DFA, SysLogAlert.Detail, "DFA computation took: " + BuilderUtility.TimeDiffString(now, endTime));
            }
            CreateDFAPriorStateLists();
        }

        public static void ComputeLALR()
        {
            CancelTableBuild = false;
            DateTime now = DateAndTime.Now;
            BuildLR.Build();
            DateTime endTime = DateAndTime.Now;
            Log.Add(SysLogSection.LALR, SysLogAlert.Detail, "LALR computation took: " + BuilderUtility.TimeDiffString(now, endTime));
            if (!LoggedCriticalError())
            {
                Log.Add(SysLogSection.LALR, SysLogAlert.Success, "LALR Table was succesfully created", "The table contains a total of " + Conversions.ToString(BuildTables.LALR.Count) + " states", "");
                LogActionTotals();
            }
            CreateLRPriorStateLists();
        }

        private static RegExp CreateBasicRegExp(CharacterSetBuild CharSet, char Kleene)
        {
            RegExp exp2 = new RegExp();
            RegExpSeq seq = new RegExpSeq();
            RegExpItem item = new RegExpItem();
            SetItem item2 = new SetItem
            {
                Characters = CharSet,
                Type = SetItem.SetType.Chars
            };
            item.Data = item2;
            item.Kleene = Conversions.ToString(Kleene);
            seq.Add(item);
            exp2.Add(seq);
            return exp2;
        }

        private static RegExp CreateBasicRegExp(string Text, SetItem.SetType Type, char Kleene)
        {
            RegExp exp2 = new RegExp();
            RegExpSeq seq = new RegExpSeq();
            RegExpItem item = new RegExpItem();
            SetItem item2 = new SetItem
            {
                Text = Text,
                Type = Type
            };
            item.Data = item2;
            item.Kleene = Conversions.ToString(Kleene);
            seq.Add(item);
            exp2.Add(seq);
            return exp2;
        }

        public static void CreateDFAPriorStateLists()
        {
            int num2;
            FAStateBuild build;
            int num3 = BuildTables.DFA.Count - 1;
            for (num2 = 0; num2 <= num3; num2++)
            {
                build = BuildTables.DFA[num2];
                build.PriorStates.Clear();
            }
            int num4 = BuildTables.DFA.Count - 1;
            for (num2 = 0; num2 <= num4; num2++)
            {
                build = BuildTables.DFA[num2];
                int num5 = build.Edges().Count() - 1;
                for (int i = 0; i <= num5; i++)
                {
                    FAEdge edge = build.Edges()[i];
                    BuildTables.DFA[edge.Target].PriorStates.Add(new int[] { num2 });
                }
            }
        }

        private static void CreateImplicitDeclarations()
        {
            int num2 = BuildTables.Symbol.Count() - 1;
            for (int i = 0; i <= num2; i++)
            {
                SymbolBuild build = BuildTables.Symbol[i];
                if (((build.Category() == SymbolCategory.Terminal) & build.UsesDFA) & (build.RegularExp == null))
                {
                    build.RegularExp = CreateBasicRegExp(build.Name, SetItem.SetType.Sequence, '\0');
                    build.CreatedBy = CreatorType.Implicit;
                }
            }
        }

        public static void CreateLRPriorStateLists()
        {
            int num2;
            LRStateBuild build;
            int num3 = BuildTables.LALR.Count - 1;
            for (num2 = 0; num2 <= num3; num2++)
            {
                build = BuildTables.LALR[num2];
                build.PriorStates.Clear();
            }
            int num4 = BuildTables.LALR.Count - 1;
            for (num2 = 0; num2 <= num4; num2++)
            {
                build = BuildTables.LALR[num2];
                int num5 = build.Count - 1;
                for (int i = 0; i <= num5; i++)
                {
                    LRAction action = build[(short)i];
                    switch (((int)action.Type()))
                    {
                    case 1:
                    case 3:
                        BuildTables.LALR[action.Value()].PriorStates.Add(new int[] { num2 });
                        break;
                    }
                }
            }
        }

        private static void DoSemanticAnalysis()
        {
            if (!LoggedCriticalError())
            {
                CheckIllegalSymbols();
            }
            if (!LoggedCriticalError())
            {
                CheckUnusedSymbols();
            }
            if (!LoggedCriticalError())
            {
                CheckRuleRecursion();
            }
            if (!LoggedCriticalError())
            {
                CheckDuplicateGroupStart();
            }
        }

        public static void EnterGrammar(TextReader MetaGrammar)
        {
            Restart();
            Mode = ProgramMode.Input;
            Notify.Started("Analyzing Grammar");
            Grammar.Build(MetaGrammar);
            Populate();
            AssignTableIndexes();
            LinkGroupSymbolsToGroup();
            AssignPriorities();
            string str = Strings.UCase(BuildTables.Properties["Case Sensitive"].Value);
            if ((((str == "TRUE") || (str == "FALSE")) ? 1 : 0) == 0)
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid Property Value", "The \"Case Sensitive\" parameter must be either True or False.", "");
            }
            if (GetParamCharMapping() == CharMappingMode.Invalid)
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid Property Value", "The \"Character Mapping\" parameter must be either Unicode, Windows-1252, or ANSI (same as Windows-1252).", "");
            }
            int num = 0;
            int num3 = BuildTables.Symbol.Count() - 1;
            for (int i = 0; i <= num3; i++)
            {
                if (BuildTables.Symbol[i].Type == SymbolType.Content)
                {
                    num++;
                }
            }
            Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, "The grammar contains a total of " + Conversions.ToString(num) + " formal terminals.");
            if (!LoggedCriticalError())
            {
                DoSemanticAnalysis();
            }
            if (!LoggedCriticalError())
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Success, "The grammar was successfully analyzed");
            }
            Notify.Completed("Grammar Completed");
            Mode = ProgramMode.Idle;
        }

        internal static CharacterSetBuild EvaluateSetExp(ISetExpression Exp)
        {
            CharacterSetBuild build3 = null;
            if (Exp is SetItem)
            {
                SetItem item = (SetItem)Exp;
                switch (item.Type)
                {
                case SetItem.SetType.Chars:
                    return (CharacterSetBuild)item.Characters;

                case SetItem.SetType.Name:
                {
                    CharacterSetBuild characterSet = (CharacterSetBuild)GetCharacterSet(item.Text);
                    if (characterSet != null)
                    {
                        return new CharacterSetBuild(characterSet);
                    }
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Character set is not defined", "The character set {" + item.Text + "} was not defined in the grammar.", "");
                    return new CharacterSetBuild();
                }
                }
                return build3;
            }
            if (!(Exp is SetExp))
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Internal Error: Invalid object in set expression.", "Embedded object: " + Versioned.TypeName(Exp), "");
                return new CharacterSetBuild();
            }
            SetExp exp = (SetExp)Exp;
            CharacterSetBuild build4 = EvaluateSetExp(exp.LeftOperand);
            CharacterSetBuild setB = EvaluateSetExp(exp.RightOperand);
            switch (((char)(exp.Operator - '+')))
            {
            case '\0':
                build4.UnionWith(setB);
                return build4;

            case '\x0002':
                build4.Remove(setB);
                return build4;
            }
            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Internal Error: Invalid set operator.", "Operator: '" + Conversions.ToString(exp.Operator) + "'", "");
            return (CharacterSetBuild)new CharacterSet();
        }

        public static string GetAdvanceName(AdvanceMode Advance)
        {
            switch (((int)Advance))
            {
            case 0:
                return "Token";

            case 1:
                return "Character";
            }
            return "Invalid";
        }

        public static CharacterSet GetCharacterSet(string Name)
        {
            CharacterSet set2 = null;
            int num = UserDefinedSets.ItemIndex(Name);
            if (num != -1)
            {
                return UserDefinedSets[num];
            }
            num = PredefinedSets.ItemIndex(Name);
            if (num != -1)
            {
                set2 = PredefinedSets[num];
            }
            return set2;
        }

        public static string GetEndingName(EndingMode Ending)
        {
            switch (((int)Ending))
            {
            case 0:
                return "Open";

            case 1:
                return "Closed";
            }
            return "Invalid";
        }

        public static string GetNestingListText(GroupBuild G)
        {
            string name = "";
            IntegerList nesting = G.Nesting;
            if (nesting.Count >= 1)
            {
                name = BuildTables.Group[nesting[0]].Name;
                int num2 = nesting.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    name = name + ", " + BuildTables.Group[nesting[i]].Name;
                }
            }
            nesting = null;
            return name;
        }

        public static CharMappingMode GetParamCharMapping()
        {
            switch (Strings.UCase(BuildTables.Properties["Character Mapping"].Value))
            {
            case "WINDOWS-1252":
            case "ANSI":
                return CharMappingMode.Windows1252;

            case "NONE":
                return CharMappingMode.None;
            }
            return CharMappingMode.Invalid;
        }

        public static bool IsPredefinedSet(string Name)
        {
            return (PredefinedSets.ItemIndex(Name) != -1);
        }

        public static bool IsUserDefinedSet(string Name)
        {
            return (UserDefinedSets.ItemIndex(Name) != -1);
        }

        private static void LinkGroupSymbolsToGroup()
        {
            int num2 = BuildTables.Group.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                GroupBuild build = BuildTables.Group[i];
                build.Container.Group = build;
                build.Start.Group = build;
                build.End.Group = build;
            }
        }

        private static void LoadPredefinedSets()
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            try
            {
                reader.Open(FileUtility.AppPath() + @"\sets.dat");
                if (reader.Header() != "GOLD Character Sets")
                {
                    FatalLoadMessage = "The file 'sets.dat' is invalid";
                    FatalLoadError = true;
                }
                else
                {
                    while (!reader.EndOfFile())
                    {
                        reader.GetNextRecord();
                        string str2 = reader.RetrieveString();
                        string str3 = reader.RetrieveString();
                        string str = reader.RetrieveString();
                        int num = reader.RetrieveInt16();
                        DefinedCharacterSet item = new DefinedCharacterSet
                        {
                            Name = str2,
                            Type = str3,
                            Comment = str
                        };
                        while (!reader.RecordComplete())
                        {
                            item.AddRange(reader.RetrieveInt16(), reader.RetrieveInt16());
                        }
                        PredefinedSets.Add(item);
                    }
                    reader.Close();
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                Information.Err().Clear();
                FatalLoadMessage = exception.Message;
                FatalLoadError = true;
                ProjectData.ClearProjectError();
            }
        }

        public static bool LoadTables(string Path)
        {
            bool flag;
            BuildTables.Load(Path);
            CreateDFAPriorStateLists();
            CreateLRPriorStateLists();
            return flag;
        }

        public static void LogActionTotals()
        {
            int num;
            int num2;
            int num5;
            int num6;
            short[] numArray = new short[(BuildTables.Symbol.Count() - 1) + 1];
            int num7 = BuildTables.LALR.Count - 1;
            for (int i = 0; i <= num7; i++)
            {
                LRStateBuild build = BuildTables.LALR[i];
                short num8 = (short)(build.Count - 1);
                for (short j = 0; j <= num8; j = (short)(j + 1))
                {
                    LRAction action = build[j];
                    switch (((int)action.Type()))
                    {
                    case 1:
                        num6++;
                        break;

                    case 2:
                    {
                        num5++;
                        short[] numArray2 = numArray;
                        int index = action.SymbolIndex();
                        numArray2[index] = (short)(numArray2[index] + 1);
                        break;
                    }
                    case 3:
                        num2++;
                        break;

                    case 4:
                        num++;
                        break;
                    }
                }
            }
            string title = "Total actions: " + Conversions.ToString(num6) + " Shifts, " + Conversions.ToString(num5) + " Reduces, " + Conversions.ToString(num2) + " Gotos, " + Conversions.ToString(num) + " Accepts.";
            Log.Add(SysLogSection.LALR, SysLogAlert.Detail, title);
        }

        public static bool LoggedCriticalError()
        {
            return (Log.AlertCount(SysLogAlert.Critical) != 0);
        }

        public static bool LoggedWarning()
        {
            return (Log.AlertCount(SysLogAlert.Warning) != 0);
        }

        public static void Populate()
        {
            BuildTables.Clear();
            BuildTables.Symbol.AddUnique(new SymbolBuild("EOF", SymbolType.End, false, CreatorType.Generated));
            BuildTables.Symbol.AddUnique(new SymbolBuild("Error", SymbolType.Error, false, CreatorType.Generated));
            PopulateProperties();
            PopulateSets();
            PopulateDefinedSymbols();
            PopulateGroupsAndWhitespace();
            PopulateProductionHeads();
            PopulateHandleSymbols();
            PopulateProductions();
            PopulateStartSymbol();
            ApplyGroupAttributes();
            ApplySymbolAttributes();
            ApplyVirtualProperty();
            CreateImplicitDeclarations();
        }

        private static void PopulateDefinedSymbols()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = Grammar.TerminalDefs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Grammar.GrammarSymbol current = (Grammar.GrammarSymbol)enumerator.Current;
                    SymbolBuild item = new SymbolBuild
                    {
                        Name = current.Name,
                        Type = current.Type,
                        UsesDFA = true,
                        CreatedBy = CreatorType.Defined,
                        RegularExp = current.Exp
                    };
                    BuildTables.Symbol.Add(item);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void PopulateGroupsAndWhitespace()
        {
            Grammar.GrammarGroup current;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            Grammar.GrammarGroupList list = new Grammar.GrammarGroupList();
            CharacterSetBuild charSet = new CharacterSetBuild();
            SymbolBuild build3 = new SymbolBuild();
            try
            {
                enumerator = Grammar.Groups.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (Grammar.GrammarGroup)enumerator.Current;
                    if (!current.IsBlock)
                    {
                        list.Add(current);
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            CharSetMode paramCharSet = Grammar.GetParamCharSet();
            if (paramCharSet == CharSetMode.ANSI)
            {
                charSet.Copy(PredefinedSets["Whitespace"]);
            }
            else
            {
                charSet.Copy(PredefinedSets["All Whitespace"]);
            }
            if (list.Count >= 1)
            {
                RegExp exp = new RegExp();
                int num2 = BuildTables.Symbol.ItemIndex("NewLine");
                if (num2 == -1)
                {
                    charSet.Remove(new int[] { 10, 13, 0x2028, 0x2029 });
                    exp.AddTextExp("{LF}|{CR}{LF}?|{LS}|{PS}");
                    build3 = BuildTables.Symbol.AddUnique(new SymbolBuild("NewLine", SymbolType.Noise, exp));
                    build3.CreatedBy = CreatorType.Generated;
                }
                else
                {
                    build3 = BuildTables.Symbol[num2];
                }
            }
            try
            {
                enumerator2 = Grammar.Groups.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SymbolBuild build2;
                    EndingMode closed;
                    current = (Grammar.GrammarGroup)enumerator2.Current;
                    SymbolBuild container = BuildTables.Symbol.AddUnique(new SymbolBuild(current.Container, SymbolType.Content, false, CreatorType.Generated));
                    SymbolBuild start = BuildTables.Symbol.AddUnique(new SymbolBuild(current.Start, SymbolType.GroupStart, true, CreatorType.Generated));
                    start.Type = SymbolType.GroupStart;
                    if (current.IsBlock)
                    {
                        closed = EndingMode.Closed;
                        build2 = BuildTables.Symbol.AddUnique(new SymbolBuild(current.End, SymbolType.GroupEnd, true, CreatorType.Generated));
                    }
                    else
                    {
                        closed = EndingMode.Open;
                        build2 = build3;
                    }
                    BuildTables.Group.AddUnique(new GroupBuild(current.Name, container, start, build2, closed));
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            if ((BuildTables.Symbol.ItemIndex("Whitespace") == -1) & (Strings.UCase(BuildTables.Properties["Auto Whitespace"].Value) == "TRUE"))
            {
                RegExp exp2 = CreateBasicRegExp(charSet, '+');
                SymbolBuild item = new SymbolBuild();
                item = new SymbolBuild("Whitespace", SymbolType.Noise, exp2)
                {
                    CreatedBy = CreatorType.Generated
                };
                BuildTables.Symbol.Add(item);
                Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, "Whitespace was implicitly defined", "The special terminal 'Whitespace' was implicitly defined as: {" + BuilderUtility.DisplayText(charSet, true, 0x400, "", -1) + "}+", "");
            }
            string name = "Whitespace";
            SymbolType noise = SymbolType.Noise;
            Reclassify(name, noise);
            name = "Comment";
            noise = SymbolType.Noise;
            Reclassify(name, noise);
        }

        private static void PopulateHandleSymbols()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = Grammar.HandleSymbols.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Grammar.GrammarSymbol current = (Grammar.GrammarSymbol)enumerator.Current;
                    if (current.Type == SymbolType.Nonterminal)
                    {
                        if (BuildTables.Symbol.ItemIndexCategory(current.Name, SymbolCategory.Nonterminal) == -1)
                        {
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined rule: <" + current.Name + ">", "", Conversions.ToString(current.Line));
                        }
                    }
                    else if (BuildTables.Symbol.ItemIndexCategory(current.Name, SymbolCategory.Terminal) == -1)
                    {
                        SymbolBuild item = new SymbolBuild(current.Name, SymbolType.Content, true, CreatorType.Implicit);
                        BuildTables.Symbol.Add(item);
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, item.Name + " was implicitly defined", "The terminal was implicitly declared as " + item.Text(false), "");
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void PopulateProductionHeads()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = Grammar.Productions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Grammar.GrammarProduction current = (Grammar.GrammarProduction)enumerator.Current;
                    BuildTables.Symbol.AddUnique(new SymbolBuild(current.Head.Name, SymbolType.Nonterminal));
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void PopulateProductions()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = Grammar.Productions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IEnumerator enumerator2;
                    Grammar.GrammarProduction current = (Grammar.GrammarProduction)enumerator.Current;
                    ProductionBuild item = new ProductionBuild();
                    int num = BuildTables.Symbol.ItemIndex(current.Head.Name, SymbolType.Nonterminal);
                    item.Head = BuildTables.Symbol[num];
                    try
                    {
                        enumerator2 = current.Handle.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Grammar.GrammarSymbol symbol = (Grammar.GrammarSymbol)enumerator2.Current;
                            if (symbol.Type == SymbolType.Nonterminal)
                            {
                                num = BuildTables.Symbol.ItemIndexCategory(symbol.Name, SymbolCategory.Nonterminal);
                            }
                            else
                            {
                                num = BuildTables.Symbol.ItemIndexCategory(symbol.Name, SymbolCategory.Terminal);
                            }
                            item.Handle().Add(BuildTables.Symbol[num]);
                        }
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                    BuildTables.Production.Add(item);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void PopulateProperties()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = Grammar.Properties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Variable current = (Variable)enumerator.Current;
                    BuildTables.Properties.Add(current);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            if (!BuildTables.Properties.Contains("Name"))
            {
                BuildTables.Properties.Add("Name", "(Untitled)");
            }
            if (!BuildTables.Properties.Contains("Author"))
            {
                BuildTables.Properties.Add("Author", "(Unknown)");
            }
            if (!BuildTables.Properties.Contains("Version"))
            {
                BuildTables.Properties.Add("Version", "(Not Specified)");
            }
            if (!BuildTables.Properties.Contains("Auto Whitespace"))
            {
                BuildTables.Properties.Add("Auto Whitespace", "True");
            }
            if (!BuildTables.Properties.Contains("Case Sensitive"))
            {
                BuildTables.Properties.Add("Case Sensitive", "False");
            }
            if (!BuildTables.Properties.Contains("Character Mapping"))
            {
                BuildTables.Properties.Add("Character Mapping", "Windows-1252");
            }
            else if (BuildTables.Properties["Character Mapping"].Value.ToUpper().Contains("UNICODE"))
            {
                BuildTables.Properties["Character Mapping"].Value = "None";
            }
            BuildTables.Properties.Add("Character Set", "Unicode");
        }

        public static void PopulateSaveCGTWarningTable()
        {
            SaveCGTWarnings.Clear();
            try
            {
                GroupBuild build;
                int num;
                int num4 = BuildTables.Group.Count - 1;
                for (num = 0; num <= num4; num++)
                {
                    build = BuildTables.Group[num];
                    if ((build.Name.ToUpper() != "COMMENT BLOCK") & (build.Name.ToUpper() != "COMMENT LINE"))
                    {
                        SaveCGTWarnings.Add("The group '" + build.Name + "' will not be saved", "Version 1.0 only supports one group: Comment. The start/end symbols will be saved as regular terminals.");
                    }
                }
                num = BuildTables.Group.ItemIndex("COMMENT BLOCK");
                if (num != -1)
                {
                    build = BuildTables.Group[num];
                    if (build.Nesting.Count != 2)
                    {
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'all' nested block comments. When the file is saved, it will use this attribute.");
                    }
                    if (build.Advance != AdvanceMode.Token)
                    {
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'token' advancing in block comments. When the file is saved, it will use this attribute.");
                    }
                    if (build.Ending != EndingMode.Closed)
                    {
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'closed' block comments. When the file is saved, it will use this attribute.");
                    }
                }
                if (BuildTables.CharSet.Count > 0)
                {
                    long num3 = 0L;
                    long num2 = 0L;
                    int num5 = BuildTables.CharSet.Count - 1;
                    for (num = 0; num <= num5; num++)
                    {
                        num3 += (5 + (BuildTables.CharSet[num].Count() * 2)) + 2;
                        num2 += 12 + (6 * BuildTables.CharSet[num].RangeList().Count);
                    }
                    if ((((double)num3) / ((double)num2)) >= 2.0)
                    {
                        SaveCGTWarnings.Add(SysLogSection.Grammar, SysLogAlert.Info, "Version 1.0 will require " + Conversions.ToString(num3) + " bytes to store character set data. The new format will require " + Conversions.ToString(num2) + " bytes.");
                    }
                }
                bool flag = false;
                for (num = 0; (num < BuildTables.CharSet.Count) & !flag; num++)
                {
                    if (BuildTables.CharSet[num].Contains(0))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    SaveCGTWarnings.Add("Character &00 cannot be stored", "Due to how character sets are stored in Version 1.0, null characters (&00) will not stored.");
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                Information.Err().Clear();
                ProjectData.ClearProjectError();
            }
        }

        private static void PopulateSets()
        {
            bool flag;
            DefinedCharacterSet set2;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            try
            {
                enumerator = Grammar.UserSets.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Grammar.GrammarSet current = (Grammar.GrammarSet)enumerator.Current;
                    set2 = new DefinedCharacterSet(current.Name)
                    {
                        Exp = current.Exp
                    };
                    UserDefinedSets.Add(set2);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            bool flag2 = true;
            try
            {
                enumerator2 = Grammar.UsedSetNames.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Grammar.GrammarIdentifier identifier = (Grammar.GrammarIdentifier)enumerator2.Current;
                    if (UserDefinedSets.ItemIndex(identifier.Name) == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined Set", "The set {" + identifier.Name + "} is used but not defined in the grammar.", Conversions.ToString(identifier.Line));
                        flag2 = false;
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            if (flag2)
            {
                int num3 = UserDefinedSets.Count - 1;
                int number = 0;
                while (number <= num3)
                {
                    UserDefinedSets[number].Dependacy = UserDefinedSets[number].Exp.UsedDefinedSets();
                    number++;
                }
                flag = true;
                while (flag)
                {
                    IEnumerator enumerator3;
                    flag = false;
                    try
                    {
                        enumerator3 = UserDefinedSets.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            set2 = (DefinedCharacterSet)enumerator3.Current;
                            int num4 = set2.Dependacy.Count() - 1;
                            number = 0;
                            while (number <= num4)
                            {
                                int num = set2.Dependacy[number];
                                if (set2.Dependacy.UnionWith(UserDefinedSets[num].Dependacy))
                                {
                                    flag = true;
                                }
                                number++;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator3 is IDisposable)
                        {
                            (enumerator3 as IDisposable).Dispose();
                        }
                    }
                }
                int num5 = UserDefinedSets.Count - 1;
                for (number = 0; number <= num5; number++)
                {
                    if (UserDefinedSets[number].Dependacy.Contains(number))
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Circular Set Definition", "The set {" + UserDefinedSets[number].Name + "} is defined using itself. This can be either direct or through other sets.", "");
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                flag = true;
                while (flag)
                {
                    IEnumerator enumerator4;
                    flag = false;
                    try
                    {
                        enumerator4 = UserDefinedSets.GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            set2 = (DefinedCharacterSet)enumerator4.Current;
                            CharacterSetBuild setB = EvaluateSetExp(set2.Exp);
                            if (!set2.IsEqualSet(setB))
                            {
                                set2.Copy(setB);
                                flag = true;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator4 is IDisposable)
                        {
                            (enumerator4 as IDisposable).Dispose();
                        }
                    }
                }
            }
        }

        private static void PopulateStartSymbol()
        {
            string name = Strings.Trim(BuildTables.Properties["Start Symbol"].Value);
            if (name.StartsWith("<") & name.EndsWith(">"))
            {
                name = name.Substring(1, name.Length - 2);
            }
            if (name == "")
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "No specified start symbol", "Please assign the \"Start Symbol\" parameter to the start symbol's name", "");
            }
            else
            {
                int num = BuildTables.Symbol.NonterminalIndex(name);
                if (num == -1)
                {
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid start symbol", "Please assign the \"Start Symbol\" parameter to the start symbol's name", "");
                }
                else
                {
                    BuildTables.StartSymbol = BuildTables.Symbol[num];
                }
            }
        }

        private static short Reclassify(string Name, SymbolType NewType)
        {
            short num = BuildTables.Symbol.TerminalIndex(Name);
            if (num != -1)
            {
                BuildTables.Symbol[num].Type = NewType;
                BuildTables.Symbol[num].Reclassified = true;
            }
            return num;
        }

        public static void Restart()
        {
            Clear();
        }

        public static bool RuleTypeExists(SymbolBuild NonTerminal)
        {
            bool flag = false;
            for (short i = 0; !flag & (i < BuildTables.Production.Count()); i = (short)(i + 1))
            {
                if (BuildTables.Production[i].Head.IsEqualTo(NonTerminal))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static bool SaveLog(string FilePath)
        {
            bool flag;
            try
            {
                TextWriter writer = new StreamWriter(FilePath, false, Encoding.UTF8);
                int num2 = Log.Count() - 1;
                for (int i = 0; i <= num2; i++)
                {
                    SysLogItem item = Log[i];
                    string str = item.SectionName().PadRight(15) + item.AlertName().PadRight(10);
                    if (item.Index == null)
                    {
                        str = str + Strings.Space(8);
                    }
                    else
                    {
                        str = str + item.Index.ToString().PadRight(8);
                    }
                    str = str + item.Title + " : " + item.Description;
                    writer.WriteLine(str);
                    item = null;
                }
                writer.Close();
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = false;
            }
            return true;
        }

        public static void Setup()
        {
            LoadPredefinedSets();
            GrammarParse.Setup();
            UnicodeTable.Setup();
        }

        public static string SymbolTypeName(SymbolType Type)
        {
            switch (((int)Type))
            {
            case 0:
                return "Nonterminal";

            case 1:
                return "Content";

            case 2:
                return "Noise";

            case 3:
                return "End of File";

            case 4:
                return "Lexical Group Start";

            case 5:
                return "Lexical Group End";

            case 6:
                return "Comment Line (LEGACY)";

            case 7:
                return "Runtime Error Symbol";
            }
            return "";
        }

        // Nested Types
        public enum CharMappingMode
        {
            Invalid = -1,
            None = 1,
            Windows1252 = 0
        }

        public enum CharSetMode
        {
            ANSI = 1,
            Invalid = -1,
            Unicode = 0
        }

        public enum ProgramMode
        {
            Startup,
            Idle,
            Input,
            NFACase,
            NFAClosure,
            BuildingNFA,
            BuildingDFA,
            BuildingFirstSets,
            BuildingLALRClosure,
            BuildingLALR
        }
    }
}