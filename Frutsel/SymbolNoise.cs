namespace Frutsel
{
    public sealed class SymbolNoise : SymbolTerminal
    {
        public SymbolNoise()
        {
        }

        public SymbolNoise(string name, Expression expression)
            : base(name, expression)
        {
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.Noise; }
        }
    }
}
