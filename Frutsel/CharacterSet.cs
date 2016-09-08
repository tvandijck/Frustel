using System.Collections;
using System.Collections.Generic;

namespace Frutsel
{
    public class CharacterSet : IEnumerable<char>
    {
        private readonly HashSet<char> m_set = new HashSet<char>();

        public CharacterSet()
        {
        }

        public CharacterSet(char c)
        {
            m_set.Add(c);
        }

        public CharacterSet(string s)
        {
            foreach (char c in s)
            {
                m_set.Add(c);
            }
        }

        public CharacterSet(IEnumerable<char> s)
        {
            foreach (char c in s)
            {
                m_set.Add(c);
            }
        }

        public CharacterSet(char start, char end)
        {
            for (char c = start; c <= end; ++c)
            {
                m_set.Add(c);
            }
        }

        public int Count
        {
            get { return m_set.Count; }
        }

        public bool Contains(char c)
        {
            return m_set.Contains(c);
        }

        public void Add(char c)
        {
            m_set.Add(c);
        }

        public void Add(IEnumerable<char> other)
        {
            if (ReferenceEquals(other, null))
                return;

            foreach (char c in other)
            {
                m_set.Add(c);
            }
        }

        public void Subtract(char c)
        {
            m_set.Remove(c);
        }

        public void Subtract(IEnumerable<char> other)
        {
            if (ReferenceEquals(other, null))
                return;

            foreach (char c in other)
            {
                m_set.Remove(c);
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var res = string.Join("", m_set);
            return res;
        }

        public static CharacterSet operator +(CharacterSet left, IEnumerable<char> right)
        {
            var result = new CharacterSet(left);
            result.Add(right);
            return result;
        }

        public static CharacterSet operator +(CharacterSet left, char right)
        {
            var result = new CharacterSet(left);
            result.Add(right);
            return result;
        }

        public static CharacterSet operator -(CharacterSet left, IEnumerable<char> right)
        {
            var result = new CharacterSet(left);
            result.Subtract(right);
            return result;
        }

        public static CharacterSet operator -(CharacterSet left, char right)
        {
            var result = new CharacterSet(left);
            result.Subtract(right);
            return result;
        }
    }
}