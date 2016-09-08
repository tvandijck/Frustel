namespace GoldEngine
{
    internal interface ISetExpression
    {
        CharacterSetBuild Evaluate();
        NumberSet UsedDefinedSets();
    }
}