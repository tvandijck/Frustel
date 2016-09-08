namespace Frutsel
{
    public sealed class SymbolError : Symbol
    {
        public SymbolError()
        {
            Name = "Error";
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.Error; }
        }
    }
}
