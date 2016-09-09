using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Frutsel.Build;

namespace Frutsel
{
    public class Grammar
    {
        private readonly Dictionary<string, CharacterSet> m_characterSets = new Dictionary<string, CharacterSet>();
        private readonly Dictionary<string, int> m_groupIndices = new Dictionary<string, int>();
        private readonly List<Group> m_groups = new List<Group>();
        private readonly Dictionary<string, int> m_symbolIndices = new Dictionary<string, int>();
        private readonly List<BuildSymbol> m_symbols = new List<BuildSymbol>();
        private readonly List<BuildProduction> m_productions = new List<BuildProduction>();

        private bool m_caseSensitive = true;
        private bool m_autoWhitespace = true;
        private Symbol m_startSymbol = null;

        private readonly CharacterSet m_ht;
        private readonly CharacterSet m_lf;
        private readonly CharacterSet m_vt;
        private readonly CharacterSet m_ff;
        private readonly CharacterSet m_cr;
        private readonly CharacterSet m_space;
        private readonly CharacterSet m_nbsp;
        private readonly CharacterSet m_ls;
        private readonly CharacterSet m_ps;

        private readonly CharacterSet m_digit;
        private readonly CharacterSet m_letter;
        private readonly CharacterSet m_alphaNumeric;
        private readonly CharacterSet m_printable;
        private readonly CharacterSet m_whitespace;

        public Grammar()
        {
            // Constants.
            m_ht = AddCharacterSet("HT", new CharacterSet('\x0009'));
            m_lf = AddCharacterSet("LF", new CharacterSet('\x000A'));
            m_vt = AddCharacterSet("VT", new CharacterSet('\x000B'));
            m_ff = AddCharacterSet("FF", new CharacterSet('\x000C'));
            m_cr = AddCharacterSet("CR", new CharacterSet('\x000D'));
            m_space = AddCharacterSet("Space", new CharacterSet('\x0020'));
            m_nbsp = AddCharacterSet("NBSP", new CharacterSet('\x00A0'));
            m_ls = AddCharacterSet("LS", new CharacterSet('\x2028'));
            m_ps = AddCharacterSet("PS", new CharacterSet('\x2029'));

            // Common Character Sets.
            m_digit = AddCharacterSet("Digit", new CharacterSet('0', '9'));
            m_letter = AddCharacterSet("Letter", new CharacterSet('a', 'z') + new CharacterSet('A', 'Z'));
            m_alphaNumeric = AddCharacterSet("Alphanumeric", m_digit + m_letter);
            m_printable = AddCharacterSet("Printable", new CharacterSet('\x0020', '\x007e') + '\x00A0');
            m_whitespace = AddCharacterSet("Whitespace", new CharacterSet('\x0009', '\x000d') + '\x0020' + '\x00A0');

            // Common Symbols.
            AddSymbol(new SymbolEnd());
            AddSymbol(new SymbolError());
        }

        public bool CaseSensitive
        {
            get { return m_caseSensitive; }
            set { m_caseSensitive = value; }
        }

        public bool AutoWhitespace
        {
            get { return m_autoWhitespace; }
            set { m_autoWhitespace = value; }
        }

        public Symbol StartSymbol
        {
            get { return m_startSymbol; }
            set { m_startSymbol = value; }
        }

        public IDictionary<string, CharacterSet> CharacterSets
        {
            get { return new ReadOnlyDictionary<string, CharacterSet>(m_characterSets); }
        }

        internal IList<BuildSymbol> BuildSymbols
        {
            get { return m_symbols.AsReadOnly(); }
        }

        internal IList<BuildProduction> BuildProductions
        {
            get { return m_productions.AsReadOnly(); }
        }

        public CharacterSet HT
        {
            get { return m_ht; }
        }

        public CharacterSet LF
        {
            get { return m_lf; }
        }

        public CharacterSet VT
        {
            get { return m_vt; }
        }

        public CharacterSet FF
        {
            get { return m_ff; }
        }

        public CharacterSet CR
        {
            get { return m_cr; }
        }

        public CharacterSet Space
        {
            get { return m_space; }
        }

        public CharacterSet Nbsp
        {
            get { return m_nbsp; }
        }

        public CharacterSet LS
        {
            get { return m_ls; }
        }

        public CharacterSet PS
        {
            get { return m_ps; }
        }

        public CharacterSet Digit
        {
            get { return m_digit; }
        }

        public CharacterSet Letter
        {
            get { return m_letter; }
        }

        public CharacterSet AlphaNumeric
        {
            get { return m_alphaNumeric; }
        }

        public CharacterSet Printable
        {
            get { return m_printable; }
        }

        public CharacterSet Whitespace
        {
            get { return m_whitespace; }
        }

        public CharacterSet AddCharacterSet(string name, CharacterSet set)
        {
            m_characterSets.Add(name, set);
            return set;
        }

