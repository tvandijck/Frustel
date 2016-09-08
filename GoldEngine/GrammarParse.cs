using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GoldEngine
{
    internal sealed class GrammarParse
    {
        // Fields
        public static bool Accepted;
        private static Parser Scanner = new Parser();

        // Methods
        private static object CreateNewObject(Reduction TheReduction)
        {
            object left = null;
            Reduction reduction2;
            int num2;
            object[] objArray;
            object[] objArray2;
            bool[] flagArray;
            RegExpItem item4;
            RegExpSeq seq5;
            Reduction reduction = TheReduction;
            switch (reduction.Parent.TableIndex)
            {
            case 13:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 14:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 15:
            {
                GrammarValueList instance = (GrammarValueList)reduction.get_Data(0);
                objArray = new object[1];
                reduction2 = reduction;
                num2 = 3;
                objArray[0] = RuntimeHelpers.GetObjectValue(reduction2.get_Data(num2));
                objArray2 = objArray;
                flagArray = new bool[] { true };
                NewLateBinding.LateCall(instance, null, "Add", objArray2, null, null, flagArray, true);
                if (flagArray[0])
                {
                    reduction2.set_Data(num2, RuntimeHelpers.GetObjectValue(objArray2[0]));
                }
                left = instance;
                break;
            }
            case 0x10:
            {
                GrammarValueList list2 = new GrammarValueList();
                objArray2 = new object[1];
                reduction2 = reduction;
                num2 = 0;
                objArray2[0] = RuntimeHelpers.GetObjectValue(reduction2.get_Data(num2));
                objArray = objArray2;
                flagArray = new bool[] { true };
                NewLateBinding.LateCall(list2, null, "Add", objArray, null, null, flagArray, true);
                if (flagArray[0])
                {
                    reduction2.set_Data(num2, RuntimeHelpers.GetObjectValue(objArray[0]));
                }
                left = list2;
                break;
            }
            case 0x11:
                left = Conversions.ToString(reduction.get_Data(0)) + " " + reduction.get_Data(1).ToString();
                break;

            case 0x12:
                left = RuntimeHelpers.GetObjectValue(reduction.get_Data(0));
                break;

            case 0x13:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 20:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 0x15:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 0x16:
            {
                string name = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                Grammar.AddProperty(name, ((GrammarValueList)reduction.get_Data(3)).ToString(), reduction[0].Position.Line);
                break;
            }
            case 0x17:
            {
                GrammarValueList list4 = (GrammarValueList)reduction.get_Data(0);
                GrammarValueList list = (GrammarValueList)reduction.get_Data(3);
                list4.Add(list);
                left = list4;
                break;
            }
            case 0x18:
                left = RuntimeHelpers.GetObjectValue(reduction.get_Data(0));
                break;

            case 0x19:
            {
                GrammarAttribAssign assign = new GrammarAttribAssign
                {
                    Name = Conversions.ToString(reduction.get_Data(0)),
                    Values = (GrammarAttribList)reduction.get_Data(4),
                    Line = reduction[2].Position.Line
                };
                Grammar.AddSymbolAttrib(assign);
                break;
            }
            case 0x1a:
            {
                GrammarAttribAssign assign2 = new GrammarAttribAssign
                {
                    Name = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(reduction.get_Data(0), " "), reduction.get_Data(1))),
                    Values = (GrammarAttribList)reduction.get_Data(5),
                    Line = reduction[3].Position.Line
                };
                Grammar.AddGroupAttrib(assign2);
                break;
            }
            case 0x1b:
            {
                GrammarAttribList list6 = (GrammarAttribList)reduction.get_Data(0);
                list6.Add((GrammarAttrib)reduction.get_Data(3));
                left = list6;
                break;
            }
            case 0x1c:
            {
                GrammarAttribList list7 = new GrammarAttribList();
                list7.Add((GrammarAttrib)reduction.get_Data(0));
                left = list7;
                break;
            }
            case 0x1d:
            {
                GrammarAttrib attrib = new GrammarAttrib
                {
                    Name = Conversions.ToString(reduction.get_Data(0))
                };
                objArray2 = new object[1];
                reduction2 = reduction;
                num2 = 2;
                objArray2[0] = RuntimeHelpers.GetObjectValue(reduction2.get_Data(num2));
                objArray = objArray2;
                flagArray = new bool[] { true };
                NewLateBinding.LateCall(attrib.List, null, "Add", objArray, null, null, flagArray, true);
                if (flagArray[0])
                {
                    reduction2.set_Data(num2, RuntimeHelpers.GetObjectValue(objArray[0]));
                }
                attrib.IsSet = false;
                left = attrib;
                break;
            }
            case 30:
            {
                GrammarAttrib attrib2 = new GrammarAttrib
                {
                    Name = Conversions.ToString(reduction.get_Data(0)),
                    List = (GrammarValueList)reduction.get_Data(3),
                    IsSet = true
                };
                left = attrib2;
                break;
            }
            case 0x1f:
            {
                Grammar.GrammarSet charSet = new Grammar.GrammarSet
                {
                    Name = Conversions.ToString(reduction.get_Data(1)),
                    Exp = (ISetExpression)reduction.get_Data(5),
                    Line = reduction[0].Position.Line
                };
                Grammar.AddUserSet(charSet);
                break;
            }
            case 0x20:
                left = new SetExp((ISetExpression)reduction.get_Data(0), '+', (ISetExpression)reduction.get_Data(3));
                break;

            case 0x21:
                left = new SetExp((ISetExpression)reduction.get_Data(0), '-', (ISetExpression)reduction.get_Data(3));
                break;

            case 0x22:
                left = RuntimeHelpers.GetObjectValue(reduction.get_Data(0));
                break;

            case 0x23:
            {
                reduction2 = reduction;
                num2 = 0;
                string fullText = Conversions.ToString(reduction2.get_Data(num2));
                Grammar.WarnRegexSetLiteral(fullText, reduction[0].Position.Line);
                reduction2.set_Data(num2, fullText);
                left = new SetItem(new CharacterSetBuild(Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)))));
                break;
            }
            case 0x24:
                Grammar.AddUsedSetName(Conversions.ToString(reduction.get_Data(1)), reduction[0].Position.Line);
                left = new SetItem(SetItem.SetType.Name, Conversions.ToString(reduction.get_Data(1)));
                break;

            case 0x25:
            {
                CharacterSetBuild build = (CharacterSetBuild)reduction.get_Data(1);
                left = new SetItem(build);
                break;
            }
            case 0x26:
                left = Operators.ConcatenateObject(Operators.ConcatenateObject(reduction.get_Data(0), " "), reduction.get_Data(1));
                break;

            case 0x27:
                left = RuntimeHelpers.GetObjectValue(reduction.get_Data(0));
                break;

            case 40:
            {
                CharacterSetBuild build2 = (CharacterSetBuild)reduction.get_Data(0);
                CharacterSetRange range = (CharacterSetRange)reduction.get_Data(3);
                build2.AddRange(range.First, range.Last);
                left = build2;
                break;
            }
            case 0x29:
            {
                CharacterSetBuild build3 = new CharacterSetBuild();
                CharacterSetRange range2 = (CharacterSetRange)reduction.get_Data(0);
                build3.AddRange(range2.First, range2.Last);
                left = build3;
                break;
            }
            case 0x2a:
                left = new CharacterSetRange(Conversions.ToInteger(reduction.get_Data(0)), Conversions.ToInteger(reduction.get_Data(0)));
                break;

            case 0x2b:
                left = new CharacterSetRange(Conversions.ToInteger(reduction.get_Data(0)), Conversions.ToInteger(reduction.get_Data(2)));
                break;

            case 0x2c:
                left = Grammar.GetHexValue(Conversions.ToString(reduction.get_Data(0)));
                if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectLess(left, 0, false), Operators.CompareObjectGreater(left, 0xffff, false))))
                {
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid set constant value", Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("The value '", reduction.get_Data(0)), "' is not valid.")), Conversions.ToString(reduction[0].Position.Line));
                }
                break;

            case 0x2d:
                left = Grammar.GetDecValue(Conversions.ToString(reduction.get_Data(0)));
                if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectLess(left, 0, false), Operators.CompareObjectGreater(left, 0xffff, false))))
                {
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid set constant value", Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("The value '", reduction.get_Data(0)), "' is not valid.")), Conversions.ToString(reduction[0].Position.Line));
                }
                break;

            case 0x2e:
            {
                string str3 = Conversions.ToString(reduction.get_Data(1));
                string str6 = str3.ToUpper();
                if (((str6 == "LINE") || (str6 == "START")) || (str6 == "END"))
                {
                    Grammar.GrammarGroup g = new Grammar.GrammarGroup(Conversions.ToString(reduction.get_Data(0)), Conversions.ToString(reduction.get_Data(1)), Conversions.ToString(reduction.get_Data(4)), reduction[1].Position.Line);
                    Grammar.AddGroup(g);
                }
                else
                {
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid group usage value", "The usage value '" + str3 + "' is not valid. It can be either Start, End or Line.", Conversions.ToString(reduction[1].Position.Line));
                }
                break;
            }
            case 0x2f:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 0x30:
                left = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0)));
                break;

            case 0x31:
            {
                Grammar.GrammarSymbol sym = new Grammar.GrammarSymbol
                {
                    Name = Conversions.ToString(reduction.get_Data(0)),
                    Exp = (RegExp)reduction.get_Data(3),
                    Line = reduction[2].Position.Line,
                    Type = SymbolType.Content
                };
                Grammar.AddTerminalHead(sym);
                break;
            }
            case 50:
            {
                RegExp exp = (RegExp)reduction.get_Data(0);
                RegExpSeq seq = (RegExpSeq)reduction.get_Data(3);
                seq.Priority = -1;
                exp.Add(seq);
                left = exp;
                break;
            }
            case 0x33:
            {
                RegExp exp2 = new RegExp();
                RegExpSeq seq2 = (RegExpSeq)reduction.get_Data(0);
                seq2.Priority = -1;
                exp2.Add(seq2);
                left = exp2;
                break;
            }
            case 0x34:
            {
                RegExpSeq seq3 = (RegExpSeq)reduction.get_Data(0);
                reduction2 = reduction;
                num2 = 1;
                item4 = (RegExpItem)reduction2.get_Data(num2);
                seq3.Add(item4);
                reduction2.set_Data(num2, item4);
                left = seq3;
                break;
            }
            case 0x35:
            {
                RegExpSeq seq4 = new RegExpSeq();
                reduction2 = reduction;
                num2 = 0;
                item4 = (RegExpItem)reduction2.get_Data(num2);
                seq4.Add(item4);
                reduction2.set_Data(num2, item4);
                left = seq4;
                break;
            }
            case 0x36:
            {
                SetItem data = (SetItem)reduction.get_Data(0);
                left = new RegExpItem(data, Conversions.ToString(reduction.get_Data(1)));
                break;
            }
            case 0x37:
            {
                SetItem item2 = new SetItem(SetItem.SetType.Sequence, Grammar.TokenText(Conversions.ToString(reduction.get_Data(0))));
                left = new RegExpItem(item2, Conversions.ToString(reduction.get_Data(1)));
                break;
            }
            case 0x38:
            {
                SetItem item3 = new SetItem(SetItem.SetType.Sequence, Conversions.ToString(reduction.get_Data(0)));
                left = new RegExpItem(item3, Conversions.ToString(reduction.get_Data(1)));
                break;
            }
            case 0x39:
                left = new RegExpItem(RuntimeHelpers.GetObjectValue(reduction.get_Data(1)), Conversions.ToString(reduction.get_Data(3)));
                break;

            case 0x3a:
            {
                RegExp exp3 = (RegExp)reduction.get_Data(0);
                reduction2 = reduction;
                num2 = 2;
                seq5 = (RegExpSeq)reduction2.get_Data(num2);
                exp3.Add(seq5);
                reduction2.set_Data(num2, seq5);
                left = exp3;
                break;
            }
            case 0x3b:
            {
                RegExp exp4 = new RegExp();
                reduction2 = reduction;
                num2 = 0;
                seq5 = (RegExpSeq)reduction2.get_Data(num2);
                exp4.Add(seq5);
                reduction2.set_Data(num2, seq5);
                left = exp4;
                break;
            }
            case 60:
                left = "+";
                break;

            case 0x3d:
                left = "?";
                break;

            case 0x3e:
                left = "*";
                break;

            case 0x3f:
                left = "";
                break;

            case 0x40:
            {
                IEnumerator enumerator;
                Grammar.GrammarProductionList list8 = (Grammar.GrammarProductionList)reduction.get_Data(3);
                Grammar.GrammarSymbol symbol2 = new Grammar.GrammarSymbol
                {
                    Name = Grammar.TokenText(Conversions.ToString(reduction.get_Data(0))),
                    Type = SymbolType.Nonterminal,
                    Line = reduction[2].Position.Line
                };
                try
                {
                    enumerator = list8.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Grammar.GrammarProduction current = (Grammar.GrammarProduction)enumerator.Current;
                        current.Head = symbol2;
                        Grammar.AddProduction(current);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                break;
            }
            case 0x41:
            {
                Grammar.GrammarProductionList list9 = (Grammar.GrammarProductionList)reduction.get_Data(0);
                Grammar.GrammarProduction item = new Grammar.GrammarProduction
                {
                    Handle = (Grammar.GrammarSymbolList)reduction.get_Data(3),
                    Line = reduction[2].Position.Line
                };
                list9.Add(item);
                left = list9;
                break;
            }
            case 0x42:
            {
                Grammar.GrammarProductionList list10 = new Grammar.GrammarProductionList();
                Grammar.GrammarProduction production3 = new Grammar.GrammarProduction
                {
                    Handle = (Grammar.GrammarSymbolList)reduction.get_Data(0)
                };
                list10.Add(production3);
                left = list10;
                break;
            }
            case 0x43:
            {
                Grammar.GrammarSymbolList list11 = (Grammar.GrammarSymbolList)reduction.get_Data(0);
                left = list11;
                break;
            }
            case 0x44:
            {
                Grammar.GrammarSymbolList list12 = new Grammar.GrammarSymbolList();
                left = list12;
                break;
            }
            case 0x45:
            {
                Grammar.GrammarSymbolList list13 = (Grammar.GrammarSymbolList)reduction.get_Data(0);
                list13.Add((Grammar.GrammarSymbol)reduction.get_Data(1));
                left = list13;
                break;
            }
            case 70:
            {
                Grammar.GrammarSymbolList list14 = new Grammar.GrammarSymbolList();
                left = list14;
                break;
            }
            case 0x47:
            {
                Grammar.GrammarSymbol symbol3 = new Grammar.GrammarSymbol(Conversions.ToString(reduction.get_Data(0)), SymbolType.Content, reduction[0].Position.Line);
                Grammar.AddHandleSymbol(symbol3);
                left = symbol3;
                break;
            }
            case 0x48:
            {
                Grammar.GrammarSymbol symbol4 = new Grammar.GrammarSymbol(Grammar.TokenText(Conversions.ToString(reduction.get_Data(0))), SymbolType.Nonterminal, reduction[0].Position.Line);
                Grammar.AddHandleSymbol(symbol4);
                left = symbol4;
                break;
            }
            }
            reduction = null;
            return left;
        }

        private static string FriendlyTerminalName(Symbol Sym)
        {
            switch (((short)(Sym.TableIndex - 0x16)))
            {
            case 0:
                return "Decimal Number";

            case 1:
                return "Hexadecimal Number";

            case 2:
                return "Identifier";

            case 3:
                return "Literal";

            case 4:
                return "New Line";

            case 5:
                return "Nonterminal";

            case 6:
                return "Parameter Name";

            case 7:
                return "Set Literal";
            }
            return Sym.Name;
        }

        public static bool Parse(TextReader Reader)
        {
            string right = "";
            string str = "";
            Grammar.Clear();
            Accepted = false;
            Parser scanner = Scanner;
            scanner.Open(Reader);
            scanner.TrimReductions = false;
            bool flag = false;
            while (!flag)
            {
                short num;
                short num2;
                object[] objArray;
                ParseMessage message = scanner.Parse();
                switch (((int)message))
                {
                case 0:
                    str = Conversions.ToString(scanner.CurrentToken().Data);
                    goto Label_0296;

                case 1:
                {
                    Parser parser2 = scanner;
                    Reduction currentReduction = (Reduction)parser2.CurrentReduction;
                    parser2.CurrentReduction = currentReduction;
                    scanner.CurrentReduction = RuntimeHelpers.GetObjectValue(CreateNewObject(currentReduction));
                    goto Label_0296;
                }
                case 2:
                    flag = true;
                    goto Label_0296;

                case 3:
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "INTERNAL ERROR", "The scanner was unable to be initialized. Please report this bug.", "");
                    flag = true;
                    goto Label_0296;

                case 4:
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Lexical Error", "Cannot recognize the token after: " + str, Conversions.ToString(scanner.CurrentPosition().Line));
                    flag = true;
                    goto Label_0296;

                case 5:
                    if (scanner.ExpectedSymbols().Count() < 1)
                    {
                        goto Label_011B;
                    }
                    right = FriendlyTerminalName(scanner.ExpectedSymbols()[0]);
                    num2 = (short)(scanner.ExpectedSymbols().Count() - 1);
                    num = 1;
                    goto Label_0112;

                case 6:
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Runaway Comment", "You have a unterminated block comment.", "");
                    flag = true;
                    goto Label_0296;

                case 7:
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "INTERNAL ERROR", "The scanner had an internal error. Please report this bug.", "");
                    flag = true;
                    goto Label_0296;

                default:
                    goto Label_0296;
                }
                Label_00EC:
                right = right + ", " + FriendlyTerminalName(scanner.ExpectedSymbols()[num]);
                num = (short)(num + 1);
                Label_0112:
                if (num <= num2)
                {
                    goto Label_00EC;
                }
                Label_011B:
                objArray = new object[2];
                Token token = scanner.CurrentToken();
                objArray[0] = RuntimeHelpers.GetObjectValue(token.Data);
                objArray[1] = false;
                object[] arguments = objArray;
                bool[] copyBack = new bool[] { true, false };
                if (copyBack[0])
                {
                    token.Data = RuntimeHelpers.GetObjectValue(arguments[0]);
                }
                BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Syntax Error", Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("Read: ", NewLateBinding.LateGet(null, typeof(BuilderUtility), "DisplayText", arguments, null, null, copyBack)), "\r\n"), "Expecting: "), right)), Conversions.ToString(scanner.CurrentPosition().Line));
                flag = true;
                Label_0296:
                Notify.Counter = scanner.CurrentPosition().Line;
            }
            Notify.Counter = scanner.CurrentPosition().Line;
            scanner = null;
            return Accepted;
        }

        public static void Setup()
        {
            string str3 = "";
            string str = "";
            string str2 = FileUtility.AppPath();
            bool flag = false;
            if (Scanner == null)
            {
                flag = true;
                str = "The scanner object is nothing";
            }
            else if (!Scanner.LoadTables(str2 + @"\gp.dat"))
            {
                flag = true;
                str = "The file 'gp.dat' cannot be loaded.";
            }
            else
            {
                str3 = Strings.Trim(Scanner.Tables.Properties["Version"].Value);
                if (str3 == "")
                {
                    flag = true;
                    str = "The file 'gp.dat' is invalid: ";
                }
                else if (str3 != "5.0.1")
                {
                    flag = true;
                    str = "The file 'gp.dat' is the incorrect version: '" + str3 + "'";
                }
            }
            if (flag)
            {
                BuilderApp.FatalLoadMessage = str;
                BuilderApp.FatalLoadError = true;
            }
        }

        // Nested Types
        private enum ProductionIndex
        {
            Grammar,
            Content,
            Content2,
            Definition,
            Definition2,
            Definition3,
            Definition4,
            Definition5,
            Definition6,
            Nlo_Newline,
            Nlo,
            Nl_Newline,
            Nl_Newline2,
            Terminalname_Identifier,
            Terminalname_Literal,
            Valuelist_Comma,
            Valuelist,
            Valueitems,
            Valueitems2,
            Valueitem_Identifier,
            Valueitem_Nonterminal,
            Valueitem_Literal,
            Param_Parametername_Eq,
            Parambody_Pipe,
            Parambody,
            Attributedecl_Ateq_Lbrace_Rbrace,
            Attributedecl_Identifier_Ateq_Lbrace_Rbrace,
            Attributelist_Comma,
            Attributelist,
            Attributeitem_Identifier_Eq_Identifier,
            Attributeitem_Identifier_Eq_Lbrace_Rbrace,
            Setdecl_Lbrace_Rbrace_Eq,
            Setexp_Plus,
            Setexp_Minus,
            Setexp,
            Setitem_Setliteral,
            Setitem_Lbrace_Rbrace,
            Setitem_Lbrace_Rbrace2,
            Idseries_Identifier,
            Idseries_Identifier2,
            Charcodelist_Comma,
            Charcodelist,
            Charcodeitem,
            Charcodeitem_Dotdot,
            Charcodevalue_Hexnumber,
            Charcodevalue_Decnumber,
            Groupdecl_Identifier_Eq,
            Groupitem_Identifier,
            Groupitem_Literal,
            Terminaldecl_Eq,
            Terminalbody_Pipe,
            Terminalbody,
            Regexpseq,
            Regexpseq2,
            Regexpitem,
            Regexpitem_Literal,
            Regexpitem_Identifier,
            Regexpitem_Lparan_Rparan,
            Subregexp_Pipe,
            Subregexp,
            Kleeneopt_Plus,
            Kleeneopt_Question,
            Kleeneopt_Times,
            Kleeneopt,
            Ruledecl_Nonterminal_Coloncoloneq,
            Handles_Pipe,
            Handles,
            Handle,
            Handle_Ltgt,
            Symbols,
            Symbols2,
            Symbol,
            Symbol_Nonterminal
        }

        private enum SymbolIndex
        {
            Eof,
            Error,
            Comment,
            Whitespace,
            Exclam,
            Exclamtimes,
            Timesexclam,
            Minus,
            Lparan,
            Rparan,
            Times,
            Comma,
            Dotdot,
            Coloncoloneq,
            Question,
            Ateq,
            Lbrace,
            Pipe,
            Rbrace,
            Plus,
            Ltgt,
            Eq,
            Decnumber,
            Hexnumber,
            Identifier,
            Literal,
            Newline,
            Nonterminal,
            Parametername,
            Setliteral,
            Attributedecl,
            Attributeitem,
            Attributelist,
            Charcodeitem,
            Charcodelist,
            Charcodevalue,
            Content,
            Definition,
            Grammar,
            Groupdecl,
            Groupitem,
            Handle,
            Handles,
            Idseries,
            Kleeneopt,
            Nl,
            Nlo,
            Param,
            Parambody,
            Regexpitem,
            Regexpseq,
            Ruledecl,
            Setdecl,
            Setexp,
            Setitem,
            Subregexp,
            Symbol,
            Symbols,
            Terminalbody,
            Terminaldecl,
            Terminalname,
            Valueitem,
            Valueitems,
            Valuelist
        }
    }
}