using System.Collections.Generic;

namespace Frutsel
{
    public class GrammarTables
    {
        private readonly List<DFAState> m_dfa = new List<DFAState>();
        private readonly List<Symbol> m_symbols = new List<Symbol>();
        private readonly List<Group> m_groups = new List<Group>();
        private int m_dfaStartState;

        public IList<DFAState> DFA
        {
            get {  return m_dfa; }
        }

        public IList<Symbol> Symbols
        {
            get { return m_symbols; }
        }

        public IList<Group> Groups
        {
            get { return m_groups; }
        }

        public int DfaStartState
        {
            get { return m_dfaStartState; }
            set { m_dfaStartState = value; }
        }
    }
}