        public Symbol AddSymbol(Symbol symbol)
        {
            if (symbol.TableIndex == -1)
            {
                int index = m_symbols.Count;
                symbol.TableIndex = index;
                m_symbolIndices.Add(symbol.Name, index);
                m_symbols.Add(new BuildSymbol(symbol));
            }
            return symbol;
        }

        public Group AddGroup(Group group)
        {
            if (group.TableIndex == -1)
            {
                int index = m_groups.Count;
                group.TableIndex = index;
                m_groupIndices.Add(group.Name, index);
                m_groups.Add(group);
            }
            return group;
        }

        public void AddProduction(Symbol head, params Symbol[] handles)
        {
            var h = m_symbols[head.TableIndex];
            var t = handles.Select(s => m_symbols[s.TableIndex]).ToArray();

            var production = new BuildProduction(h, t);
            production.TableIndex = m_productions.Count;
            m_productions.Add(production);
        }

        public GrammarTables Build()
        {
            Populate();
            AssignTableIndexes();
            LinkGroupSymbolsToGroup();
            AssignPriorities();

            int count = m_symbols.Count(s => s.Symbol.Type == ESymbolType.Content);
            Console.WriteLine($"The grammar contains a total of {count} formal terminals.");
            DoSemanticAnalysis();

            // create result table.
            var tables = new GrammarTables();

            // copy groups.
            foreach (var group in m_groups)
            {
                tables.Groups.Add(group);
            }

            // copy symbols.
            foreach (var sym in m_symbols)
            {
                tables.Symbols.Add(sym.Symbol);
            }

            // build the DFA table.
            BuildLALR.Build(tables, this);
            BuildDFA.Build(tables, this);

            // return result.
            return tables;
        }

        private void Populate()
        {
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

        private void AssignTableIndexes()
        {
        }

        private void LinkGroupSymbolsToGroup()
        {
            foreach (Group group in m_groups)
            {
                group.Container.Group = group;
                group.Start.Group = group;
                group.End.Group = group;
            }
        }

        private void AssignPriorities()
        {
            foreach (var terminal in m_symbols.OfType<SymbolTerminal>())
            {
                if (terminal.Expression == null)
                    continue;

                bool flag = false;
                foreach (var seq in terminal.Expression)
                {
                    if (seq.Priority == -1)
                    {
                        if (seq.IsVariableLength())
                        {
                            seq.Priority = 10001;
                            flag = true;
                        }
                        else
                        {
                            seq.Priority = 0;
                        }
                    }
                }

                terminal.VariableLength = flag;
                if (flag)
                {
                    Console.WriteLine(terminal.Name + " is variable length.");
                }
            }
        }

        private void DoSemanticAnalysis()
        {
        }


        private void PopulateProperties()
        {

        }

        private void PopulateSets()
        {

        }

        private void PopulateDefinedSymbols()
        {

        }

        private void PopulateGroupsAndWhitespace()
        {
            // create the new line symbol if it's not already there.
            int newline;
            if (!m_symbolIndices.TryGetValue("NewLine", out newline))
            {
                // NewLine = {LF}|{CR}{LF}?|{LS}|{PS}
                AddSymbol(new SymbolNoise("NewLine", new Expression
                {
                    new Sequence {{EKleene.None, m_lf}},
                    new Sequence {{EKleene.None, m_cr}, {EKleene.ZeroOrOne, m_lf}},
                    new Sequence {{EKleene.None, m_ls}},
                    new Sequence {{EKleene.None, m_ps}},
                }));
            }

            // check whitespace.
            int whitespace;
            if (!m_symbolIndices.TryGetValue("Whitespace", out whitespace) && m_autoWhitespace)
            {
                // Whitespace = {Whitespace}+
                AddSymbol(new SymbolNoise("Whitespace", new Expression
                {
                    new Sequence
                    {
                        {EKleene.OneOrMore, m_whitespace},
                    },
                }));

                Console.WriteLine("The whitespace terminal was implicitly defined as '{Whitespace}+'.");
            }
            else
            {
                if (m_symbols[whitespace].Symbol.Type != ESymbolType.Noise)
                {
                    Console.WriteLine("WARNING: The whitespace terminal was not defined as Noise.");
                }
            }

            // check comment.
            int comment;
            if (m_symbolIndices.TryGetValue("Comment", out comment))
            {
                if (m_symbols[comment].Symbol.Type != ESymbolType.Noise)
                {
                    Console.WriteLine("WARNING: The comment terminal was not defined as Noise.");
                }
            }
        }

        private void PopulateProductionHeads()
        {

        }

        private void PopulateHandleSymbols()
        {

        }

        private void PopulateProductions()
        {

        }

        private void PopulateStartSymbol()
        {

        }

        private void ApplyGroupAttributes()
        {

        }

        private void ApplySymbolAttributes()
        {

        }

        private void ApplyVirtualProperty()
        {

        }

        private void CreateImplicitDeclarations()
        {
        }
    }
}
