using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Frutsel
{
    public class BuildDFA : IDisposable
    {
        private struct NfaAccept
        {
            public int Priority;
            public Symbol Symbol;
        }

        private class NfaState
        {
            private readonly List<NfaAccept> m_acceptList = new List<NfaAccept>();
            private readonly List<Edge> m_edges = new List<Edge>();
            private HashSet<int> m_nfaClosure;
            private readonly HashSet<int> m_nfaStates = new HashSet<int>();
            private int m_tableIndex;

            public List<NfaAccept> AcceptList
            {
                get { return m_acceptList; }
            }

            public List<Edge> Edges
            {
                get { return m_edges; }
            }

            public HashSet<int> NFAClosure
            {
                get { return m_nfaClosure; }
                set { m_nfaClosure = value; }
            }

            public HashSet<int> NFAStates
            {
                get { return m_nfaStates; }
            }

            public int TableIndex
            {
                get { return m_tableIndex; }
                set { m_tableIndex = value; }
            }

            public void AddLambdaEdge(int target)
            {
                AddEdge(target, null);
            }

            public void AddEdge(int target, CharacterSet characterSet)
            {
                if (characterSet != null && characterSet.Count > 0)
                {
                    foreach (var item in m_edges)
                    {
                        if (item.Target == target)
                        {
                            item.Characters.Add(characterSet);
                            return;
                        }
                    }
                }

                m_edges.Add(new Edge(target, characterSet));
            }

            public void AddAccept(Symbol symbol, int priority)
            {
                m_acceptList.Add(new NfaAccept
                {
                    Priority = priority,
                    Symbol = symbol
                });
            }
        }

        private readonly List<NfaState> m_nfa = new List<NfaState>();
        private readonly List<NfaState> m_dfa = new List<NfaState>();
        private readonly int m_root;

        public static void Build(GrammarTables result, IEnumerable<SymbolTerminal> terminals)
        {
            using (var builder = new BuildDFA())
            {
                builder.InternalBuild(terminals, result);
            }
        }

        public void Dispose()
        {
        }

        public int AddState()
        {
            m_nfa.Add(new NfaState());
            return m_nfa.Count - 1;
        }

        public void AddLambdaEdge(int source, int target)
        {
            m_nfa[source].AddLambdaEdge(target);
        }

        public void AddEdge(int source, int target, CharacterSet characterSet)
        {
            m_nfa[source].AddEdge(target, characterSet);
        }

        private BuildDFA()
        {
            m_root = AddState();
        }

        private void InternalBuild(IEnumerable<SymbolTerminal> terminals, GrammarTables result)
        {
            Console.WriteLine("Computing DFA States");
            foreach (var terminal in terminals)
            {
                if (terminal.Expression == null)
                    continue;

                var target = AddState();
                var source = AddState();
                foreach (Sequence sequence in terminal.Expression)
                {
                    Automata type = sequence.CreateAutomata(this);
                    m_nfa[target].AddLambdaEdge(type.Head);
                    m_nfa[type.Tail].AddLambdaEdge(source);
                    m_nfa[type.Tail].AddAccept(terminal, sequence.Priority);
                }
                m_nfa[m_root].AddLambdaEdge(target);
            }

            for (int i = 0; i < m_nfa.Count; ++i)
            {
                var reachable = new HashSet<int> { i };
                ClosureNFA(reachable);
                m_nfa[i].NFAClosure = reachable;
            }

            var nfaList = new HashSet<int> { m_root };
            result.DfaStartState = BuildDFAState(nfaList);

            foreach (var build in m_dfa)
            {
                Debug.Assert(build.TableIndex == result.DFA.Count);
                var state = new DFAState
                {
                    TableIndex = build.TableIndex
                };

                // copy edges.
                foreach (var edge in build.Edges)
                {
                    state.Edges.Add(edge);
                    //result.AddCharacterSet(edge.Value);
                }

                // get accept symbol.
                if (build.AcceptList.Count == 0)
                {
                    state.Accept = null;
                }
                else if (build.AcceptList.Count == 1)
                {
                    state.Accept = build.AcceptList[0].Symbol;
                }
                else
                {
                    var accept = build.AcceptList[0];
                    var priority = accept.Priority;

                    var list = new List<Symbol>();
                    list.Add(accept.Symbol);

                    for (int i = 1; i < build.AcceptList.Count; ++i)
                    {
                        accept = build.AcceptList[i];
                        if (accept.Priority == priority)
                        {
                            list.Add(accept.Symbol);
                        }
                        else if (accept.Priority < priority)
                        {
                            list.Clear();
                            list.Add(accept.Symbol);
                            priority = accept.Priority;
                        }
                    }

                    build.AcceptList.Clear();
                    foreach (var symbol in list)
                    {
                        build.AddAccept(symbol, priority);
                    }
                    if (list.Count == 1)
                    {
                        state.Accept = list[0];
                    }
                }

                result.DFA.Add(state);
            }

            CheckErrorsDFA(result);
            Console.WriteLine("DFA States Completed");
        }

        private void CheckErrorsDFA(GrammarTables result)
        {
            bool hasErrors = false;

            if (m_dfa[result.DfaStartState].AcceptList.Count > 0)
            {
                hasErrors = true;
                foreach (var item in m_dfa[result.DfaStartState].AcceptList)
                {
                    Console.WriteLine($"The terminal '{item.Symbol.Name}' can be zero length.");
                    Console.WriteLine("The definition of this terminal allows it to contain no characters.");
                }
            }

            if (!hasErrors)
            {
                foreach (var build in m_dfa)
                {
                    if (build.AcceptList.Count >= 2)
                    {
                        hasErrors = true;
                        Console.WriteLine($"DFA State '{build.TableIndex}' cannot distinguish between: ");
                        foreach (var item in build.AcceptList)
                        {
                            Console.WriteLine($"  {item.Symbol.Name}");
                        }
                    }
                }
            }

            if (!hasErrors)
            {
                var symbols = new HashSet<Symbol>(result.Symbols.OfType<SymbolTerminal>().Where(s => s.Expression != null));
                foreach (var build in result.DFA)
                {
                    symbols.Remove(build.Accept);
                }
                foreach (var build in symbols)
                {
                    hasErrors = true;
                    Console.WriteLine($"The terminal '{build.Name}' cannot be accepted by the DFA.");
                }
            }

            if (!hasErrors)
            {
                Console.WriteLine("The DFA State Table was successfully created.");
                Console.WriteLine($"The table contains a total of {result.DFA.Count} states.");
            }
        }

        private void ClosureNFA(HashSet<int> reachable)
        {
            var copy = reachable.ToArray();
            foreach (int num in copy)
            {
                foreach (var edge in m_nfa[num].Edges)
                {
                    if ((edge.Characters == null) && !reachable.Contains(edge.Target))
                    {
                        reachable.Add(edge.Target);
                        ClosureNFA(reachable);
                    }
                }
            }
        }

        private int BuildDFAState(HashSet<int> nfaList)
        {
            NfaState state = new NfaState();
            foreach (var num in nfaList)
            {
                state.NFAStates.UnionWith(m_nfa[num].NFAClosure);
            }

            int index = DFAStateNumber(state);
            if (index == -1)
            {
                index = AddDFAState(state);
                foreach (var num in state.NFAStates)
                {
                    foreach (var item in m_nfa[num].AcceptList)
                    {
                        state.AcceptList.Add(item);
                    }
                }

                var set2 = new List<int>();
                var list = new List<Edge>();
                foreach (var num in state.NFAStates)
                {
                    foreach (var edge in m_nfa[num].Edges)
                    {
                        list.Add(edge);
                        set2.Add(edge.Target);
                    }
                }

                if (set2.Count >= 1)
                {
                    var buildArray = new List<CharacterSet>(set2.Count);
                    foreach (int target in set2)
                    {
                        var build = new CharacterSet();
                        foreach (var edge in list)
                        {
                            if (edge.Target == target)
                            {
                                build.Add(edge.Characters);
                            }
                        }
                        foreach (var edge in list)
                        {
                            if (edge.Target != target)
                            {
                                build.Subtract(edge.Characters);
                            }
                        }
                        buildArray.Add(build);
                    }

                    CharacterSet build5 = new CharacterSet();
                    foreach (var edge in list)
                    {
                        build5.Add(edge.Characters);
                    }
                    foreach (var set in buildArray)
                    {
                        build5.Subtract(set);
                    }
                    for (int i = 0; i < set2.Count; ++i)
                    {
                        if (buildArray[i].Count >= 1)
                        {
                            var setList = new HashSet<int> { set2[i] };
                            state.AddEdge(BuildDFAState(setList), buildArray[i]);
                        }
                    }
                    foreach (var number in build5)
                    {
                        var setList = new HashSet<int>();
                        foreach (var edge in list)
                        {
                            if (ReferenceEquals(edge.Characters, null))
                                continue;

                            if (edge.Characters.Contains(number))
                            {
                                setList.Add(edge.Target);
                            }
                        }
                        if (setList.Count > 0)
                        {
                            var build = new CharacterSet();
                            build.Add(number);
                            state.AddEdge(BuildDFAState(setList), build);
                        }
                    }

                }
            }
            return index;
        }

        private int AddDFAState(NfaState state)
        {
            state.TableIndex = m_dfa.Count;
            m_dfa.Add(state);
            return state.TableIndex;
        }

        private int DFAStateNumber(NfaState state)
        {
            for (var i = 0; i < m_dfa.Count; ++i)
            {
                if (m_dfa[i].NFAStates.SetEquals(state.NFAStates))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}