using System;

namespace GoldEngine
{
    internal class ConfigTrack : DictionarySet.IMember
    {
        // Fields
        public bool FromConfig;
        public bool FromFirst;
        public LRConfig Parent;

        // Methods
        public ConfigTrack(ConfigTrack Track)
        {
            this.Parent = Track.Parent;
            this.FromConfig = Track.FromConfig;
            this.FromFirst = Track.FromFirst;
        }

        public ConfigTrack(LRConfig Config, ConfigTrackSource Source)
        {
            this.Parent = Config;
            this.FromConfig = Source == ConfigTrackSource.Config;
            this.FromFirst = Source == ConfigTrackSource.First;
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
            return (IComparable)this.Parent.TableIndex();
        }

        public string Text()
        {
            string marker = "^";
            return Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(this.FromFirst ? "F" : "-", this.FromConfig ? "C" : "-"), " : "), this.Parent.Text(marker)));
        }

        public DictionarySet.MemberResult Union(DictionarySet.IMember Obj)
        {
            ConfigTrack track2 = (ConfigTrack)Obj;
            return new DictionarySet.MemberResult(new ConfigTrack(this)
            {
                FromConfig = this.FromConfig | track2.FromConfig,
                FromFirst = this.FromFirst | track2.FromFirst
            });
        }
    }
}