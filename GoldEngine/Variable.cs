namespace GoldEngine
{
    internal class Variable
    {
        // Fields
        public string Name;
        public string Value;

        // Methods
        public Variable()
        {
            this.Name = "";
            this.Value = "";
        }

        public Variable(string Name)
        {
            this.Name = Name;
            this.Value = "";
        }
    }
}