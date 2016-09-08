namespace GoldEngine
{
    internal class GroupBuildList : GroupList
    {
        // Methods
        public GroupBuildList()
        {
        }

        internal GroupBuildList(int Size) : base(Size)
        {
        }

        public int Add(GroupBuild Item)
        {
            return base.Add(Item);
        }

        public GroupBuild AddUnique(GroupBuild Grp)
        {
            int num = this.ItemIndex(Grp.Name);
            if (num == -1)
            {
                base.Add(Grp);
                return Grp;
            }
            return (GroupBuild)base[num];
        }

        public GroupBuild AddUnique(string Name)
        {
            int num = this.ItemIndex(Name);
            if (num == -1)
            {
                GroupBuild item = new GroupBuild
                {
                    Name = Name
                };
                base.Add(item);
                return item;
            }
            return (GroupBuild)base[num];
        }

        public int ItemIndex(string Name)
        {
            int num = -1;
            for (int i = 0; (i < base.Count) & (num == -1); i++)
            {
                GroupBuild build = (GroupBuild)base[i];
                if (build.Name.ToUpper() == Name.ToUpper())
                {
                    num = i;
                }
            }
            return num;
        }

        public override string ToString()
        {
            string name = "";
            if (base.Count >= 1)
            {
                name = this[0].Name;
                int num2 = base.Count - 1;
                for (int i = 1; i <= num2; i++)
                {
                    name = name + ", " + this[i].Name;
                }
            }
            return name;
        }

        // Properties
        public new GroupBuild this[int Index]
        {
            get
            {
                return (GroupBuild)base[Index];
            }
            set
            {
                base[Index] = value;
            }
        }
    }
}