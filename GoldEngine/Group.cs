namespace GoldEngine
{
    internal class Group
    {
        // Fields
        internal AdvanceMode Advance = AdvanceMode.Character;
        internal Symbol Container;
        internal Symbol End;
        internal EndingMode Ending = EndingMode.Closed;
        internal string Name;
        internal IntegerList Nesting = new IntegerList();
        internal Symbol Start;
        internal short TableIndex;
    }
}