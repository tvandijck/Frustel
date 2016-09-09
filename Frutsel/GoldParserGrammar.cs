using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel
{
    public sealed class GoldParserGrammar : Grammar
    {
        private readonly CharacterSet m_hexDigit;
        private readonly CharacterSet m_parameterCh;
        private readonly CharacterSet m_nonTerminalCh;
        private readonly CharacterSet m_terminalCh;
        private readonly CharacterSet m_literalCh;
        private readonly CharacterSet m_setLiteralCh;
        private readonly CharacterSet m_setNameCh;
        private readonly CharacterSet m_whitespaceCh;
        private readonly CharacterSet m_idHead;
        private readonly CharacterSet m_idTail;

        // terminals.
        private readonly Symbol m_comment;
        private readonly Symbol m_whiteTerminal;
        private readonly Symbol m_commentLine;
        private readonly Symbol m_commentStart;
        private readonly Symbol m_commentEnd;
        private readonly Symbol m_minus;
        private readonly Symbol m_lparen;
        private readonly Symbol m_rparen;
        private readonly Symbol m_times;
        private readonly Symbol m_comma;
        private readonly Symbol m_dotDot;
        private readonly Symbol m_colonColonEq;
        private readonly Symbol m_question;
        private readonly Symbol m_ateq;
        private readonly Symbol m_lbrace;
        private readonly Symbol m_eq;
        private readonly Symbol m_ltgt;
        private readonly Symbol m_plus;
        private readonly Symbol m_rbrace;
        private readonly Symbol m_pipe;
        private readonly Symbol m_decNumber;
        private readonly Symbol m_hexNumber;
        private readonly Symbol m_identifier;
        private readonly Symbol m_parameterName;
        private readonly Symbol m_nonTerminal;
        private readonly Symbol m_literal;
        private readonly Symbol m_setLiteral;
        private readonly Symbol m_newLine;

        // nonterminals.
        private readonly Symbol m_attributeDecl;
        private readonly Symbol m_attributeItem;
        private readonly Symbol m_attributeList;
        private readonly Symbol m_charcodeItem;
        private readonly Symbol m_charcodeList;
        private readonly Symbol m_charcodeValue;
        private readonly Symbol m_content;
        private readonly Symbol m_definition;
        private readonly Symbol m_grammar;
        private readonly Symbol m_groupDecl;
        private readonly Symbol m_groupItem;
        private readonly Symbol m_handle;
        private readonly Symbol m_handles;
        private readonly Symbol m_idSeries;
        private readonly Symbol m_kleeneOpt;
        private readonly Symbol m_nl;
        private readonly Symbol m_nlo;
        private readonly Symbol m_param;
        private readonly Symbol m_paramBody;
        private readonly Symbol m_regExpItem;
        private readonly Symbol m_regExpSeq;
        private readonly Symbol m_ruleDecl;
        private readonly Symbol m_setDecl;
        private readonly Symbol m_setExp;
        private readonly Symbol m_setItem;
        private readonly Symbol m_subRegExp;
        private readonly Symbol m_symbol;
        private readonly Symbol m_symbols;
        private readonly Symbol m_terminalBody;
        private readonly Symbol m_terminalDecl;
        private readonly Symbol m_terminalName;
        private readonly Symbol m_valueItem;
        private readonly Symbol m_valueItems;
        private readonly Symbol m_valueList;


        public GoldParserGrammar()
        {
            // Character Sets.
            m_parameterCh = AddCharacterSet("Parameter Ch", Printable - "'\"");
            m_terminalCh = AddCharacterSet("Terminal Ch", AlphaNumeric + "_-.");
            m_nonTerminalCh = AddCharacterSet("Nonterminal Ch", m_terminalCh + Space);
            m_literalCh = AddCharacterSet("Literal Ch", Printable - '\'');
            m_setLiteralCh = AddCharacterSet("Set Literal Ch", Printable - "[]'");
            m_setNameCh = AddCharacterSet("Set Name Ch", Printable - "{}");
            m_whitespaceCh = AddCharacterSet("Whitespace Ch", Whitespace - CR - LF);
            m_idHead = AddCharacterSet("Id Head", Letter + "_");
            m_idTail = AddCharacterSet("Id Tail", m_idHead + Digit);
            m_hexDigit = AddCharacterSet("HexDigit Ch", Digit + "abcdefABCDEF");

            // Comment container.
            m_comment = AddSymbol(new SymbolNoise("Comment", null));

            // Whitespace = {Whitespace Ch}+
            m_whiteTerminal = AddSymbol(new SymbolNoise("Whitespace", new Expression
            {
                new Sequence
                {
                    {EKleene.OneOrMore, m_whitespaceCh},
                }
            }));

            // Comment line/start/end
            m_commentLine = AddSymbol(new SymbolGroupStart("!", new Expression { "!" }));
            m_commentStart = AddSymbol(new SymbolGroupStart("!*", new Expression { "!*" }));
            m_commentEnd = AddSymbol(new SymbolGroupEnd("*!", new Expression { "*!" }));

            // simple symbols.
            m_minus = AddSymbol(new SymbolTerminal("-", new Expression { "-" }));
            m_lparen = AddSymbol(new SymbolTerminal("(", new Expression { "(" }));
            m_rparen = AddSymbol(new SymbolTerminal(")", new Expression { ")" }));
            m_times = AddSymbol(new SymbolTerminal("*", new Expression { "*" }));
            m_comma = AddSymbol(new SymbolTerminal(",", new Expression { "," }));
            m_dotDot = AddSymbol(new SymbolTerminal("..", new Expression { ".." }));
            m_colonColonEq = AddSymbol(new SymbolTerminal("::=", new Expression { "::=" }));
            m_question = AddSymbol(new SymbolTerminal("?", new Expression { "?" }));
            m_ateq = AddSymbol(new SymbolTerminal("@=", new Expression { "@=" }));
            m_lbrace = AddSymbol(new SymbolTerminal("{", new Expression { "{" }));
            m_pipe = AddSymbol(new SymbolTerminal("|", new Expression { "|" }));
            m_rbrace = AddSymbol(new SymbolTerminal("}", new Expression { "}" }));
            m_plus = AddSymbol(new SymbolTerminal("+", new Expression { "+" }));
            m_ltgt = AddSymbol(new SymbolTerminal("<>", new Expression { "<>" }));
            m_eq = AddSymbol(new SymbolTerminal("=", new Expression { "=" }));

            // DecNumber = {Digit}+
            m_decNumber = AddSymbol(new SymbolTerminal("DecNumber", new Expression
            {
                new Sequence
                {
                    {EKleene.OneOrMore, CharacterSets["Digit"]}
                }
            }));

            // HexNumber = 0x{Hex Digit}+
            m_hexNumber = AddSymbol(new SymbolTerminal("HexNumber", new Expression
            {
                new Sequence
                {
                    {EKleene.OneOrMore, new CharacterSet('0')},
                    {EKleene.OneOrMore, new CharacterSet('x')},
                    {EKleene.OneOrMore, m_hexDigit}
                }
            }));

            // Identifier = {Id Head}{Id Tail}*
            m_identifier = AddSymbol(new SymbolTerminal("Identifier", new Expression
            {
                new Sequence
                {
                    {EKleene.None, m_idHead},
                    {EKleene.ZeroOrMore, m_idTail},
                }
            }));

            // Literal = '' {Literal Ch}* ''
            m_literal = AddSymbol(new SymbolTerminal("Literal", new Expression
            {
                new Sequence
                {
                    {EKleene.None, new CharacterSet('\'')},
                    {EKleene.ZeroOrMore, m_literalCh},
                    {EKleene.None, new CharacterSet('\'')}
                },
            }));

            // NewLine    = {CR}{LF} | {CR} | {LF}
            m_newLine = AddSymbol(new SymbolTerminal("NewLine", new Expression
            {
                new Sequence
                {
                    {EKleene.None, CharacterSets["CR"]},
                    {EKleene.None, CharacterSets["LF"]}
                },
                new Sequence
                {
                    {EKleene.None, CharacterSets["CR"]},
                },
                new Sequence
                {
                    {EKleene.None, CharacterSets["LF"]},
                }
            }));

            // NonTerminal = '<' {Nonterminal Ch}+ '>'
            m_nonTerminal = AddSymbol(new SymbolTerminal("NonTerminal", new Expression
            {
                new Sequence
                {
                    {EKleene.None, new CharacterSet('<')},
                    {EKleene.OneOrMore, m_nonTerminalCh},
                    {EKleene.None, new CharacterSet('>')}
                }
            }));

            // ParameterName = '"' {Parameter Ch}+ '"' 
            m_parameterName = AddSymbol(new SymbolTerminal("ParameterName", new Expression
            {
                new Sequence
                {
                    {EKleene.None, new CharacterSet('"')},
                    {EKleene.OneOrMore, m_parameterCh},
                    {EKleene.None, new CharacterSet('"')}
                }
            }));

            // SetLiteral = '[' ({Set Literal Ch} | '' {Literal Ch}* '' )+ ']'
            m_setLiteral = AddSymbol(new SymbolTerminal("SetLiteral", new Expression
            {
                new Sequence
                {
                    {EKleene.None, new CharacterSet('[')},
                    {EKleene.OneOrMore, new Expression
                        {
                            new Sequence
                            {
                                {EKleene.None, m_setLiteralCh},
                            },
                            new Sequence
                            {
                                {EKleene.None, new CharacterSet('\'')},
                                {EKleene.ZeroOrMore, m_literalCh},
                                {EKleene.None, new CharacterSet('\'')}
                            },
                        }
                    },
                    {EKleene.None, new CharacterSet(']')}
                },
            }));

            // Setup comment blocks.
            Group lineGroup = AddGroup(new Group
            {
                Name = "Comment Line",
                Container = m_comment,
                Start = m_commentLine,
                End = m_newLine,
                Advance = EAdvanceMode.CharacterAdvanceMode,
                Ending = EEndingMode.OpenEndingMode,
            });
            Group blockGroup = AddGroup(new Group
            {
                Name = "Comment Block",
                Container = m_comment,
                Start = m_commentStart,
                End = m_commentEnd,
                Advance = EAdvanceMode.CharacterAdvanceMode,
                Ending = EEndingMode.OpenEndingMode,
            });
            blockGroup.Nesting.Add(lineGroup);
            blockGroup.Nesting.Add(blockGroup);

            // Define all non terminal symbols.
            m_attributeDecl = AddSymbol(new SymbolNonTerminal("Attribute Decl"));
            m_attributeItem = AddSymbol(new SymbolNonTerminal("Attribute Item"));
            m_attributeList = AddSymbol(new SymbolNonTerminal("Attribute List"));
            m_charcodeItem = AddSymbol(new SymbolNonTerminal("Charcode Item"));
            m_charcodeList = AddSymbol(new SymbolNonTerminal("Charcode List"));
            m_charcodeValue = AddSymbol(new SymbolNonTerminal("Charcode Value"));
            m_content = AddSymbol(new SymbolNonTerminal("Content"));
            m_definition = AddSymbol(new SymbolNonTerminal("Definition"));
            m_grammar = AddSymbol(new SymbolNonTerminal("Grammar"));
            m_groupDecl = AddSymbol(new SymbolNonTerminal("Group Decl"));
            m_groupItem = AddSymbol(new SymbolNonTerminal("Group Item"));
            m_handle = AddSymbol(new SymbolNonTerminal("Handle"));
            m_handles = AddSymbol(new SymbolNonTerminal("Handles"));
            m_idSeries = AddSymbol(new SymbolNonTerminal("ID Series"));
            m_kleeneOpt = AddSymbol(new SymbolNonTerminal("Kleene Opt"));
            m_nl = AddSymbol(new SymbolNonTerminal("nl"));
            m_nlo = AddSymbol(new SymbolNonTerminal("nlo"));
            m_param = AddSymbol(new SymbolNonTerminal("Param"));
            m_paramBody = AddSymbol(new SymbolNonTerminal("Param Body"));
            m_regExpItem = AddSymbol(new SymbolNonTerminal("Reg Exp Item"));
            m_regExpSeq = AddSymbol(new SymbolNonTerminal("Reg Exp Seq"));
            m_ruleDecl = AddSymbol(new SymbolNonTerminal("Rule Decl"));
            m_setDecl = AddSymbol(new SymbolNonTerminal("Set Decl"));
            m_setExp = AddSymbol(new SymbolNonTerminal("Set Exp"));
            m_setItem = AddSymbol(new SymbolNonTerminal("Set Item"));
            m_subRegExp = AddSymbol(new SymbolNonTerminal("Sub Reg Exp"));
            m_symbol = AddSymbol(new SymbolNonTerminal("Symbol"));
            m_symbols = AddSymbol(new SymbolNonTerminal("Symbols"));
            m_terminalBody = AddSymbol(new SymbolNonTerminal("Terminal Body"));
            m_terminalDecl = AddSymbol(new SymbolNonTerminal("Terminal Decl"));
            m_terminalName = AddSymbol(new SymbolNonTerminal("Terminal Name"));
            m_valueItem = AddSymbol(new SymbolNonTerminal("Value Item"));
            m_valueItems = AddSymbol(new SymbolNonTerminal("Value Items"));
            m_valueList = AddSymbol(new SymbolNonTerminal("Value List"));

            // "Start Symbol" = <Grammar>
            StartSymbol = m_grammar;

            // <Grammar>
            //     ::= <nlo> <Content>
            AddProduction(m_grammar, m_nlo, m_content);

            //<Content>
            //    ::= <Content> <Definition>
            //    |   <Definition>
            AddProduction(m_content, m_content, m_definition);
            AddProduction(m_content, m_definition);

            //<Definition>
            //    ::= <Param>
            //    |   <Attribute Decl>
            //    |   <Set Decl>
            //    |   <Group Decl>
            //    |   <Terminal Decl>
            //    |   <Rule Decl>
            AddProduction(m_definition, m_param);
            AddProduction(m_definition, m_attributeDecl);
            AddProduction(m_definition, m_setDecl);
            AddProduction(m_definition, m_groupDecl);
            AddProduction(m_definition, m_terminalDecl);
            AddProduction(m_definition, m_ruleDecl);

            //<nlo>
            //    ::= Newline <nlo>
            //    |
            AddProduction(m_nlo, m_newLine, m_nlo);
            AddProduction(m_nlo);

            //<nl>
            //    ::= Newline <nl>
            //    |   Newline
            AddProduction(m_nl, m_newLine, m_nl);
            AddProduction(m_nl, m_newLine);

            //<Terminal Name>
            //    ::= Identifier
            //    |   Literal
            AddProduction(m_terminalName, m_identifier);
            AddProduction(m_terminalName, m_literal);

            //<Value List>
            //    ::= <Value List> ',' <nlo> <Value Items>
            //    |   <Value Items>
            AddProduction(m_valueList, m_valueList, m_comma, m_nlo, m_valueItems);
            AddProduction(m_valueList, m_valueItems);

            //<Value Items>
            //    ::= <Value Items> <Value Item>
            //    |   <Value Item>
            AddProduction(m_valueItems, m_valueItems, m_valueItem);
            AddProduction(m_valueItems, m_valueItem);

            //<Value Item>
            //    ::= Identifier
            //    |   Nonterminal
            //    |   Literal
            AddProduction(m_valueItem, m_identifier);
            AddProduction(m_valueItem, m_nonTerminal);
            AddProduction(m_valueItem, m_literal);

            //<Param>
            //    ::= ParameterName <nlo> '=' <Param Body> <nl>
            AddProduction(m_param, m_parameterName, m_nlo, m_eq, m_paramBody, m_nl);

            //<Param Body>
            //    ::= <Param Body> <nlo> '|' <Value List>
            //    |   <Value List>
            AddProduction(m_paramBody, m_paramBody, m_nlo, m_pipe, m_valueList);
            AddProduction(m_paramBody, m_valueList);

            //<Attribute Decl>
            //    ::= <Terminal Name> <nlo> '@=' '{' <Attribute List> '}' <nl>
            //    |   <Terminal Name> Identifier <nlo> '@=' '{' <Attribute List> '}' <nl>
            AddProduction(m_attributeDecl, m_terminalName, m_nlo, m_ateq, m_lbrace, m_attributeList, m_rbrace, m_nl);
            AddProduction(m_attributeDecl, m_terminalName, m_identifier, m_nlo, m_ateq, m_lbrace, m_attributeList, m_rbrace, m_nl);

            //<Attribute List>
            //    ::= <Attribute List> ',' <nlo> <Attribute Item>
            //    |   <Attribute Item>
            AddProduction(m_attributeList, m_attributeList, m_comma, m_nlo, m_attributeItem);
            AddProduction(m_attributeList, m_attributeItem);

            //<Attribute Item>
            //    ::= Identifier '=' Identifier
            //    |   Identifier '=' '{' <Value List> '}'
            AddProduction(m_attributeItem, m_identifier, m_eq, m_identifier);
            AddProduction(m_attributeItem, m_identifier, m_eq, m_lbrace, m_valueList, m_rbrace);

            //<Set Decl>
            //    ::= '{' <ID Series> '}' <nlo> '=' <Set Exp> <nl>
            AddProduction(m_setDecl, m_lbrace, m_idSeries, m_rbrace, m_nlo, m_eq, m_setExp, m_nl);

            //<Set Exp>
            //    ::= <Set Exp> <nlo> '+' <Set Item>
            //    |   <Set Exp> <nlo> '-' <Set Item>
            //    |   <Set Item>
            AddProduction(m_setExp, m_setExp, m_nlo, m_plus, m_setItem);
            AddProduction(m_setExp, m_setExp, m_nlo, m_minus, m_setItem);
            AddProduction(m_setExp, m_setItem);

            //<Set Item>
            //    ::= SetLiteral
            //    |   '{' <ID Series> '}'
            //    |   '{' <Charcode List> '}'
            AddProduction(m_setItem, m_setLiteral);
            AddProduction(m_setItem, m_lbrace, m_idSeries, m_rbrace);
            AddProduction(m_setItem, m_lbrace, m_charcodeList, m_rbrace);

            //<ID Series>
            //    ::= <ID Series> Identifier
            //    |   Identifier
            AddProduction(m_idSeries, m_idSeries, m_identifier);
            AddProduction(m_idSeries, m_identifier);

            //<Charcode List>
            //    ::= <Charcode List> ',' <nlo> <Charcode Item>
            //    |   <Charcode Item>
            AddProduction(m_charcodeList, m_charcodeList, m_comma, m_nlo, m_charcodeItem);
            AddProduction(m_charcodeList, m_charcodeItem);

            //<Charcode Item>
            //    ::= <Charcode Value>
            //    |   <Charcode Value> '..' <Charcode Value>
            AddProduction(m_charcodeItem, m_charcodeValue);
            AddProduction(m_charcodeItem, m_charcodeValue, m_dotDot, m_charcodeValue);

            //<Charcode Value>
            //    ::= HexNumber
            //    |   DecNumber
            AddProduction(m_charcodeValue, m_hexNumber);
            AddProduction(m_charcodeValue, m_decNumber);

            //<Group Decl>
            //    ::= <Terminal Name> Identifier <nlo> '=' <Group Item> <nl>
            AddProduction(m_groupDecl, m_terminalName, m_identifier, m_nlo, m_eq, m_groupItem, m_nl);

            //<Group Item>
            //    ::= Identifier
            //    |   Literal
            AddProduction(m_groupItem, m_identifier);
            AddProduction(m_groupItem, m_literal);

            //<Terminal Decl>
            //    ::= <Terminal Name> <nlo> '=' <Terminal Body> <nl>
            AddProduction(m_terminalDecl, m_terminalName, m_nlo, m_eq, m_terminalBody, m_nl);

            //<Terminal Body>
            //    ::= <Terminal Body> <nlo> '|' <Reg Exp Seq>
            //    |   <Reg Exp Seq>
            AddProduction(m_terminalBody, m_terminalBody, m_nlo, m_pipe, m_regExpSeq);
            AddProduction(m_terminalBody, m_regExpSeq);

            //<Reg Exp Seq>
            //    ::= <Reg Exp Seq> <Reg Exp Item>
            //    |   <Reg Exp Item>
            AddProduction(m_regExpSeq, m_regExpSeq, m_regExpItem);
            AddProduction(m_regExpSeq, m_regExpItem);

            //<Reg Exp Item>
            //    ::= <Set Item> <Kleene Opt>
            //    |   Literal <Kleene Opt>
            //    |   Identifier <Kleene Opt>
            //    |   '(' <Sub Reg Exp> ')' <Kleene Opt>
            AddProduction(m_regExpItem, m_setItem, m_kleeneOpt);
            AddProduction(m_regExpItem, m_literal, m_kleeneOpt);
            AddProduction(m_regExpItem, m_identifier, m_kleeneOpt);
            AddProduction(m_regExpItem, m_lparen, m_subRegExp, m_rparen, m_kleeneOpt);

            //<Sub Reg Exp>
            //    ::= <Sub Reg Exp> '|' <Reg Exp Seq>
            //    |   <Reg Exp Seq>
            AddProduction(m_subRegExp, m_subRegExp, m_pipe, m_regExpSeq);
            AddProduction(m_subRegExp, m_regExpSeq);

            //<Kleene Opt>
            //    ::= '+'
            //    |   '?'
            //    |   '*'
            //    |
            AddProduction(m_kleeneOpt, m_plus);
            AddProduction(m_kleeneOpt, m_question);
            AddProduction(m_kleeneOpt, m_times);
            AddProduction(m_kleeneOpt);

            //<Rule Decl>
            //    ::= Nonterminal <nlo> '::=' <Handles> <nl>
            AddProduction(m_ruleDecl, m_nonTerminal, m_nlo, m_colonColonEq, m_handles, m_nl);

            //<Handles>
            //    ::= <Handles> <nlo> '|' <Handle>
            //    |   <Handle>
            AddProduction(m_handles, m_handles, m_nlo, m_pipe, m_handle);
            AddProduction(m_handles, m_handle);

            //<Handle>
            //    ::= <Symbols>
            //    |   '<>'
            AddProduction(m_handle, m_symbols);
            AddProduction(m_handle, m_ltgt);

            //<Symbols>
            //    ::= <Symbols> <Symbol>
            //    |
            AddProduction(m_symbols, m_symbols, m_symbol);
            AddProduction(m_symbols);

            //<Symbol>
            //    ::= <Terminal Name>
            //    |   NonTerminal
            AddProduction(m_symbol, m_terminalName);
            AddProduction(m_symbol, m_nonTerminal);
        }
    }
}
