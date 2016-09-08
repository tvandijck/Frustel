using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Frutsel
{
    public sealed class Expression : IEnumerable<Sequence>
    {
        private readonly List<Sequence> m_sequences = new List<Sequence>();

        public void Add(Sequence item)
        {
            m_sequences.Add(item);
        }

        public void Add(string token)
        {
            var sequence = new Sequence();
            foreach (var c in token)
            {
                sequence.Add(EKleene.None, new CharacterSet(c));
            }
            m_sequences.Add(sequence);
        }

        public IEnumerator<Sequence> GetEnumerator()
        {
            return m_sequences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(" | ", m_sequences);
        }
    }
}