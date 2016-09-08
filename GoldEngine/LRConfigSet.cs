namespace GoldEngine
{
    internal class LRConfigSet : DictionarySet
    {
        // Methods
        public bool Add(LRConfig Item)
        {
            return base.Add(new DictionarySet.IMember[] { Item });
        }

        public LRConfigCompare CompareCore(LRConfigSet ConfigB)
        {
            bool flag;
            bool flag2;
            bool flag3;
            if (base.Count() == ConfigB.Count())
            {
                flag = false;
                flag2 = false;
                flag3 = false;
                for (short i = 0; (i < base.Count()) & !flag3; i = (short)(i + 1))
                {
                    LRConfig configB = ConfigB[i];
                    switch (this[i].CompareCore(configB))
                    {
                    case LRConfigCompare.ProperSubset:
                        flag2 = true;
                        break;

                    case LRConfigCompare.EqualCore:
                        flag = true;
                        break;

                    case LRConfigCompare.UnEqual:
                        flag3 = true;
                        break;
                    }
                }
            }
            else
            {
                flag3 = true;
            }
            if (flag3)
            {
                return LRConfigCompare.UnEqual;
            }
            if (flag)
            {
                return LRConfigCompare.EqualCore;
            }
            if (flag2)
            {
                return LRConfigCompare.ProperSubset;
            }
            return LRConfigCompare.EqualFull;
        }

        public bool EqualCore(LRConfigSet ConfigSetB)
        {
            return this.IsEqualTo(ConfigSetB);
        }

        public bool IsEqualTo(LRConfigSet ConfigSetB)
        {
            bool flag2;
            if (base.Count() == ConfigSetB.Count())
            {
                short num = 0;
                flag2 = false;
                while ((num < base.Count()) & !flag2)
                {
                    LRConfig config = (LRConfig)base[num];
                    LRConfig config2 = ConfigSetB[num];
                    if (!config.IsEqualTo(config2))
                    {
                        flag2 = true;
                    }
                    num = (short)(num + 1);
                }
            }
            else
            {
                flag2 = true;
            }
            return !flag2;
        }

        public bool IsGreaterThan(LRConfigSet ConfigSetB)
        {
            LRConfigSet configSetB = this;
            return ConfigSetB.IsLessThan(configSetB);
        }

        public bool IsLessThan(LRConfigSet ConfigSetB)
        {
            bool flag = false;
            short num3 = 0;
            bool flag3 = false;
            if (base.Count() < ConfigSetB.Count())
            {
                return true;
            }
            if (base.Count() > ConfigSetB.Count())
            {
                return false;
            }
            while ((num3 < base.Count()) & !flag)
            {
                LRConfig config = (LRConfig)base[num3];
                LRConfig config2 = ConfigSetB[num3];
                int num = config.TableIndex();
                int num2 = config2.TableIndex();
                short position = config.Position;
                short num5 = config2.Position;
                if (num != num2)
                {
                    flag3 = num < num2;
                    flag = true;
                }
                else if (position != num5)
                {
                    flag3 = position < num5;
                    flag = true;
                }
                num3 = (short)(num3 + 1);
            }
            return flag3;
        }

        public LRConfigSet Union(LRConfigSet SetB)
        {
            return this.Union(SetB);
        }

        public bool UnionWith(LRConfigSet SetB)
        {
            return base.UnionWith(SetB);
        }

        // Properties
        public new LRConfig this[int Index]
        {
            get
            {
                return (LRConfig)base[Index];
            }
        }
    }
}