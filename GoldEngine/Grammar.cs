using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace GoldEngine
{
    internal sealed class Grammar
    {
        // Fields
        internal static GrammarAttribAssignList GroupAttributes = new GrammarAttribAssignList();
        internal static GrammarGroupList Groups = new GrammarGroupList();
        internal static GrammarSymbolList HandleSymbols = new GrammarSymbolList();
        internal static GrammarProductionList Productions = new GrammarProductionList();
        internal static VariableList Properties = new VariableList();
        internal static GrammarAttribAssignList SymbolAttributes = new GrammarAttribAssignList();
        internal static GrammarSymbolList TerminalDefs = new GrammarSymbolList();
        internal static GrammarIdentifierList UsedSetNames = new GrammarIdentifierList();
        internal static GrammarSetList UserSets = new GrammarSetList();

        // Methods
        internal static void AddGroup(GrammarGroup G)
        {
            int num = Groups.ItemIndex(G.Name);
            if (num == -1)
            {
                Groups.Add(G);
            }
            else
            {
                GrammarGroup group = Groups[num];
                if ((G.Start != "") & (group.Start == ""))
                {
                    group.Start = G.Start;
                }
                else if ((G.End != "") & (group.End == ""))
                {
                    group.End = G.End;
                }
                else
                {
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Duplicate group assignment", " The attributes for the group '" + G.Name + " were reassigned.", Conversions.ToString(G.Line));
                }
            }
        }

        internal static void AddGroupAttrib(GrammarAttribAssign Assign)
        {
            if (GroupAttributes.ItemIndex(Assign) == -1)
            {
                GroupAttributes.Add(Assign);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Duplicate group attribute assignment", "The attributes for the group '" + Assign.Name + "' were set previously.", Conversions.ToString(Assign.Line));
            }
        }

        internal static bool AddHandleSymbol(GrammarSymbol Sym)
        {
            bool flag2 = HandleSymbols.ItemIndex(Sym) != -1;
            if (!flag2)
            {
                HandleSymbols.Add(Sym);
            }
            return !flag2;
        }

        internal static void AddProduction(GrammarProduction Prod)
        {
            if (Productions.ItemIndex(Prod) == -1)
            {
                Productions.Add(Prod);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Duplicate production", "The production '" + Prod.Head.Name + "' was redefined.", Conversions.ToString(Prod.Line));
            }
        }

        internal static void AddProperty(string Name, string Value, int Line)
        {
            short num = (short)Properties.ItemIndex(Name);
            if (num == -1)
            {
                Properties.Add(Name, Value);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Duplicate property assignment", "The property \"" + Name + "\" was reassigned.", Conversions.ToString(Line));
            }
        }

        internal static void AddSymbolAttrib(GrammarAttribAssign Assign)
        {
            if (SymbolAttributes.ItemIndex(Assign) == -1)
            {
                SymbolAttributes.Add(Assign);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Duplicate symbol attribute assignment", "The attributes for the symbol '" + Assign.Name + "' were set previously.", Conversions.ToString(Assign.Line));
            }
        }

        internal static bool AddTerminalHead(GrammarSymbol Sym)
        {
            short num = TerminalDefs.ItemIndex(Sym);
            bool flag2 = num != -1;
            if (num == -1)
            {
                TerminalDefs.Add(Sym);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Duplicate definition for the terminal '" + Sym.Name + "'", "", Conversions.ToString(Sym.Line));
            }
            return !flag2;
        }

        internal static void AddUsedSetName(string Name, int Line)
        {
            int num = UsedSetNames.ItemIndex(Name);
            if (!BuilderApp.IsPredefinedSet(Name) & (num == -1))
            {
                UsedSetNames.Add(new GrammarIdentifier(Name, Line));
            }
        }

        internal static void AddUserSet(GrammarSet CharSet)
        {
            if (BuilderApp.IsPredefinedSet(CharSet.Name))
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Predined Set", "The set {" + CharSet.Name + "} is a set built into GOLD.", Conversions.ToString(CharSet.Line));
            }
            else if (UserSets.ItemIndex(CharSet.Name) != -1)
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Set redefined", "The set {" + CharSet.Name + "} was redefined", Conversions.ToString(CharSet.Line));
            }
            else if (BuilderApp.UserDefinedSets.ItemIndex(CharSet.Name) == -1)
            {
                UserSets.Add(CharSet);
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Duplicate set definition", "The set '" + CharSet.Name + "' was previously defined.", Conversions.ToString(CharSet.Line));
            }
        }

        public static void Build(TextReader Reader)
        {
            Clear();
            GrammarParse.Parse(Reader);
        }

        private static void CheckGroupMissingStartEnd()
        {
            IEnumerator enumerator;
            try
            {
                enumerator = BuilderApp.BuildTables.Group.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GroupBuild current = (GroupBuild)enumerator.Current;
                    if ((current.Start != null) & (current.End == null))
                    {
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Definition for '" + current.Name + " Start' is missing a matching '" + current.Name + " End'");
                    }
                    else if ((current.Start == null) & (current.End != null))
                    {
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Definition for '" + current.Name + " End' is missing a matching '" + current.Name + " Start'");
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        public static void Clear()
        {
            UserSets.Clear();
            UsedSetNames.Clear();
            Properties.Clear();
            Groups.Clear();
            TerminalDefs.Clear();
            HandleSymbols.Clear();
            Productions.Clear();
            SymbolAttributes.Clear();
            GroupAttributes.Clear();
        }

        internal static int GetDecValue(string Text)
        {
            int num2 = -1;
            try
            {
                string expression = Text.Substring(1);
                if (!Versioned.IsNumeric(expression))
                {
                    return num2;
                }
                int num3 = (int)Math.Round(Conversion.Val(expression));
                if ((num3 <= 0xffffL) & (num3 >= 0))
                {
                    num2 = num3;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
            }
            return num2;
        }

        internal static int GetHexValue(string Text)
        {
            long num4;
            bool flag = false;
            try
            {
                int num5;
                int num6;
                string source = Text.Substring(1);
                int num2 = 1;
                num4 = 0L;
                int num3 = source.Count<char>() - 1;
                goto Label_0051;
                Label_001E:
                num5 = HexDigitValue(source[num3]);
                if (num5 == -1)
                {
                    flag = true;
                }
                num4 += num5 * num2;
                num2 *= 0x10;
                num3 += -1;
                Label_0051:
                num6 = 0;
                if (num3 >= num6)
                {
                    goto Label_001E;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = true;
            }
            if (flag)
            {
                return -1;
            }
            return (int)num4;
        }

        public static BuilderApp.CharSetMode GetParamCharSet()
        {
            switch (Properties["Character Set"].Value)
            {
            case "UNICODE":
                return BuilderApp.CharSetMode.Unicode;

            case "ANSI":
                return BuilderApp.CharSetMode.ANSI;
            }
            return BuilderApp.CharSetMode.Invalid;
        }

        private static int HexDigitValue(char Digit)
        {
            switch (((char)(Digit - '0')))
            {
            case '\0':
            case '\x0001':
            case '\x0002':
            case '\x0003':
            case '\x0004':
            case '\x0005':
            case '\x0006':
            case '\a':
            case '\b':
            case '\t':
                return Conversion.Val(Digit);

            case '\x0011':
            case '1':
                return 10;

            case '\x0012':
            case '2':
                return 11;

            case '\x0013':
            case '3':
                return 12;

            case '\x0014':
            case '4':
                return 13;

            case '\x0015':
            case '5':
                return 14;

            case '\x0016':
            case '6':
                return 15;
            }
            return -1;
        }

        internal static bool IsValidPropertyName(string Name)
        {
            switch (Name.ToUpper())
            {
            case "NAME":
            case "VERSION":
            case "ABOUT":
            case "AUTHOR":
            case "START SYMBOL":
                return true;

            case "AUTO WHITESPACE":
            case "CHARACTER MAPPING":
            case "CASE SENSITIVE":
            case "VIRTUAL TERMINALS":
                return true;
            }
            return false;
        }

        internal static string RemoveMultiSpaces(string Text)
        {
            string str2 = "";
            bool flag = false;
            int num2 = Text.Count<char>() - 1;
            for (int i = 0; i <= num2; i++)
            {
                char ch = Text[i];
                if (Conversions.ToString(ch) == " ")
                {
                    if (flag)
                    {
                        str2 = str2 + " ";
                    }
                    flag = false;
                }
                else
                {
                    str2 = str2 + Conversions.ToString(ch);
                    flag = true;
                }
            }
            return str2;
        }

        internal static string SetLiteral(string Source)
        {
            string str2 = "";
            bool flag = false;
            string str3 = "";
            short num2 = (short)(Source.Count<char>() - 1);
            for (short i = 0; i <= num2; i = (short)(i + 1))
            {
                string str = Conversions.ToString(Source[i]);
                if (flag)
                {
                    if (str == "'")
                    {
                        if (str2 == "")
                        {
                            str = "'";
                        }
                        else
                        {
                            str = str2;
                        }
                        flag = false;
                    }
                    else
                    {
                        str2 = str2 + str;
                        str = "";
                    }
                }
                else if (str == "'")
                {
                    flag = true;
                    str2 = "";
                    str = "";
                }
                str3 = str3 + str;
            }
            return str3;
        }

        internal static string TokenText(string Text)
        {
            string source = "";
            if (Text.Length >= 2)
            {
                source = Text.Substring(1, Text.Length - 2);
                switch (Text[0])
                {
                case '"':
                    return source;

                case '[':
                    return SetLiteral(source);

                case '<':
                    return RemoveMultiSpaces(source);

                case '\'':
                    if (Text == "''")
                    {
                        return "'";
                    }
                    return RemoveMultiSpaces(source);
                }
                return Text;
            }
            return Text;
        }

        internal static void WarnRegexSetLiteral(string FullText, int CurrentLineNumber)
        {
            string source = TokenText(FullText);
            if (LikeOperator.LikeString(source, "*[a-zA-Z]-[a-zA-Z]*", CompareMethod.Binary) | LikeOperator.LikeString(source, "*#-#*", CompareMethod.Binary))
            {
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "The set literal " + FullText + " does not represent a set range", "The GOLD Builder interpreters sets like [A-Z] as three characters: 'A', '-', and 'Z'. Set ranges are supported, but use a different notation. Please consult the documentation.", Conversions.ToString(CurrentLineNumber));
            }
        }

        // Nested Types
        internal class GrammarGroup : Grammar.GrammarParseItem
        {
            // Fields
            public string Container;
            public string End;
            public bool IsBlock;
            public string Name;
            public string Start;

            // Methods
            public GrammarGroup()
            {
            }

            public GrammarGroup(string Name, string Usage, string Value, int Line)
            {
                this.Container = Name;
                base.Line = Line;
                switch (Usage.ToUpper())
                {
                case "START":
                    this.Name = Name + " Block";
                    this.IsBlock = true;
                    this.Start = Value;
                    break;

                case "END":
                    this.Name = Name + " Block";
                    this.IsBlock = true;
                    this.End = Value;
                    break;

                case "LINE":
                    this.Name = Name + " Line";
                    this.IsBlock = false;
                    this.Start = Value;
                    break;
                }
            }

            internal void AddSymbolAttrib(GrammarAttribAssign Assign)
            {
                if (Grammar.SymbolAttributes.ItemIndex(Assign) == -1)
                {
                    Grammar.SymbolAttributes.Add(Assign);
                }
                else
                {
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Duplicate symbol attribute assignment", "The attributes for the symbol '" + Assign.Name + "' were set previously.", Conversions.ToString(Assign.Line));
                }
            }
        }

        internal class GrammarGroupList : ArrayList
        {
            // Methods
            public int Add(Grammar.GrammarGroup Item)
            {
                return base.Add(Item);
            }

            public int ItemIndex(string Name)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    Grammar.GrammarGroup group = (Grammar.GrammarGroup)base[i];
                    if (group.Name.ToUpper() == Name.ToUpper())
                    {
                        num = i;
                    }
                }
                return num;
            }

            // Properties
            public new Grammar.GrammarGroup this[int Index]
            {
                get
                {
                    return (Grammar.GrammarGroup)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }

        internal class GrammarIdentifier : Grammar.GrammarParseItem
        {
            // Fields
            public string Name;

            // Methods
            public GrammarIdentifier()
            {
            }

            public GrammarIdentifier(string Name, int Line)
            {
                this.Name = Name;
                base.Line = Line;
            }
        }

        internal class GrammarIdentifierList : ArrayList
        {
            // Methods
            public int Add(Grammar.GrammarIdentifier Item)
            {
                return base.Add(Item);
            }

            public int ItemIndex(string Name)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    Grammar.GrammarIdentifier identifier = (Grammar.GrammarIdentifier)base[i];
                    if (identifier.Name.ToUpper() == Name.ToUpper())
                    {
                        num = i;
                    }
                }
                return num;
            }

            // Properties
            public new Grammar.GrammarIdentifier this[int Index]
            {
                get
                {
                    return (Grammar.GrammarIdentifier)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }

        internal class GrammarParseItem
        {
            // Fields
            public int Line;
        }

        internal class GrammarProduction
        {
            // Fields
            public Grammar.GrammarSymbolList Handle;
            public Grammar.GrammarSymbol Head;
            public int Line;

            // Methods
            internal bool IsIdentical(Grammar.GrammarProduction Prod)
            {
                bool flag2 = false;
                if (!this.Head.IsIdentical(Prod.Head))
                {
                    flag2 = true;
                }
                else if (this.Handle.Count != Prod.Handle.Count)
                {
                    flag2 = true;
                }
                else
                {
                    for (int i = 0; (i < this.Handle.Count) & !flag2; i++)
                    {
                        flag2 = !this.Handle[i].IsIdentical(Prod.Handle[i]);
                    }
                }
                return !flag2;
            }
        }

        internal class GrammarProductionList : ArrayList
        {
            // Methods
            public int Add(Grammar.GrammarProduction Item)
            {
                return base.Add(Item);
            }

            public int ItemIndex(Grammar.GrammarProduction Prod)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    if (this[i].IsIdentical(Prod))
                    {
                        num = i;
                    }
                }
                return num;
            }

            // Properties
            public new Grammar.GrammarProduction this[int Index]
            {
                get
                {
                    return (Grammar.GrammarProduction)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }

        internal class GrammarSet : Grammar.GrammarParseItem
        {
            // Fields
            public ISetExpression Exp;
            public string Name;
        }

        internal class GrammarSetList : ArrayList
        {
            // Methods
            public int Add(Grammar.GrammarSet Item)
            {
                return base.Add(Item);
            }

            public int ItemIndex(string Name)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    Grammar.GrammarSet set = (Grammar.GrammarSet)base[i];
                    if (set.Name.ToUpper() == Name.ToUpper())
                    {
                        num = i;
                    }
                }
                return num;
            }

            // Properties
            public new Grammar.GrammarSet this[int Index]
            {
                get
                {
                    return (Grammar.GrammarSet)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }

        internal class GrammarSymbol : Grammar.GrammarParseItem
        {
            // Fields
            public RegExp Exp;
            public string Name;
            public SymbolType Type;

            // Methods
            public GrammarSymbol()
            {
                this.Type = SymbolType.Error;
            }

            public GrammarSymbol(string Name, SymbolType Type, int Line)
            {
                this.Type = Type;
                this.Name = Name;
                base.Line = Line;
            }

            internal bool IsIdentical(Grammar.GrammarSymbol Sym)
            {
                return ((this.Name.ToUpper() == Sym.Name) & (this.Type == Sym.Type));
            }
        }

        internal class GrammarSymbolList : ArrayList
        {
            // Methods
            public int Add(Grammar.GrammarSymbol Item)
            {
                return base.Add(Item);
            }

            internal short ItemIndex(Grammar.GrammarSymbol Sym)
            {
                return this.ItemIndex(Sym.Name, Sym.Type);
            }

            public int ItemIndex(string Name)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    Grammar.GrammarSymbol symbol = (Grammar.GrammarSymbol)base[i];
                    if (symbol.Name.ToUpper() == Name.ToUpper())
                    {
                        num = i;
                    }
                }
                return num;
            }

            internal short ItemIndex(string Name, SymbolType type)
            {
                int num = -1;
                for (int i = 0; (i < base.Count) & (num == -1); i++)
                {
                    Grammar.GrammarSymbol symbol = (Grammar.GrammarSymbol)base[i];
                    if ((symbol.Name.ToUpper() == Name.ToUpper()) & (symbol.Type == type))
                    {
                        num = i;
                    }
                }
                return (short)num;
            }

            // Properties
            public new Grammar.GrammarSymbol this[int Index]
            {
                get
                {
                    return (Grammar.GrammarSymbol)base[Index];
                }
                set
                {
                    base[Index] = value;
                }
            }
        }
    }
}