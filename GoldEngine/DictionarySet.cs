using System;
using System.Linq;

namespace GoldEngine
{
    internal class DictionarySet
    {
        // Fields
        protected SortArray MyList;

        // Methods
        public DictionarySet()
        {
            this.MyList = new SortArray();
        }

        public DictionarySet(DictionarySet Dictionary)
        {
            this.MyList = new SortArray();
            this.MyList.ResizeArray(Dictionary.Count());
            int num2 = Dictionary.Count() - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.MyList.Add(Dictionary[i]);
            }
        }

        public DictionarySet(DictionarySet A, DictionarySet B)
        {
            this.MyList = new SortArray();
            SetOperation operation = this.DoUnion(A, B);
            this.MyList = operation.List;
        }

        public bool Add(DictionarySet SetB)
        {
            return this.UnionWith(SetB);
        }

        public bool Add(params IMember[] Items)
        {
            bool flag2 = false;
            int num2 = Enumerable.Count<IMember>(Items) - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (this.MyList.Add(Items[i]))
                {
                    flag2 = true;
                }
            }
            return flag2;
        }

        public void Clear()
        {
            this.MyList.Clear();
        }

        public Compare CompareTo(DictionarySet SetB)
        {
            int num;
            int num3 = (int)this.MyList.Count();
            int num4 = SetB.Count();
            if (num3 == num4)
            {
                bool flag = false;
                num = 0;
                while ((num < num3) & !flag)
                {
                    if (this.MyList[num].Key().CompareTo(SetB[num].Key()) != 0)
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
            if (num3 < num4)
            {
                num = 0;
                int num2 = 0;
                while ((num < num3) & (num2 < num4))
                {
                    if (this.MyList[num].Key().CompareTo(SetB[num2].Key()) == 0)
                    {
                        num++;
                        num2++;
                    }
                    else
                    {
                        num2++;
                    }
                }
                if (num > (num3 - 1))
                {
                    return Compare.Subset;
                }
                return Compare.UnEqual;
            }
            return Compare.UnEqual;
        }

        public bool Contains(IComparable Key)
        {
            return (this.IndexOf(Key) != -1);
        }

        public void Copy(DictionarySet List)
        {
            this.Clear();
            if (List.Count() > 0)
            {
                int num2 = List.Count() - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.MyList.Add(List[i]);
                }
            }
        }

        public int Count()
        {
            return (int)this.MyList.Count();
        }

        public DictionarySet Difference(DictionarySet SetB)
        {
            DictionarySet set2 = new DictionarySet();
            SetOperation operation = this.DoDifference(this, SetB);
            set2.MyList = operation.List;
            return set2;
        }

        public bool DifferenceWith(DictionarySet SetB)
        {
            int num = (int)this.MyList.Count();
            SetOperation operation = this.DoDifference(this, SetB);
            this.MyList = operation.List;
            return (operation.Changed | (this.MyList.Count() != num));
        }

        private SetOperation DoDifference(DictionarySet SetA, DictionarySet SetB)
        {
            SetOperation operation2 = new SetOperation();
            int num = 0;
            int num2 = 0;
            operation2.Changed = false;
            while ((num < SetA.Count()) | (num2 < SetB.Count()))
            {
                if (num >= SetA.Count())
                {
                    num2++;
                }
                else if (num2 >= SetB.Count())
                {
                    operation2.List.Add(this.MyList[num]);
                    num++;
                }
                else if (SetA[num].Key().CompareTo(SetB[num2].Key()) == 0)
                {
                    MemberResult result = SetA[num].Difference(SetB[num2]);
                    if (result != null)
                    {
                        operation2.List.Add(result.NewObject);
                        if (result.SetChanged)
                        {
                            operation2.Changed = true;
                        }
                    }
                    num++;
                    num2++;
                }
                else if (SetA[num].Key().CompareTo(SetB[num2].Key()) < 0)
                {
                    operation2.List.Add(this.MyList[num]);
                    num++;
                }
                else
                {
                    num2++;
                }
            }
            operation2.List.RemoveNullItems();
            return operation2;
        }

        private SetOperation DoIntersection(DictionarySet SetA, DictionarySet SetB)
        {
            SetOperation operation2 = new SetOperation();
            int num = 0;
            int num2 = 0;
            operation2.Changed = false;
            while ((num < SetA.Count()) & (num2 < SetB.Count()))
            {
                if (SetA[num].Key().CompareTo(SetB[num2].Key()) == 0)
                {
                    MemberResult result = SetA[num].Intersect(SetB[num2]);
                    if (result != null)
                    {
                        operation2.List.Add(result.NewObject);
                        if (result.SetChanged)
                        {
                            operation2.Changed = true;
                        }
                    }
                    num++;
                    num2++;
                }
                else if (SetA[num].Key().CompareTo(SetB[num2].Key()) < 0)
                {
                    num++;
                }
                else
                {
                    num2++;
                }
            }
            operation2.List.RemoveNullItems();
            return operation2;
        }

        private SetOperation DoUnion(DictionarySet SetA, DictionarySet SetB)
        {
            SetOperation operation2 = new SetOperation();
            int num = 0;
            int num2 = 0;
            operation2.Changed = false;
            while ((num < SetA.Count()) | (num2 < SetB.Count()))
            {
                IMember newObject;
                if (num >= SetA.Count())
                {
                    newObject = SetB[num2];
                    num2++;
                }
                else if (num2 >= SetB.Count())
                {
                    newObject = SetA[num];
                    num++;
                }
                else if (SetA[num].Key().CompareTo(SetB[num2].Key()) == 0)
                {
                    MemberResult result = SetA[num].Union(SetB[num2]);
                    if (result == null)
                    {
                        newObject = SetA[num];
                    }
                    else
                    {
                        newObject = result.NewObject;
                        if (result.SetChanged)
                        {
                            operation2.Changed = true;
                        }
                    }
                    num++;
                    num2++;
                }
                else if (SetA[num].Key().CompareTo(SetB[num2].Key()) < 0)
                {
                    newObject = SetA[num];
                    num++;
                }
                else
                {
                    newObject = SetB[num2];
                    num2++;
                }
                operation2.List.Add(newObject);
            }
            operation2.List.RemoveNullItems();
            return operation2;
        }

        public int IndexOf(IComparable Key)
        {
            if (this.MyList.Count() == 0L)
            {
                return -1;
            }
            if (Key.CompareTo(this.MyList[0].Key()) < 0)
            {
                return -1;
            }
            if (Key.CompareTo(this.MyList[(int)(this.MyList.Count() - 1L)].Key()) > 0)
            {
                return -1;
            }
            int num5 = (int)(this.MyList.Count() - 1L);
            int num3 = 0;
            int num = -1;
            bool flag = false;
            do
            {
                int num4 = (int)Math.Round((double)(((double)(num3 + num5)) / 2.0));
                if (num3 > num5)
                {
                    flag = true;
                }
                else if (this.MyList[num4].Key().CompareTo(Key) == 0)
                {
                    num = num4;
                    flag = true;
                }
                else if (this.MyList[num4].Key().CompareTo(Key) < 0)
                {
                    num3 = num4 + 1;
                }
                else if (this.MyList[num4].Key().CompareTo(Key) > 0)
                {
                    num5 = num4 - 1;
                }
            }
            while (!flag);
            return num;
        }

        public DictionarySet Intersection(DictionarySet SetB)
        {
            DictionarySet set2 = new DictionarySet();
            SetOperation operation = this.DoIntersection(this, SetB);
            set2.MyList = operation.List;
            return set2;
        }

        public bool IntersectionWith(DictionarySet SetB)
        {
            int num = (int)this.MyList.Count();
            SetOperation operation = this.DoIntersection(this, SetB);
            this.MyList = operation.List;
            return (operation.Changed | (this.MyList.Count() != num));
        }

        public bool IsEqualSet(DictionarySet SetB)
        {
            bool flag2;
            if (this.MyList.Count() != SetB.Count())
            {
                flag2 = true;
            }
            else
            {
                int num = 0;
                flag2 = false;
                while ((num <= (this.Count() - 1)) & !flag2)
                {
                    if (this.MyList[num].Key().CompareTo(SetB[num].Key()) != 0)
                    {
                        flag2 = true;
                    }
                    num++;
                }
            }
            return !flag2;
        }

        public bool IsProperSubsetOf(DictionarySet SetB)
        {
            int num = 0;
            int num2 = 0;
            while ((num < this.MyList.Count()) & (num2 < SetB.Count()))
            {
                if (this.MyList[num].Key().CompareTo(SetB[num2].Key()) == 0)
                {
                    num++;
                    num2++;
                }
                else
                {
                    num2++;
                }
            }
            if (num < this.MyList.Count())
            {
                return false;
            }
            return true;
        }

        public bool Remove(DictionarySet SetB)
        {
            bool flag = false;
            int index = 0;
            int num2 = 0;
            int num3 = (int)this.MyList.Count();
            while ((index < this.MyList.Count()) | (num2 < SetB.Count()))
            {
                if (index >= this.MyList.Count())
                {
                    num2++;
                }
                else if (num2 >= SetB.Count())
                {
                    index++;
                }
                else if (this.MyList[index].Key().CompareTo(SetB[num2].Key()) == 0)
                {
                    this.MyList.Remove(index);
                    index++;
                    num2++;
                }
                else if (this.MyList[index].Key().CompareTo(SetB[num2].Key()) < 0)
                {
                    index++;
                }
                else
                {
                    num2++;
                }
            }
            this.MyList.RemoveNullItems();
            if (num3 != this.MyList.Count())
            {
                flag = true;
            }
            return flag;
        }

        public void Remove(params IMember[] Item)
        {
            int num3 = Enumerable.Count<IMember>(Item) - 1;
            for (int i = 0; i <= num3; i++)
            {
                int index = this.IndexOf(Item[i].Key());
                if (index != -1)
                {
                    this.MyList.Remove(index);
                }
            }
            this.MyList.RemoveNullItems();
        }

        public override string ToString()
        {
            return Notify.Text;
        }

        public DictionarySet Union(DictionarySet SetB)
        {
            DictionarySet set = new DictionarySet();
            SetOperation operation = this.DoUnion(this, SetB);
            set.MyList = operation.List;
            return set;
        }

        public bool UnionWith(DictionarySet SetB)
        {
            int num = (int)this.MyList.Count();
            SetOperation operation = this.DoUnion(this, SetB);
            this.MyList = operation.List;
            return (operation.Changed | (this.MyList.Count() != num));
        }

        // Properties
        public IMember ByKey(IComparable Key)
        {
            int index = this.IndexOf(Key);
            if ((index >= 0) & (index < this.MyList.Count()))
            {
                return this.MyList[index];
            }
            return null;
        }

        public IMember this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.MyList.Count()))
                {
                    return this.MyList[Index];
                }
                return null;
            }
        }

        // Nested Types
        public enum Compare
        {
            Equal,
            UnEqual,
            Subset
        }

        internal interface IMember
        {
            // Methods
            DictionarySet.MemberResult Difference(DictionarySet.IMember Item);
            DictionarySet.MemberResult Intersect(DictionarySet.IMember Item);
            IComparable Key();
            DictionarySet.MemberResult Union(DictionarySet.IMember Item);
        }

        internal class MemberResult
        {
            // Fields
            public DictionarySet.IMember NewObject;
            public bool SetChanged;

            // Methods
            public MemberResult(DictionarySet.IMember NewObject)
            {
                this.NewObject = NewObject;
                this.SetChanged = false;
            }

            public MemberResult(DictionarySet.IMember NewObject, bool SetChanged)
            {
                this.NewObject = NewObject;
                this.SetChanged = SetChanged;
            }
        }

        private class SetOperation
        {
            // Fields
            public bool Changed = false;
            public DictionarySet.SortArray List = new DictionarySet.SortArray();
        }

        internal class SortArray
        {
            // Fields
            private int m_ArraySize = 0;
            private short m_BlockSize = 100;
            private int m_Count = 0;
            private DictionarySet.IMember[] m_List;

            // Methods
            public SortArray()
            {
                this.ResizeArray(this.m_Count);
            }

            public bool Add(DictionarySet.IMember Item)
            {
                bool setChanged = false;
                InsertIndexInfo info = this.InsertIndex(Item.Key());
                if (info.Found > 0)
                {
                    DictionarySet.MemberResult result = this.m_List[info.Index].Union(Item);
                    if ((result != null) && (result.NewObject != null))
                    {
                        this.m_List[info.Index] = result.NewObject;
                        setChanged = result.SetChanged;
                    }
                    return setChanged;
                }
                setChanged = true;
                this.ResizeArray(this.m_Count + 1);
                int num2 = info.Index + 1;
                for (int i = this.m_Count - 1; i >= num2; i += -1)
                {
                    this.m_List[i] = this.m_List[i - 1];
                }
                this.m_List[info.Index] = Item;
                return setChanged;
            }

            public void Clear()
            {
                this.m_ArraySize = 0;
                this.m_Count = 0;
                this.m_List = new DictionarySet.IMember[2];
            }

            public long Count()
            {
                return (long)this.m_Count;
            }

            private InsertIndexInfo InsertIndex(IComparable Key)
            {
                InsertIndexInfo info2 = new InsertIndexInfo();
                info2.Found = 0;
                if (this.m_Count == 0)
                {
                    info2.Index = 0;
                    return info2;
                }
                if (Key.CompareTo(this.m_List[0].Key()) < 0)
                {
                    info2.Index = 0;
                    return info2;
                }
                if (Key.CompareTo(this.m_List[this.m_Count - 1].Key()) > 0)
                {
                    info2.Index = this.m_Count;
                    return info2;
                }
                int num3 = this.m_Count - 1;
                int num = 0;
                bool flag = false;
                do
                {
                    int index = (int)Math.Round((double)(((double)(num + num3)) / 2.0));
                    if (num > num3)
                    {
                        info2.Index = num;
                        flag = true;
                    }
                    else if (this.m_List[index].Key().CompareTo(Key) == 0)
                    {
                        info2.Index = index;
                        info2.Found = -1;
                        flag = true;
                    }
                    else if (this.m_List[index].Key().CompareTo(Key) < 0)
                    {
                        num = index + 1;
                    }
                    else
                    {
                        num3 = index - 1;
                    }
                }
                while (!flag);
                return info2;
            }

            internal void Remove(int Index)
            {
                if ((Index >= 0) & (Index < this.m_Count))
                {
                    this.m_List[Index] = null;
                }
            }

            internal void RemoveNullItems()
            {
                int index = 0;
                int num2 = 0;
                while (index < this.m_Count)
                {
                    if (this.m_List[index] == null)
                    {
                        index++;
                    }
                    else if (num2 == index)
                    {
                        num2++;
                        index++;
                    }
                    else
                    {
                        this.m_List[num2] = this.m_List[index];
                        num2++;
                        index++;
                    }
                }
                this.ResizeArray(num2);
            }

            public void ResizeArray(int RequiredSize)
            {
                if ((RequiredSize > this.m_ArraySize) | (Math.Abs((int)(RequiredSize - this.m_ArraySize)) > this.m_BlockSize))
                {
                    this.m_ArraySize = ((RequiredSize / this.m_BlockSize) + 1) * this.m_BlockSize;
                    this.m_List = (DictionarySet.IMember[])Utils.CopyArray((Array)this.m_List, new DictionarySet.IMember[(this.m_ArraySize - 1) + 1]);
                }
                this.m_Count = RequiredSize;
            }

            // Properties
            public short Blocksize
            {
                get
                {
                    return this.m_BlockSize;
                }
                set
                {
                    if (value >= 1)
                    {
                        this.m_BlockSize = value;
                    }
                    else
                    {
                        this.m_BlockSize = 1;
                    }
                }
            }

            public DictionarySet.IMember this[int Index]
            {
                get
                {
                    if ((Index >= 0) & (Index < this.m_Count))
                    {
                        return this.m_List[Index];
                    }
                    return null;
                }
            }

            // Nested Types
            private class InsertIndexInfo
            {
                // Fields
                public int Found;
                public int Index;
            }
        }
    }
}