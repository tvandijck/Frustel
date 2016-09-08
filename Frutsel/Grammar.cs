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
        private readonly Dictionary<string, Symbol> m_symbols = new Dictionary<string, Symbol>();
        private bool m_caseSensitive = true;

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

        public IDictionary<string, CharacterSet> CharacterSets
        {
            get { return new ReadOnlyDictionary<string, CharacterSet>(m_characterSets); }
        }


        public CharacterSet AddCharacterSet(string name, CharacterSet set)
        {
            m_characterSets.Add(name, set);
            return set;
        }

        public Symbol AddSymbol(Symbol symbol)
        {
            m_symbols.Add(symbol.Name, symbol);
            return symbol;
        }

        public GrammarTables Build()
        {
            AssignPriorities();


            GrammarTables tables = new GrammarTables();

            // copy symbols for now.
            foreach (var symbol in m_symbols.Values)
            {
                tables.Symbols.Add(symbol);
            }

            // build the DFA table.
            BuildDFA.Build(tables, m_symbols.Values.OfType<SymbolTerminal>());


            // return result.
            return tables;
        }

        private void AssignPriorities()
        {
            foreach (var terminal in m_symbols.Values.OfType<SymbolTerminal>())
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
    }
}