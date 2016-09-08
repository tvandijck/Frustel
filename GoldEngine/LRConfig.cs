using System;

namespace GoldEngine
{
    internal class LRConfig : IComparable, DictionarySet.IMember
    {
        // Fields
        public bool InheritLookahead;
        public LookaheadSymbolSet LookaheadSet;
        public bool Modified;
        public ProductionBuild Parent;
        public short Position;
        public LRStatus Status;

        // Methods
        public LRConfig()
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = null;
            this.Position = 0;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
        }

        public LRConfig(LRConfig Config)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Config.Parent;
            this.Position = Config.Position;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = Config.Modified;
            this.InheritLookahead = Config.InheritLookahead;
            this.LookaheadSet.Copy(Config.LookaheadSet);
        }

        public LRConfig(ProductionBuild Rule)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Rule;
            this.Position = 0;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
        }

        public LRConfig(ProductionBuild Parent, int Position, LookaheadSymbolSet InitSet)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Parent;
            this.Position = (short)Position;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
            int num2 = InitSet.Count() - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.LookaheadSet.Add(new LookaheadSymbol(InitSet[i]));
            }
        }

        public SymbolBuild Checkahead(short Offset = 0)
        {
            if (this.Position <= ((this.Parent.Handle().Count() - 1) - Offset))
            {
                return this.Parent.Handle()[(this.Position + 1) + Offset];
            }
            return null;
        }

        public short CheckaheadCount()
        {
            return (short)((this.Parent.Handle().Count() - this.Position) - 1);
        }

        public LRConfigCompare CompareCore(LRConfig ConfigB)
        {
            if ((this.Parent.TableIndex == ConfigB.TableIndex()) & (this.Position == ConfigB.Position))
            {
                LRConfigCompare compare2;
                switch (this.LookaheadSet.CompareTo(ConfigB.LookaheadSet))
                {
                case DictionarySet.Compare.Equal:
                    return LRConfigCompare.EqualFull;

                case DictionarySet.Compare.UnEqual:
                    return LRConfigCompare.EqualCore;

                case DictionarySet.Compare.Subset:
                    return LRConfigCompare.ProperSubset;
                }
                return compare2;
            }
            return LRConfigCompare.UnEqual;
        }

        public int CompareTo(object obj)
        {
            LRConfig config = (LRConfig)obj;
            if (this.IsEqualTo(config))
            {
                return 0;
            }
            if (this.IsLessThan(config))
            {
                return -1;
            }
            return 1;
        }

        public DictionarySet.MemberResult Difference(DictionarySet.IMember NewObject)
        {
            return null;
        }

        public bool HasEqualCore(LRConfig Config)
        {
            return this.IsEqualTo(Config);
        }

        public DictionarySet.MemberResult Intersect(DictionarySet.IMember NewObject)
        {
            return null;
        }

        public bool IsComplete()
        {
            return (this.Position > (this.Parent.Handle().Count() - 1));
        }

        public bool IsEqualTo(LRConfig Config)
        {
            return ((this.Parent.TableIndex == Config.TableIndex()) & (this.Position == Config.Position));
        }

        public bool IsGreaterThan(LRConfig ConfigB)
        {
            LRConfig configB = this;
            return ConfigB.IsLessThan(configB);
        }

        public bool IsLessThan(LRConfig ConfigB)
        {
            if (this.Position != ConfigB.Position)
            {
                return (this.Position > ConfigB.Position);
            }
            return ((this.Parent.TableIndex != ConfigB.TableIndex()) && (this.Parent.TableIndex < ConfigB.TableIndex()));
        }

        public IComparable Key()
        {
            return this;
        }

        public LRActionType NextAction()
        {
            if (this.Position > (this.Parent.Handle().Count() - 1))
            {
                return LRActionType.Reduce;
            }
            if (this.NextSymbol(0).Category() == SymbolCategory.Terminal)
            {
                return LRActionType.Shift;
            }
            return LRActionType.Goto;
        }

        public SymbolBuild NextSymbol(int Offset = 0)
        {
            return this.Parent.Handle()[this.Position + Offset];
        }

        public int TableIndex()
        {
            return this.Parent.TableIndex;
        }

        public string Text(string Marker = "^")
        {
            string str = "<" + this.Parent.Head.Name + "> ::=";
            short num2 = (short)(this.Parent.Handle().Count() - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                if (i == this.Position)
                {
                    str = str + " " + Marker;
                }
                str = str + " " + this.Parent.Handle()[i].Text(false);
            }
            if (this.Position > (this.Parent.Handle().Count() - 1))
            {
                str = str + " " + Marker;
            }
            return str;
        }

        public DictionarySet.MemberResult Union(DictionarySet.IMember NewObject)
        {
            bool flag;
            LRConfig config = (LRConfig)NewObject;
            LRConfig newObject = new LRConfig(this);
            if (newObject.LookaheadSet.UnionWith(config.LookaheadSet))
            {
                flag = true;
                newObject.Modified = true;
            }
            else
            {
                newObject.Modified = this.Modified | config.Modified;
                flag = false;
            }
            newObject.InheritLookahead = this.InheritLookahead | config.InheritLookahead;
            return new DictionarySet.MemberResult(newObject, flag);
        }
    }
}