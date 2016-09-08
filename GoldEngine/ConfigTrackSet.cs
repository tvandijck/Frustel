namespace GoldEngine
{
    internal class ConfigTrackSet : DictionarySet
    {
        // Methods
        public ConfigTrackSet()
        {
        }

        public ConfigTrackSet(ConfigTrackSet A, ConfigTrackSet B) : base(A, B)
        {
        }

        public bool Add(ConfigTrack Item)
        {
            return base.Add(new DictionarySet.IMember[] { Item });
        }

        public bool UnionWith(ConfigTrackSet SetB)
        {
            return base.UnionWith(SetB);
        }

        // Properties
        public new ConfigTrack this[int Index]
        {
            get
            {
                return (ConfigTrack)base[Index];
            }
        }
    }
}