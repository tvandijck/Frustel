namespace Frutsel
{
    public sealed class SymbolGroupEnd : SymbolTerminal
    {
        public SymbolGroupEnd()
        {
        }

        public SymbolGroupEnd(string name, Expression expression)
            : base(name, expression)
        {
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.GroupEnd; }
        }
    }
}