namespace Frutsel
{
    public abstract class Symbol
    {
        public string Name { get; set; }
        public Group Group { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name}({Name})";
        }

        public abstract ESymbolType Type { get; }
    }
}