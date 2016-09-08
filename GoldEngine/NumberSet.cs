using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldEngine
{
    internal class NumberSet
    {
        private int m_arraySize;
        private short m_blockSize;
        private int m_count;
        private int[] m_list;

        public NumberSet(params int[] numbers)
        {
            m_blockSize = 0x20;
            m_arraySize = 0;
            m_blockSize = 0x10;
            m_count = 0;
            int upperBound = numbers.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                AddItem(numbers[i]);
            }
        }

        public NumberSet(NumberSet numbers)
        {
            m_blockSize = 0x20;
            m_count = numbers.Count();
            m_blockSize = 0x10;
            ResizeArray(m_count);
            int num2 = numbers.Count() - 1;
            for (int i = 0; i <= num2; i++)
            {
                m_list[i] = numbers[i];
            }
        }

        public void Add(NumberSet numbers)
        {
            UnionWith(numbers);
        }

        public void Add(params int[] numbers)
        {
            int upperBound = numbers.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                AddItem(numbers[i]);
            }
        }

        private void AddItem(int number)
        {
            int index = InsertIndex(number);
            if (index != -1)
            {
                m_count++;
                ResizeArray(m_count);
                int num3 = index + 1;
                for (int i = m_count - 1; i >= num3; i += -1)
                {
                    m_list[i] = m_list[i - 1];
                }
                m_list[index] = number;
            }
        }

        public void AddRange(int startValue, int endValue)
        {
            int num;
            if (startValue > endValue)
            {
                Swap(startValue, endValue);
            }
            int num2 = (endValue - startValue) + 1;
            if (m_count == 0)
            {
                m_count = num2;
                ResizeArray(m_count);
                int num3 = m_count - 1;
                for (num = 0; num <= num3; num++)
                {
                    m_list[num] = num + startValue;
                }
            }
            else if (m_list[m_count - 1] < startValue)
            {
                ResizeArray(m_count + num2);
                int num4 = num2 - 1;
                for (num = 0; num <= num4; num++)
                {
                    m_list[m_count + num] = startValue + num;
                }
                m_count += num2;
            }
            else
            {
                int num5 = endValue;
                for (num = startValue; num <= num5; num++)
                {
                    AddItem(num);
                }
            }
        }

        public void Clear()
        {
            m_list = null;
            m_arraySize = 0;
            m_count = 0;
        }

        public Compare CompareTo(NumberSet setB)
        {
            int num;
            int count = m_count;
            int num4 = setB.Count();
            if (count == num4)
            {
                bool flag = false;
                num = 0;
                while ((num < count) & !flag)
                {
                    if (m_list[num] != setB[num])
                    {
                        flag = true;
                    }
                    else
                    {
                        num++;
                    }
                }
                if (flag)
                {
                    return Compare.UnEqual;
                }
                return Compare.Equal;
            }
            if (count < num4)
            {
                num = 0;
                int num2 = 0;
                while ((num < count) & (num2 < num4))
                {
                    if (m_list[num] == setB[num2])
                    {
                        num++;
                        num2++;
                    }
                    else
                    {
                        num2++;
                    }
                }
                if (num > (count - 1))
                {
                    return Compare.Subset;
                }
                return Compare.UnEqual;
            }
            return Compare.UnEqual;
        }

        public bool Contains(int number) =>
            (ItemIndex(number) != -1);

        public void Copy(NumberSet list)
        {
            Clear();
            if (list.Count() > 0)
            {
                m_count = list.Count();
                ResizeArray(m_count);
                int num2 = list.Count() - 1;
                for (int i = 0; i <= num2; i++)
                {
                    m_list[i] = list[i];
                }
            }
        }

        public int Count() =>
            m_count;

        public NumberSet Difference(NumberSet setB)
        {
            NumberSet set2 = new NumberSet();
            int index = 0;
            int num2 = 0;
            while ((index < m_count) | (num2 < setB.Count()))
            {
                if (index >= m_count)
                {
                    num2++;
                }
                else if (num2 >= setB.Count())
                {
                    set2.Add(m_list[index]);
                    index++;
                }
                else if (m_list[index] == setB[num2])
                {
                    index++;
                    num2++;
                }
                else if (m_list[index] < setB[num2])
                {
                    set2.Add(m_list[index]);
                    index++;
                }
                else
                {
                    num2++;
                }
            }
            return set2;
        }

        public bool DifferenceWith(NumberSet setB)
        {
            int index = 0;
            int num2 = 0;
            int num3 = 0;
            while ((index < m_count) | (num2 < setB.Count()))
            {
                if (index >= m_count)
                {
                    num2++;
                }
                else if (num2 >= setB.Count())
                {
                    m_list[num3] = m_list[index];
                    num3++;
                    index++;
                }
                else if (m_list[index] == setB[num2])
                {
                    index++;
                    num2++;
                }
                else if (m_list[index] < setB[num2])
                {
                    m_list[num3] = m_list[index];
                    num3++;
                    index++;
                }
                else
                {
                    num2++;
                }
            }
            bool flag = m_count != num3;
            m_count = num3;
            ResizeArray(m_count);
            return flag;
        }

        private string HexByte(int value)
        {
            string str2 = $"{value:X}";
            if ((str2.Length % 2) == 1)
            {
                str2 = "0" + str2;
            }
            return str2;
        }

        private int InsertIndex(int number)
        {
            if (m_count == 0)
            {
                return 0;
            }
            if (number < m_list[0])
            {
                return 0;
            }
            if (number > m_list[m_count - 1])
            {
                return m_count;
            }
            int num5 = m_count - 1;
            int num3 = 0;
            int num = -1;
            bool flag = false;
            do
            {
                int index = (int)Math.Round((double)(((double)(num3 + num5)) / 2.0));
                if (num3 > num5)
                {
                    num = num3;
                    flag = true;
                }
                else if (m_list[index] == number)
                {
                    num = -1;
                    flag = true;
                }
                else if (m_list[index] < number)
                {
                    num3 = index + 1;
                }
                else if (m_list[index] > number)
                {
                    num5 = index - 1;
                }
            } while (!flag);
            return num;
        }

        public NumberSet Intersection(NumberSet setB)
        {
            NumberSet set2 = new NumberSet();
            int index = 0;
            int num2 = 0;
            while ((index < m_count) & (num2 < setB.Count()))
            {
                if (m_list[index] == setB[num2])
                {
                    set2.Add(m_list[index]);
                    index++;
                    num2++;
                }
                else if (m_list[index] < setB[num2])
                {
                    index++;
                }
                else
                {
                    num2++;
                }
            }
            return set2;
        }

        public bool IsEqualSet(NumberSet setB)
        {
            if (m_count != setB.Count())
            {
                return false;
            }
            int index = 0;
            bool flag2 = false;
            while ((index <= (Count() - 1)) & !flag2)
            {
                if (m_list[index] != setB[index])
                {
                    flag2 = true;
                }
                index++;
            }
            return !flag2;
        }

        public bool IsProperSubsetOf(NumberSet setB)
        {
            int index = 0;
            int num2 = 0;
            while ((index < m_count) & (num2 < setB.Count()))
            {
                if (m_list[index] == setB[num2])
                {
                    index++;
                    num2++;
                }
                else
                {
                    num2++;
                }
            }
            if (index < m_count)
            {
                return false;
            }
            return true;
        }

        private int ItemIndex(int number)
        {
            if (m_count == 0)
            {
                return -1;
            }
            if ((number < m_list[0]) | (number > m_list[m_count - 1]))
            {
                return -1;
            }
            int num5 = m_count - 1;
            int num3 = 0;
            int num = -1;
            bool flag = false;
            do
            {
                int index = (int)Math.Round((double)(((double)(num3 + num5)) / 2.0));
                if (num3 > num5)
                {
                    flag = true;
                }
                else if (m_list[index] == number)
                {
                    num = index;
                    flag = true;
                }
                else if (m_list[index] < number)
                {
                    num3 = index + 1;
                }
                else if (m_list[index] > number)
                {
                    num5 = index - 1;
                }
            } while (!flag);
            return num;
        }

        private string RangeItemText(int first, int last, string rangeChars, string numberPrefix = "", bool hexFormat = false)
        {
            if (first == last)
            {
                if (hexFormat)
                {
                    return (numberPrefix + HexByte(first));
                }
                return (numberPrefix + first.ToString());
            }
            if (hexFormat)
            {
                return (numberPrefix + HexByte(first) + rangeChars + numberPrefix + HexByte(last));
            }
            return (numberPrefix + first.ToString() + rangeChars + numberPrefix + last.ToString());
        }

        public NumberRangeList RangeList()
        {
            NumberRangeList list2 = new NumberRangeList();
            if (m_count >= 1)
            {
                int first = m_list[0];
                int last = first;
                int num5 = m_count - 1;
                for (int i = 1; i <= num5; i++)
                {
                    int num4 = m_list[i];
                    if (num4 != (last + 1))
                    {
                        list2.Add(new NumberRange(first, last));
                        first = num4;
                    }
                    last = num4;
                }
                list2.Add(new NumberRange(first, last));
            }
            return list2;
        }

        public string RangeText(string rangeChars, string separator = ",", string prefix = "", bool hexFormat = false)
        {
            string str2 = "";
            if (m_count < 1)
            {
                return str2;
            }
            int first = m_list[0];
            int last = first;
            int num5 = m_count - 1;
            for (int i = 1; i <= num5; i++)
            {
                int num4 = m_list[i];
                if (num4 != (last + 1))
                {
                    str2 = str2 + RangeItemText(first, last, rangeChars, prefix, hexFormat) + separator;
                    first = num4;
                }
                last = num4;
            }
            return (str2 + RangeItemText(first, last, rangeChars, prefix, hexFormat));
        }

        public void Remove(params int[] numbers)
        {
            int upperBound = numbers.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                int num = ItemIndex(numbers[i]);
                if (num != -1)
                {
                    int num5 = m_count - 1;
                    for (int j = num + 1; j <= num5; j++)
                    {
                        m_list[j - 1] = m_list[j];
                    }
                    m_count--;
                    ResizeArray(m_count);
                }
            }
        }

        public void Remove(NumberSet numbers)
        {
            DifferenceWith(numbers);
        }

        private void ResizeArray(int requiredSize)
        {
            if ((requiredSize > m_arraySize) | (Math.Abs((int)(requiredSize - m_arraySize)) > m_blockSize))
            {
                m_arraySize = ((requiredSize / m_blockSize) + 1) * m_blockSize;
                m_list = (int[])Utils.CopyArray((Array)m_list, new int[(m_arraySize - 1) + 1]);
            }
        }

        private void Swap(int a, int b)
        {
            int num = a;
            a = b;
            b = num;
        }

        public string Text(string separator = ", ")
        {
            string str = "";
            if (m_count >= 1)
            {
                str = m_list[0].ToString();
                int num2 = m_count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    str = str + separator + m_list[i].ToString();
                }
            }
            return str;
        }

        public override string ToString()
        {
            string separator = ", ";
            return Text(separator);
        }

        public NumberSet Union(NumberSet setB)
        {
            NumberSet set = new NumberSet();
            int index = 0;
            int num2 = 0;
            while ((index < m_count) | (num2 < setB.Count()))
            {
                int num3;
                if (index >= m_count)
                {
                    num3 = setB[num2];
                    num2++;
                }
                else if (num2 >= setB.Count())
                {
                    num3 = m_list[index];
                    index++;
                }
                else if (m_list[index] == setB[num2])
                {
                    num3 = m_list[index];
                    index++;
                    num2++;
                }
                else if (m_list[index] < setB[num2])
                {
                    num3 = m_list[index];
                    index++;
                }
                else
                {
                    num3 = setB[num2];
                    num2++;
                }
                set.Add(num3);
            }
            return set;
        }

        public bool UnionWith(NumberSet setB)
        {
            int index = 0;
            int num2 = 0;
            bool flag = false;
            int num4 = 0;
            int[] numArray = new int[(m_count + setB.Count()) + 1];
            while ((index < m_count) | (num2 < setB.Count()))
            {
                int num3;
                if (index >= m_count)
                {
                    num3 = setB[num2];
                    num2++;
                    flag = true;
                }
                else if (num2 >= setB.Count())
                {
                    num3 = m_list[index];
                    index++;
                }
                else if (m_list[index] == setB[num2])
                {
                    num3 = m_list[index];
                    index++;
                    num2++;
                }
                else if (m_list[index] < setB[num2])
                {
                    num3 = m_list[index];
                    index++;
                }
                else
                {
                    num3 = setB[num2];
                    num2++;
                    flag = true;
                }
                num4++;
                numArray[num4 - 1] = num3;
            }
            if (flag)
            {
                m_count = num4;
                m_list = new int[(m_count - 1) + 1];
                int num5 = m_count - 1;
                for (index = 0; index <= num5; index++)
                {
                    m_list[index] = numArray[index];
                }
            }
            return flag;
        }

        public short Blocksize
        {
            get { return m_blockSize; }
            set
            {
                if (value >= 1)
                {
                    m_blockSize = value;
                }
                else
                {
                    m_blockSize = 1;
                }
            }
        }

        public int this[int index]
        {
            get
            {
                int num = 0;
                if ((index >= 0) & (index < m_count))
                {
                    num = m_list[index];
                }
                return num;
            }
            set
            {
                if ((index >= 0) & (index < m_count))
                {
                    m_list[index] = value;
                }
            }
        }

        public enum Compare
        {
            Equal,
            UnEqual,
            Subset
        }
    }
}

