using System;

namespace GoldEngine
{
    public class SysLog
    {
        // Fields
        private SysLogItem[] m_Array;
        private short m_Count;
        private bool m_Locked;

        // Methods
        public void Add(SysLogItem Item)
        {
            if (!this.Locked)
            {
                this.m_Count = (short)(this.m_Count + 1);
                this.m_Array = (SysLogItem[])Utils.CopyArray((Array)this.m_Array, new SysLogItem[(this.m_Count - 1) + 1]);
                this.m_Array[this.m_Count - 1] = Item;
            }
        }

        public void Add(string Title, string Description)
        {
            if (!this.Locked)
            {
                SysLogItem item = new SysLogItem();
                SysLogItem item2 = item;
                item2.Section = SysLogSection.Grammar;
                item2.Alert = SysLogAlert.Warning;
                item2.Title = Title;
                item2.Description = Description;
                item2 = null;
                this.Add(item);
            }
        }

        public void Add(SysLogSection Section, SysLogAlert Alert, string Title)
        {
            if (!this.Locked)
            {
                SysLogItem item = new SysLogItem();
                SysLogItem item2 = item;
                item2.Section = Section;
                item2.Alert = Alert;
                item2.Title = Title;
                item2 = null;
                this.Add(item);
            }
        }

        public void Add(SysLogSection Section, SysLogAlert Alert, string Title, string Description, string Index = "")
        {
            if (!this.Locked)
            {
                SysLogItem item = new SysLogItem();
                SysLogItem item2 = item;
                item2.Section = Section;
                item2.Alert = Alert;
                item2.Title = Title;
                item2.Description = Description;
                item2.Index = Index;
                if ((item2.Description != "") & !item2.Description.EndsWith("."))
                {
                    SysLogItem item3 = item2;
                    item3.Description = item3.Description + ".";
                }
                item2 = null;
                this.Add(item);
            }
        }

        public int AlertCount(SysLogAlert Alert)
        {
            int num3 = 0;
            int num4 = this.m_Count - 1;
            for (int i = 0; i <= num4; i++)
            {
                if (this.m_Array[i].Alert == Alert)
                {
                    num3++;
                }
            }
            return num3;
        }

        public void Clear()
        {
            this.m_Array = null;
            this.m_Count = 0;
        }

        public int Count()
        {
            return this.m_Count;
        }

        // Properties
        public SysLogItem this[int Index]
        {
            get
            {
                if ((Index >= 0) & (Index < this.m_Count))
                {
                    return this.m_Array[Index];
                }
                return null;
            }
            set
            {
                if ((Index >= 0) & (Index < this.m_Count))
                {
                    this.m_Array[Index] = value;
                }
            }
        }

        public bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
            }
        }
    }
}