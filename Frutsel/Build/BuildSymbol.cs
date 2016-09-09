using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel.Build
{
    internal class BuildSymbol
    {
        public readonly Symbol Symbol;
        public readonly HashSet<LookaheadSymbol> First = new HashSet<LookaheadSymbol>();
        public bool Nullable;
        public HashSet<LRConfig> PartialClosure;

        public BuildSymbol(Symbol symbol)
        {
            Symbol = symbol;
        }
    }

    internal class BuildProduction
    {
        private BuildSymbol m_head;
        private BuildSymbol[] m_handles;
        private int m_tableIndex = -1;

        public BuildProduction(BuildSymbol head, params BuildSymbol[] handles)
        {
            m_head = head;
            m_handles = handles;
        }

        public BuildSymbol Head
        {
            get { return m_head; }
        }

        public BuildSymbol[] Handles
        {
            get { return m_handles; }
        }

        public int TableIndex
        {
            get { return m_tableIndex; }
            set { m_tableIndex = value; }
        }
    }

    internal class ConfigTrack : IEquatable<ConfigTrack>
    {
        private readonly bool m_fromConfig;
        private readonly bool m_fromFirst;
        private readonly LRConfig m_parent;

        public ConfigTrack(ConfigTrack track)
        {
            m_parent = track.m_parent;
            m_fromConfig = track.FromConfig;
            m_fromFirst = track.m_fromFirst;
        }

        public ConfigTrack(LRConfig config, ConfigTrackSource source)
        {
            m_parent = config;
            m_fromConfig = source == ConfigTrackSource.Config;
            m_fromFirst = source == ConfigTrackSource.First;
        }

        public LRConfig Parent
        {
            get { return m_parent; }
        }

        public bool FromFirst
        {
            get { return m_fromFirst; }
        }

        public bool FromConfig
        {
            get { return m_fromConfig; }
        }

        public bool Equals(ConfigTrack other)
        {
            return ReferenceEquals(this, other) ||
                   m_parent == other.m_parent &&
                   m_fromConfig == other.m_fromConfig &&
                   m_fromFirst == other.m_fromFirst;
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(m_parent, null) ? -1 : m_parent.GetHashCode();
        }
    }

    internal class LookaheadSymbol : IEquatable<LookaheadSymbol>
    {
        private readonly HashSet<ConfigTrack> m_configs = new HashSet<ConfigTrack>();
        private readonly BuildSymbol m_parent;

        public LookaheadSymbol()
        {
        }

        public LookaheadSymbol(LookaheadSymbol look)
        {
            m_parent = look.m_parent;
            m_configs.UnionWith(look.m_configs);
        }

        public LookaheadSymbol(BuildSymbol sym)
        {
            m_parent = sym;
        }

        public BuildSymbol Parent
        {
            get { return m_parent; }
        }

        public HashSet<ConfigTrack> Configs
        {
            get { return m_configs; }
        }


        public bool Equals(LookaheadSymbol other)
        {
            return ReferenceEquals(this, other) ||
                   ReferenceEquals(m_parent, other.m_parent) &&
                   m_configs.SetEquals(other.m_configs);
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(m_parent, null) ? -1 : m_parent.GetHashCode();
        }
    }

    internal class LRConfig : IEquatable<LRConfig>
    {
        public bool InheritLookahead;
        public HashSet<LookaheadSymbol> LookaheadSet;
        public bool Modified;
        public BuildProduction Parent;
        public int Position;
        public LRStatus Status;

        public LRConfig()
        {
        }

        public LRConfig(BuildProduction parent, int position, IEnumerable<LookaheadSymbol> initSet)
        {
            Parent = parent;
            Position = position;
            LookaheadSet = new HashSet<LookaheadSymbol>();
            Modified = true;
            InheritLookahead = false;
            LookaheadSet.UnionWith(initSet);
        }

        public bool IsComplete()
        {
            return Position >= Parent.Handles.Length;
        }

        public BuildSymbol NextSymbol(int offset = 0)
        {
            return Parent.Handles[Position + offset];
        }

        public int CheckaheadCount()
        {
            return Parent.Handles.Length - Position - 1;
        }

        public BuildSymbol Checkahead(int offset = 0)
        {
            if (Position <= Parent.Handles.Length - 1 - offset)
            {
                return Parent.Handles[Position + 1 + offset];
            }
            return null;
        }

        public bool Equals(LRConfig other)
        {
            return (Parent.TableIndex == other.Parent.TableIndex) && 
                (Position == other.Position);
        }
    }
}
