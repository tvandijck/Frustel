namespace Frutsel.Build
{
    public abstract class Node
    {
        private readonly EKleene m_kleene;

        protected Node(EKleene kleene)
        {
            m_kleene = kleene;
        }

        public EKleene Kleene
        {
            get { return m_kleene; }
        }

        public abstract Automata CreateAutomata(BuildDFA buildDfa);
        public abstract bool IsVariableLength();

        protected void HandleKleen(BuildDFA buildDfa, int head, int tail)
        {
            switch (m_kleene)
            {
                case EKleene.ZeroOrMore:
                    buildDfa.AddLambdaEdge(head, tail);
                    buildDfa.AddLambdaEdge(tail, head);
                    break;

                case EKleene.OneOrMore:
                    buildDfa.AddLambdaEdge(tail, head);
                    break;

                case EKleene.ZeroOrOne:
                    buildDfa.AddLambdaEdge(head, tail);
                    break;
            }
        }
    }
}