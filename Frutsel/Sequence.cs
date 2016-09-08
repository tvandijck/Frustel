using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Frutsel.Build;

namespace Frutsel
{
    public struct Automata
    {
        public int Head;
        public int Tail;
    }


    public sealed class Sequence : IEnumerable<Node>
    {
        private readonly List<Node> m_items = new List<Node>();
        private int m_priority = -1;

        public int Priority
        {
            get { return m_priority; }
            set { m_priority = value; }
        }

        public void Add(EKleene kleene, CharacterSet set)
        {
            m_items.Add(new CharacterSetNode(kleene, set));
        }

        public void Add(EKleene kleene, Expression expression)
        {
            m_items.Add(new ExpressionNode(kleene, expression));
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Automata CreateAutomata(BuildDFA buildDfa)
        {
            Debug.Assert(m_items.Count > 0);

            var automata = m_items[0].CreateAutomata(buildDfa);
            var head = automata.Head;
            var tail = automata.Tail;
            for (var i=1; i<m_items.Count; ++i)
            {
                automata = m_items[i].CreateAutomata(buildDfa);
                buildDfa.AddLambdaEdge(tail, automata.Head);
                tail = automata.Tail;
            }

            return new Automata
            {
                Head = head,
                Tail = tail
            };
        }

        public bool IsVariableLength()
        {
            return m_items.Any(item => item.IsVariableLength());
        }

        public override string ToString()
        {
            return string.Join(" ", m_items);
        }
    }
}