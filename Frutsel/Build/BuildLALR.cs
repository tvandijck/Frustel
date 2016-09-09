using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel.Build
{
    internal sealed class BuildLALR : IDisposable
    {
        public static void Build(GrammarTables result, Grammar grammar)
        {
            using (var builder = new BuildLALR(grammar))
            {
                builder.InternalBuild(result);
            }
        }

        private readonly Grammar m_grammar;
        private readonly List<ConflictTableItem> m_conflictTable = new List<ConflictTableItem>();
        private readonly HashSet<LRConfig>[] m_gotoList;

        private BuildLALR(Grammar grammar)
        {
            m_grammar = grammar;

            m_conflictTable.Add(new ConflictTableItem("Reduce-Reduce", LRActionType.Reduce, LRActionType.Reduce, LRConflict.ReduceReduce));
            m_conflictTable.Add(new ConflictTableItem("Shift-Reduce", LRActionType.Shift, LRActionType.Reduce, LRConflict.ShiftReduce));
            m_conflictTable.Add(new ConflictTableItem("Accept-Reduce", LRActionType.Accept, LRActionType.Reduce, LRConflict.AcceptReduce));
            m_gotoList = new HashSet<LRConfig>[m_grammar.BuildSymbols.Count + 1];
        }

        public void Dispose()
        {
        }

        private void InternalBuild(GrammarTables result)
        {
            var startSymbol = m_grammar.StartSymbol;
            if (ReferenceEquals(startSymbol, null))
            {
                throw new Exception("ERROR: Start symbol is invalid or missing");
            }

            Console.WriteLine("Computing LALR Tables");
            SetupTempTables();


            Console.WriteLine("LALR Tables Completed");
        }

        private void SetupTempTables()
        {
            //LRConfigSetLookup.Clear();
            SetupFirstTable();
            ComputePartialClosures();
        }

        private void SetupFirstTable()
        {
            SetupNullableTable();

            foreach (var s in m_grammar.BuildSymbols)
            {
                s.First.Clear();
                if (s.Symbol.Type != ESymbolType.NonTerminal)
                {
                    s.First.Add(new LookaheadSymbol(s));
                }
            }

            bool flag;
            do
            {
                flag = false;
                foreach (var production in m_grammar.BuildProductions)
                {
                    foreach (var handle in production.Handles)
                    {
                        foreach (var i in handle.First)
                        {
                            flag |= production.Head.First.Add(i);
                        }

                        if (handle.Nullable)
                            break;
                    }
                }
            }
            while (flag);
        }

        private void ComputePartialClosures()
        {
            var count = m_grammar.BuildSymbols.Count(s => s.Symbol.Type == ESymbolType.NonTerminal);

            int index = 0;
            foreach (var s in m_grammar.BuildSymbols.Where(s => s.Symbol.Type == ESymbolType.NonTerminal))
            {
                Console.WriteLine($"{index} of {count}");
                s.PartialClosure = GetClosureConfigSet(s);
                index++;
            }
        }

        private void SetupNullableTable()
        {
            // clear the nullable flag first.
            foreach (var s in m_grammar.BuildSymbols)
            {
                s.Nullable = false;
            }

            // then figure out nullable reductions.
            bool flag;
            do
            {
                flag = false;
                foreach (var production in m_grammar.BuildProductions)
                {
                    bool allNullable = production.Handles.All(s => s.Nullable);
                    if (allNullable && !production.Head.Nullable)
                    {
                        production.Head.Nullable = true;
                        flag = true;
                    }
                }
            }
            while (flag);
        }

        private HashSet<LRConfig> GetClosureConfigSet(BuildSymbol symbol)
        {
            var set = new HashSet<LRConfig>();
            foreach (var production in m_grammar.BuildProductions)
            {
                if (production.Head == symbol)
                {
                    set.Add(new LRConfig
                    {
                        Position = 0,
                        Modified = true,
                        LookaheadSet = new HashSet<LookaheadSymbol>(),
                        Parent = production,
                        InheritLookahead = true
                    });
                }
            }

            bool anyAdded;
            do
            {
                var setB = new HashSet<LRConfig>();
                foreach (LRConfig config in set)
                {
                    if (!config.IsComplete() & config.Modified)
                    {
                        BuildSymbol build = config.NextSymbol(0);
                        if (build.Symbol.Type == ESymbolType.NonTerminal)
                        {
                            foreach (var production in m_grammar.BuildProductions)
                            {
                                if (production.Head == build)
                                {
                                    setB.Add(new LRConfig(production, 0, TotalLookahead(config)));
                                }
                            }
                        }

                        config.Modified = false;
                    }
                }

                anyAdded = setB.Aggregate(false, (current, i) => current | set.Add(i));
            }
            while (anyAdded);

            bool flag;
            do
            {
                flag = false;
                foreach (LRConfig config in set)
                {
                    BuildSymbol build = config.NextSymbol(0);
                    if ((build != null) && config.InheritLookahead && PopulateLookahead(config) && (build.Symbol.Type == ESymbolType.NonTerminal))
                    {
                        foreach (LRConfig config2 in set)
                        {
                            if ((config2.Position == 0) && (config2.Parent.Head == build) && !config2.InheritLookahead)
                            {
                                config2.InheritLookahead = true;
                                flag = true;
                            }
                        }
                    }
                }
            }
            while (flag);

            return set;
        }

        private bool PopulateLookahead(LRConfig config)
        {
            if (config.InheritLookahead)
            {
                bool flag = false;
                for (var i = 0; (i < config.CheckaheadCount()) & !flag; ++i)
                {
                    var build = config.Checkahead(i);
                    flag = !build.Nullable;
                }
                return !flag;
            }
            return false;
        }

        private HashSet<LookaheadSymbol> TotalLookahead(LRConfig config)
        {
            bool flag = false;
            var set = new HashSet<LookaheadSymbol>();
            for (int offset = 0; (offset < config.CheckaheadCount()) && !flag; offset++)
            {
                var build = config.Checkahead(offset);
                ConfigTrackSource source = (build.Symbol.Type == ESymbolType.NonTerminal ? ConfigTrackSource.First : ConfigTrackSource.Config);

                foreach (var j in build.First)
                {
                    LookaheadSymbol item = new LookaheadSymbol(j);
                    if (item.Parent.Symbol.Type != ESymbolType.NonTerminal)
                    {
                        item.Configs.Add(new ConfigTrack(config, source));
                        set.Add(item);
                    }
                }
                flag = !build.Nullable;
            }

            if (!flag)
            {
                set.UnionWith(config.LookaheadSet);
            }
            return set;
        }

        private struct ConflictTableItem
        {
            public string Name;
            public LRActionType Action1;
            public LRActionType Action2;
            public LRConflict Conflict;

            public ConflictTableItem(string newName, LRActionType newAction1, LRActionType newAction2, LRConflict newConflict)
            {
                Name = newName;
                Action1 = newAction1;
                Action2 = newAction2;
                Conflict = newConflict;
            }
        }
    }


    internal enum LRStatus : byte
    {
        Critical = 3,
        Info = 1,
        None = 0,
        Warning = 2
    }

    internal enum LRActionType
    {
        Accept = 4,
        Error = 5,
        Goto = 3,
        Reduce = 2,
        Shift = 1
    }

    internal enum LRConflict
    {
        None,
        ShiftReduce,
        ReduceReduce,
        AcceptReduce
    }

    internal enum ConfigTrackSource
    {
        Inherit,
        Config,
        First
    }
}
