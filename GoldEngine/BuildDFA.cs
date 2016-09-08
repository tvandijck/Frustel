using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GoldEngine
{
    internal sealed class BuildDFA
    {
        // Fields
        public static FAStateBuildList NFA = new FAStateBuildList();
        public static short Root;
        public static short StartState;

        // Methods
        private static short AddDFAState(FAStateBuild State)
        {
            State.TableIndex = (short)BuilderApp.BuildTables.DFA.Add(State);
            return State.TableIndex;
        }

        private static int AddState()
        {
            FAStateBuild item = new FAStateBuild();
            return NFA.Add(item);
        }

        public static void Build()
        {
            short num3;
            NumberSet nFAList = new NumberSet(new int[0]);
            NumberSet set2 = new NumberSet(new int[0]);
            Notify.Started("Computing DFA States");
            SetupForNFA();
            BuilderApp.Mode = BuilderApp.ProgramMode.BuildingNFA;
            short num5 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
            for (num3 = 0; num3 <= num5; num3 = (short)(num3 + 1))
            {
                SymbolBuild sym = BuilderApp.BuildTables.Symbol[num3];
                if (sym.UsesDFA)
                {
                    Notify.Text = sym.Name;
                    CreateNFAStates(sym);
                }
            }
            BuilderApp.Mode = BuilderApp.ProgramMode.NFAClosure;
            short num6 = (short)(NFA.Count - 1);
            for (num3 = 0; num3 <= num6; num3 = (short)(num3 + 1))
            {
                NumberSet reachable = new NumberSet(new int[0]);
                reachable.Add(new int[] { num3 });
                ClosureNFA(reachable);
                NFA[num3].NFAClosure = reachable;
            }
            BuilderApp.BuildTables.CharSet.Clear();
            if (NFA.Count <= 1)
            {
                BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Critical, "There are no terminals in the grammar");
            }
            else
            {
                short num2;
                BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Detail, "The initial Nondeterministic Finite Automata has " + Conversions.ToString(NFA.Count) + " states");
                BuilderApp.BuildTables.DFA.Clear();
                BuilderApp.Mode = BuilderApp.ProgramMode.NFACase;
                SetupMapCaseCharTables(BuilderApp.BuildTables.Properties["Case Sensitive"].Value.ToUpper() == "TRUE", BuilderApp.GetParamCharMapping());
                BuilderApp.Mode = BuilderApp.ProgramMode.BuildingDFA;
                nFAList.Add(new int[] { Root });
                StartState = BuildDFAState(nFAList);
                short num7 = (short)(BuilderApp.BuildTables.DFA.Count - 1);
                for (num3 = 0; num3 <= num7; num3 = (short)(num3 + 1))
                {
                    short num8 = (short)(BuilderApp.BuildTables.DFA[num3].Edges().Count() - 1);
                    num2 = 0;
                    while (num2 <= num8)
                    {
                        FAEdgeBuild build3 = BuilderApp.BuildTables.DFA[num3].Edges()[num2];
                        CharacterSetBuild characters = build3.Characters;
                        build3.Characters = characters;
                        short num4 = BuilderApp.AddCharacterSet(characters);
                        BuilderApp.BuildTables.DFA[num3].Edges()[num2].Characters = BuilderApp.BuildTables.CharSet[num4];
                        num2 = (short)(num2 + 1);
                    }
                }
                short num9 = (short)(BuilderApp.BuildTables.DFA.Count - 1);
                for (num3 = 0; num3 <= num9; num3 = (short)(num3 + 1))
                {
                    FAStateBuild build = BuilderApp.BuildTables.DFA[num3];
                    if (build.AcceptList.Count() == 0)
                    {
                        build.Accept = null;
                    }
                    else if (build.AcceptList.Count() == 1)
                    {
                        build.Accept = BuilderApp.BuildTables.Symbol[build.AcceptList[0].SymbolIndex];
                    }
                    else
                    {
                        FAAccept accept = build.AcceptList[0];
                        set2.Clear();
                        set2.Add(new int[] { accept.SymbolIndex });
                        short priority = accept.Priority;
                        short num10 = (short)(build.AcceptList.Count() - 1);
                        num2 = 1;
                        while (num2 <= num10)
                        {
                            accept = build.AcceptList[num2];
                            if (accept.Priority == priority)
                            {
                                set2.Add(new int[] { accept.SymbolIndex });
                            }
                            else if (accept.Priority < priority)
                            {
                                set2.Clear();
                                set2.Add(new int[] { accept.SymbolIndex });
                                priority = accept.Priority;
                            }
                            num2 = (short)(num2 + 1);
                        }
                        build.AcceptList.Clear();
                        short num11 = (short)(set2.Count() - 1);
                        for (num2 = 0; num2 <= num11; num2 = (short)(num2 + 1))
                        {
                            build.AcceptList.Add((short)set2[num2], priority);
                        }
                        if (set2.Count() == 1)
                        {
                            build.Accept = BuilderApp.BuildTables.Symbol[set2[0]];
                        }
                    }
                }
                CheckErrorsDFA();
                Notify.Completed("DFA States Completed");
            }
        }

        private static short BuildDFAState(NumberSet NFAList)
        {
            int num4;
            NumberSet nFAList = new NumberSet(new int[0]);
            FAStateBuild state = new FAStateBuild();
            int num8 = NFAList.Count() - 1;
            for (num4 = 0; num4 <= num8; num4++)
            {
                state.NFAStates.UnionWith(NFA[NFAList[num4]].NFAClosure);
            }
            short num6 = DFAStateNumber(state);
            if (num6 == -1)
            {
                int num3;
                FAStateBuild build4;
                Notify.Counter++;
                num6 = AddDFAState(state);
                int num9 = state.NFAStates.Count() - 1;
                for (num4 = 0; num4 <= num9; num4++)
                {
                    build4 = NFA[state.NFAStates[num4]];
                    int num10 = build4.AcceptList.Count() - 1;
                    num3 = 0;
                    while (num3 <= num10)
                    {
                        FAAccept item = build4.AcceptList[num3];
                        state.AcceptList.Add(item);
                        num3++;
                    }
                }
                NumberSet set2 = new NumberSet(new int[0]);
                FAEdgeList list = new FAEdgeList();
                int num11 = state.NFAStates.Count() - 1;
                for (num4 = 0; num4 <= num11; num4++)
                {
                    build4 = NFA[state.NFAStates[num4]];
                    int num12 = build4.Edges().Count() - 1;
                    num3 = 0;
                    while (num3 <= num12)
                    {
                        if (build4.Edges()[num3].Characters.Count() >= 1)
                        {
                            list.Add(build4.Edges()[num3]);
                            set2.Add(new int[] { build4.Edges()[num3].Target });
                        }
                        num3++;
                    }
                }
                if (set2.Count() >= 1)
                {
                    CharacterSetBuild build;
                    FAEdgeBuild build3;
                    CharacterSetBuild[] buildArray = new CharacterSetBuild[(set2.Count() - 1) + 1];
                    int num13 = set2.Count() - 1;
                    for (num4 = 0; num4 <= num13; num4++)
                    {
                        Notify.Analyzed++;
                        build = new CharacterSetBuild();
                        short num7 = (short)set2[num4];
                        int num14 = list.Count() - 1;
                        num3 = 0;
                        while (num3 <= num14)
                        {
                            build3 = (FAEdgeBuild)list[num3];
                            if (build3.Target == num7)
                            {
                                build.UnionWith(build3.Characters);
                            }
                            num3++;
                        }
                        int num15 = list.Count() - 1;
                        for (num3 = 0; num3 <= num15; num3++)
                        {
                            build3 = (FAEdgeBuild)list[num3];
                            if (build3.Target != num7)
                            {
                                build.DifferenceWith(build3.Characters);
                            }
                        }
                        buildArray[num4] = build;
                    }
                    CharacterSetBuild build5 = new CharacterSetBuild();
                    int num16 = list.Count() - 1;
                    num3 = 0;
                    while (num3 <= num16)
                    {
                        build5.UnionWith(list[num3].Characters);
                        num3++;
                    }
                    int num17 = set2.Count() - 1;
                    for (num4 = 0; num4 <= num17; num4++)
                    {
                        build5.DifferenceWith(buildArray[num4]);
                    }
                    int num18 = set2.Count() - 1;
                    for (num4 = 0; num4 <= num18; num4++)
                    {
                        if (buildArray[num4].Count() >= 1)
                        {
                            nFAList = new NumberSet(new int[0]);
                            nFAList.Add(new int[] { set2[num4] });
                            Notify.Analyzed++;
                            state.AddEdge(buildArray[num4], BuildDFAState(nFAList));
                        }
                    }
                    int num19 = build5.Count() - 1;
                    for (num4 = 0; num4 <= num19; num4++)
                    {
                        nFAList = new NumberSet(new int[0]);
                        int number = build5[num4];
                        int num20 = list.Count() - 1;
                        for (num3 = 0; num3 <= num20; num3++)
                        {
                            build3 = (FAEdgeBuild)list[num3];
                            if (build3.Characters.Contains(number))
                            {
                                nFAList.Add(new int[] { build3.Target });
                            }
                        }
                        if (nFAList.Count() >= 1)
                        {
                            build = new CharacterSetBuild();
                            build.Add(new int[] { number });
                            Notify.Analyzed++;
                            state.AddEdge(build, BuildDFAState(nFAList));
                        }
                    }
                }
                return num6;
            }
            return num6;
        }

        private static void CheckErrorsDFA()
        {
            short num2;
            FAStateBuild build;
            bool flag = false;
            if (!flag && (BuilderApp.BuildTables.DFA[StartState].AcceptList.Count() > 0))
            {
                short num3 = (short)(BuilderApp.BuildTables.DFA[StartState].AcceptList.Count() - 1);
                for (num2 = 0; num2 <= num3; num2 = (short)(num2 + 1))
                {
                    BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Critical, "The terminal '" + BuilderApp.BuildTables.Symbol[BuilderApp.BuildTables.DFA[StartState].AcceptList[num2].SymbolIndex].Name + "' can be zero length", "The definition of this terminal allows it to contain no characters.", "");
                }
                flag = true;
            }
            if (!flag)
            {
                short num4 = (short)(BuilderApp.BuildTables.DFA.Count - 1);
                for (num2 = 0; num2 <= num4; num2 = (short)(num2 + 1))
                {
                    build = BuilderApp.BuildTables.DFA[num2];
                    if (build.AcceptList.Count() >= 2)
                    {
                        string title = "DFA State " + Conversions.ToString((int)num2) + ": Cannot distinguish between: ";
                        short num5 = (short)(build.AcceptList.Count() - 1);
                        for (short i = 0; i <= num5; i = (short)(i + 1))
                        {
                            title = title + BuilderApp.BuildTables.Symbol[build.AcceptList[i].SymbolIndex].Name + " ";
                        }
                        BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Critical, title, ConflictDesc(), Conversions.ToString((int)num2));
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                short num6 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
                for (num2 = 0; num2 <= num6; num2 = (short)(num2 + 1))
                {
                    BuilderApp.BuildTables.Symbol[num2].Accepted = false;
                }
                short num7 = (short)(BuilderApp.BuildTables.DFA.Count - 1);
                for (num2 = 0; num2 <= num7; num2 = (short)(num2 + 1))
                {
                    build = BuilderApp.BuildTables.DFA[num2];
                    if (build.AcceptList.Count() != 0)
                    {
                        BuilderApp.BuildTables.Symbol[build.AcceptList[0].SymbolIndex].Accepted = true;
                    }
                }
                short num8 = (short)(BuilderApp.BuildTables.Symbol.Count() - 1);
                for (num2 = 0; num2 <= num8; num2 = (short)(num2 + 1))
                {
                    SymbolBuild build2 = BuilderApp.BuildTables.Symbol[num2];
                    if (!build2.Accepted & build2.UsesDFA)
                    {
                        BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Critical, "Unaccepted terminal: " + build2.Name, "The terminal " + build2.Name + " cannot be accepted by the DFA.", "");
                    }
                    build2 = null;
                }
            }
            if (!flag)
            {
                BuilderApp.Log.Add(SysLogSection.DFA, SysLogAlert.Success, "The DFA State Table was successfully created", "The table contains a total of " + Conversions.ToString(BuilderApp.BuildTables.DFA.Count) + " states", "");
            }
        }

        private static void ClosureNFA(NumberSet Reachable)
        {
            short num5 = (short)(Reachable.Count() - 1);
            for (short i = 0; i <= num5; i = (short)(i + 1))
            {
                short num = (short)Reachable[i];
                short num6 = (short)(NFA[num].Edges().Count() - 1);
                for (short j = 0; j <= num6; j = (short)(j + 1))
                {
                    int target = NFA[num].Edges()[j].Target;
                    if ((NFA[num].Edges()[j].Characters.Count() == 0) & !Reachable.Contains(target))
                    {
                        Reachable.Add(new int[] { target });
                        ClosureNFA(Reachable);
                    }
                }
            }
        }

        private static string ConflictDesc()
        {
            return "The conflict is caused when two or more terminal definitions can accept the same text.";
        }

        private static AutomataType CreateAutomataItem(RegExpItem Item)
        {
            AutomataType type;
            int head;
            int num3;
            int num4;
            int tail;
            if (Item.Data is RegExp)
            {
                RegExpItem item2 = Item;
                RegExp data = (RegExp)item2.Data;
                item2.Data = data;
                AutomataType type3 = CreateSubAutomata(data);
                head = type3.Head;
                tail = type3.Tail;
                num3 = head;
                num4 = tail;
            }
            else if (!(Item.Data is SetItem))
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid internal token at CreateAutomataItem(): " + Versioned.TypeName(RuntimeHelpers.GetObjectValue(Item.Data)));
            }
            else
            {
                CharacterSetBuild characters;
                SetItem item = (SetItem)Item.Data;
                switch (item.Type)
                {
                case SetItem.SetType.Chars:
                    characters = (CharacterSetBuild)item.Characters;
                    head = AddState();
                    tail = AddState();
                    NFA[head].AddEdge(characters, tail);
                    num3 = head;
                    num4 = tail;
                    break;

                case SetItem.SetType.Name:
                    characters = (CharacterSetBuild)BuilderApp.GetCharacterSet(item.Text);
                    if (characters == null)
                    {
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Unknown user-defined set: {" + item.Text + "}");
                        break;
                    }
                    tail = AddState();
                    head = AddState();
                    NFA[head].AddEdge(characters, tail);
                    num3 = head;
                    num4 = tail;
                    break;

                case SetItem.SetType.Sequence:
                {
                    int num6;
                    head = AddState();
                    tail = head;
                    string text = item.Text;
                    short num7 = (short)(text.Length - 1);
                    for (short i = 0; i <= num7; i = (short)(i + 1))
                    {
                        num6 = tail;
                        tail = AddState();
                        NFA[num6].AddEdge(new CharacterSetBuild(Conversions.ToString(text[i])), tail);
                    }
                    num3 = num6;
                    num4 = tail;
                    break;
                }
                default:
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid internal token at CreateAutomataItem(): " + Versioned.TypeName(RuntimeHelpers.GetObjectValue(Item.Data)));
                    break;
                }
            }
            if ((num3 == 0) | (num4 == 0))
            {
                Debug.WriteLine("ERROR: BAD KLEENE DATA");
            }
            switch (Item.Kleene)
            {
            case "*":
                NFA[num3].AddLambdaEdge(num4);
                NFA[num4].AddLambdaEdge(num3);
                break;

            case "+":
                NFA[num4].AddLambdaEdge(num3);
                break;

            case "?":
                NFA[num3].AddLambdaEdge(num4);
                break;
            }
            type.Head = (short)head;
            type.Tail = (short)tail;
            return type;
        }

        private static AutomataType CreateAutomataSeq(RegExpSeq Seq)
        {
            AutomataType type;
            short head;
            short tail;
            short num4 = (short)(Seq.Count() - 1);
            for (short i = 0; i <= num4; i = (short)(i + 1))
            {
                RegExpSeq seq = Seq;
                short num5 = i;
                RegExpItem item = seq[num5];
                seq[num5] = item;
                AutomataType type3 = CreateAutomataItem(item);
                if (i == 0)
                {
                    head = type3.Head;
                    tail = type3.Tail;
                }
                else
                {
                    NFA[tail].AddLambdaEdge(type3.Head);
                    tail = type3.Tail;
                }
            }
            type.Head = head;
            type.Tail = tail;
            return type;
        }

        public static void CreateNFAStates(SymbolBuild Sym)
        {
            short target = (short)AddState();
            short num3 = (short)AddState();
            short num4 = (short)(Sym.RegularExp.Count() - 1);
            for (short i = 0; i <= num4; i = (short)(i + 1))
            {
                RegExpSeq seq = Sym.RegularExp[i];
                AutomataType type = CreateAutomataSeq(seq);
                NFA[target].AddLambdaEdge(type.Head);
                NFA[type.Tail].AddLambdaEdge(num3);
                NFA[type.Tail].AcceptList.Add(Sym.TableIndex, seq.Priority);
            }
            NFA[Root].AddLambdaEdge(target);
        }

        private static AutomataType CreateSubAutomata(RegExp Exp)
        {
            AutomataType type3;
            short num = (short)AddState();
            short target = (short)AddState();
            short num4 = (short)(Exp.Count() - 1);
            for (short i = 0; i <= num4; i = (short)(i + 1))
            {
                RegExpSeq seq = Exp[i];
                AutomataType type = CreateAutomataSeq(seq);
                NFA[num].AddLambdaEdge(type.Head);
                NFA[type.Tail].AddLambdaEdge(target);
            }
            type3.Head = num;
            type3.Tail = target;
            return type3;
        }

        private static short DFAStateNumber(FAStateBuild State)
        {
            short num2 = 0;
            short num4 = 0;
            short num3 = -1;
            while ((((short)-((num4 < BuilderApp.BuildTables.DFA.Count) > false)) & ~num2) > 0)
            {
                if (BuilderApp.BuildTables.DFA[num4].NFAStates.IsEqualSet(State.NFAStates))
                {
                    num2 = -1;
                    num3 = num4;
                }
                num4 = (short)(num4 + 1);
            }
            return num3;
        }

        public static void SetupForNFA()
        {
            NFA.Clear();
            Root = (short)AddState();
        }

        private static void SetupMapCaseCharTables(bool CaseSensitive, BuilderApp.CharMappingMode Mapping)
        {
            short num;
            if (!CaseSensitive)
            {
                short num2 = (short)(NFA.Count - 1);
                for (num = 0; num <= num2; num = (short)(num + 1))
                {
                    NFA[num].CaseClosure();
                }
            }
            if (Mapping == BuilderApp.CharMappingMode.Windows1252)
            {
                short num3 = (short)(NFA.Count - 1);
                for (num = 0; num <= num3; num = (short)(num + 1))
                {
                    NFA[num].MappingClosure(Mapping);
                }
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct AutomataType
        {
            public short Head;
            public short Tail;
        }
    }
}