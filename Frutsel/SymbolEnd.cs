namespace Frutsel
{
    public sealed class SymbolEnd : Symbol
    {
        public SymbolEnd()
        {
            Name = "EOF";
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.End; }
        }
    }
}
