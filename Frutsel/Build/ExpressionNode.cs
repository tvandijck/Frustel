using System.Linq;

namespace Frutsel.Build
{
    internal sealed class ExpressionNode : Node
    {
        private readonly Expression m_expression;

        public ExpressionNode(EKleene kleene, Expression expression)
            : base(kleene)
        {
            m_expression = expression;
        }

        public override Automata CreateAutomata(BuildDFA buildDfa)
        {
            int head = buildDfa.AddState();
            int tail = buildDfa.AddState();
            foreach (Sequence sequence in m_expression)
            {
                Automata type = sequence.CreateAutomata(buildDfa);
                buildDfa.AddLambdaEdge(head, type.Head);
                buildDfa.AddLambdaEdge(type.Tail, tail);
            }

            HandleKleen(buildDfa, head, tail);

            return new Automata
            {
                Head = head,
                Tail = tail
            };
        }

        public override bool IsVariableLength()
        {
            return (Kleene == EKleene.ZeroOrMore) || 
                (Kleene == EKleene.OneOrMore) || 
                m_expression.Any(sequence => sequence.IsVariableLength());
        }

        public override string ToString()
        {
            string set = $"({m_expression})";
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
