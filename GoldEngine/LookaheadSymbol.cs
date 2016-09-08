using System;

namespace GoldEngine
{
    internal class LookaheadSymbol : DictionarySet.IMember
    {
        // Fields
        public ConfigTrackSet Configs;
        public SymbolBuild Parent;

        // Methods
        public LookaheadSymbol()
        {
            this.Configs = new ConfigTrackSet();
        }

        public LookaheadSymbol(LookaheadSymbol Look)
        {
            this.Parent = Look.Parent;
            this.Configs = new ConfigTrackSet();
            this.Configs.Copy(Look.Configs);
        }

        public LookaheadSymbol(SymbolBuild Sym)
        {
            this.Parent = Sym;
            this.Configs = new ConfigTrackSet();
        }

        public DictionarySet.MemberResult Difference(DictionarySet.IMember NewObject)
        {
            return null;
        }

        public DictionarySet.MemberResult Intersect(DictionarySet.IMember NewObject)
        {
            return new DictionarySet.MemberResult(this);
        }

        public IComparable Key()
        {
            return (IComparable)this.Parent.TableIndex;
        }

        public DictionarySet.MemberResult Union(DictionarySet.IMember Obj)
        {
            LookaheadSymbol symbol = (LookaheadSymbol)Obj;
            return new DictionarySet.MemberResult(new LookaheadSymbol
            {
                Parent = this.Parent,
                Configs = new ConfigTrackSet(this.Configs, symbol.Configs)
            });
        }
    }
}