using System;

namespace Frutsel
{
    public class CharacterSetNode : Node
    {
        private readonly CharacterSet m_characterSet;

        public CharacterSetNode(EKleene kleene, CharacterSet characterSet)
            : base(kleene)
        {
            m_characterSet = characterSet;
        }

        public override Automata CreateAutomata(BuildDFA buildDfa)
        {
            var head = buildDfa.AddState();
            var tail = buildDfa.AddState();
            buildDfa.AddEdge(head, tail, m_characterSet);

            HandleKleen(buildDfa, head, tail);

            return new Automata
            {
                Head = head,
                Tail = tail
            };
        }

        public override bool IsVariableLength()
        {
            return (Kleene == EKleene.ZeroOrMore) || (Kleene == EKleene.OneOrMore);
        }

        public override string ToString()
        {
            string set = $"[{m_characterSet}]";
            switch (Kleene)
            {
                case EKleene.ZeroOrMore:
                    return set + "*";
                case EKleene.OneOrMore:
                    return set + "+";
                case EKleene.ZeroOrOne:
                    return set + "?";
            }
            return set;
        }
    }
}