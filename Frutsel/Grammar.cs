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

        public Grammar()
        {
            // Constants.
            m_characterSets.Add("HT", new CharacterSet('\x0009'));
            m_characterSets.Add("LF", new CharacterSet('\x000A'));
            m_characterSets.Add("VT", new CharacterSet('\x000B'));
            m_characterSets.Add("FF", new CharacterSet('\x000C'));
            m_characterSets.Add("CR", new CharacterSet('\x000D'));
            m_characterSets.Add("Space", new CharacterSet('\x0020'));
            m_characterSets.Add("NBSP", new CharacterSet('\x00A0'));
            m_characterSets.Add("LS", new CharacterSet('\x2028'));
            m_characterSets.Add("PS", new CharacterSet('\x2029'));

            // Common Character Sets.
            var digit = new CharacterSet('0', '9');
            var letter = new CharacterSet('a', 'z') + new CharacterSet('A', 'Z');
            var alphaNumeric = digit + letter;
            var printable = new CharacterSet('\x0020', '\x007e') + '\x00A0';
            var whitespace = new CharacterSet('\x0009', '\x000d') + '\x0020' + '\x00A0';

            m_characterSets.Add("Digit", digit);
            m_characterSets.Add("Letter", letter);
            m_characterSets.Add("Alphanumeric", alphaNumeric);
            m_characterSets.Add("Printable", printable);
            m_characterSets.Add("Whitespace", whitespace);

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