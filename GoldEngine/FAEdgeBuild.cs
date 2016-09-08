namespace GoldEngine
{
    internal class FAEdgeBuild : FAEdge
    {
        public FAEdgeBuild(CharacterSetBuild CharSet, int Target) 
            : base(CharSet, Target)
        {
        }

        public new CharacterSetBuild Characters
        {
            get { return (CharacterSetBuild) base.Characters; }
            set { base.Characters = value; }
        }
    }
}