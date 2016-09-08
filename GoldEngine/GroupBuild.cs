namespace GoldEngine
{
    internal class GroupBuild : Group
    {
        // Fields
        internal string ContainerName;
        internal bool IsBlock;
        internal string NestingNames;

        // Methods
        internal GroupBuild()
        {
            this.IsBlock = false;
        }

        internal GroupBuild(string Name, bool IsBlock)
        {
            base.Name = Name;
            this.NestingNames = "None";
            base.Advance = AdvanceMode.Character;
            base.Ending = EndingMode.Closed;
            this.IsBlock = IsBlock;
        }

        internal GroupBuild(string Name, bool IsBlock, EndingMode Ending)
        {
            base.Name = Name;
            this.IsBlock = IsBlock;
            this.NestingNames = "None";
            base.Advance = AdvanceMode.Character;
            base.Ending = Ending;
        }

        internal GroupBuild(string Name, SymbolBuild Container, SymbolBuild Start, SymbolBuild End, EndingMode Mode)
        {
            base.Name = Name;
            base.Container = Container;
            base.Start = Start;
            base.End = End;
            base.Ending = Mode;
            this.IsBlock = false;
            this.NestingNames = "None";
            base.Advance = AdvanceMode.Character;
        }
    }
}