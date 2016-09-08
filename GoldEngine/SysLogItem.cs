namespace GoldEngine
{
    public class SysLogItem
    {
        // Fields
        public SysLogAlert Alert;
        public string Description;
        public string Index;
        public SysLogSection Section;
        public string Title;

        // Methods
        public string AlertName()
        {
            switch (((int)this.Alert))
            {
            case 1:
                return "Success";

            case 2:
                return "Warning";

            case 3:
                return "Error";
            }
            return "Details";
        }

        public string SectionName()
        {
            switch (((int)this.Section))
            {
            case -1:
                return "Internal";

            case 0:
                return "System";

            case 1:
                return "Grammar";

            case 2:
                return "DFA States";

            case 3:
                return "LALR States";

            case 4:
                return "Input";
            }
            return "(Unspecified)";
        }
    }
}