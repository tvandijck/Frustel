namespace GoldEngine
{
    internal class NumberRange
    {
        // Fields
        private readonly int m_first;
        private readonly int m_last;

        // Methods
        public NumberRange()
        {
            m_first = 0;
            m_last = 0;
        }

        public NumberRange(int First, int Last)
        {
            if (m_first <= m_last)
            {
                m_first = First;
                m_last = Last;
            }
            else
            {
                m_first = Last;
                m_last = First;
            }
        }

        internal NumberRangeCompare Compare(NumberRange B)
        {
            return Compare(B.First, B.Last);
        }

        internal NumberRangeCompare Compare(int First, int Last)
        {
            if ((m_first < First) & (m_last > Last))
            {
                return NumberRangeCompare.Superset;
            }
            if ((First < m_first) & (Last > m_last))
            {
                return NumberRangeCompare.Subset;
            }
            if (m_last < First)
            {
                return NumberRangeCompare.LessThanDisjoint;
            }
            if ((m_first < First) & (m_last < Last))
            {
                return NumberRangeCompare.LessThanOverlap;
            }
            if (Last < m_first)
            {
                return NumberRangeCompare.GreaterThanDisjoint;
            }
            if ((First < m_first) & (Last < m_last))
            {
                return NumberRangeCompare.GreaterThanOverlap;
            }
            return NumberRangeCompare.Subset;
        }

        internal NumberRangeRelation Relation(int First, int Last)
        {
            if ((m_last < First) | (m_first > Last))
            {
                return NumberRangeRelation.Disjoint;
            }
            if ((m_first >= First) & (m_last <= Last))
            {
                return NumberRangeRelation.Subset;
            }
            if ((m_first < First) & (m_last > Last))
            {
                return NumberRangeRelation.Superset;
            }
            return NumberRangeRelation.Overlap;
        }

        public int First
        {
            get { return m_first; }
        }

        public int Last
        {
            get { return m_last; }
        }
    }
}