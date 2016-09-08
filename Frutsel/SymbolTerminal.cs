

namespace Frutsel
{
    public class SymbolTerminal : Symbol
    {
        private readonly Expression m_expression;
        private bool m_variableLength;

        public SymbolTerminal()
        {
        }

        public SymbolTerminal(string name, Expression expression)
        {
            Name = name;
            m_expression = expression;
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.Content; }
        }

        public Expression Expression
        {
            get { return m_expression; }
        }

        public bool VariableLength
        {
            get { return m_variableLength; }
            set { m_variableLength = value; }
        }
    }
}