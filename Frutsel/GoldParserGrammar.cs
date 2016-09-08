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

        private readonly Symbol m_comment;
        private readonly Symbol m_whiteTerminal;
        private readonly Symbol m_commentLine;
        private readonly Symbol m_commentStart;
        private readonly Symbol m_commentEnd;

        private readonly Symbol m_decNumber;
        private readonly Symbol m_hexNumber;
        private readonly Symbol m_identifier;
        private readonly Symbol m_parameterName;
        private readonly Symbol m_nonTerminal;
        private readonly Symbol m_literal;
        private readonly Symbol m_setLiteral;
        private readonly Symbol m_newlineTerminal;

        public GoldParserGrammar()
        {
            // Character Sets.
            m_parameterCh = AddCharacterSet("Parameter Ch", CharacterSets["Printable"] - "'\"");
            m_terminalCh = AddCharacterSet("Terminal Ch", CharacterSets["Alphanumeric"] + "_-.");
            m_nonTerminalCh = AddCharacterSet("Nonterminal Ch", m_terminalCh + CharacterSets["Space"]);
            m_literalCh = AddCharacterSet("Literal Ch", CharacterSets["Printable"] - '\'');
            m_setLiteralCh = AddCharacterSet("Set Literal Ch", CharacterSets["Printable"] - "[]'");
            m_setNameCh = AddCharacterSet("Set Name Ch", CharacterSets["Printable"] - "{}");
            m_whitespaceCh = AddCharacterSet("Whitespace Ch", CharacterSets["Whitespace"] - CharacterSets["CR"] - CharacterSets["LF"]);
            m_idHead = AddCharacterSet("Id Head", CharacterSets["Letter"] + "_");
            m_idTail = AddCharacterSet("Id Tail", m_idHead + CharacterSets["Digit"]);
            m_hexDigit = AddCharacterSet("HexDigit Ch", CharacterSets["Digit"] + "abcdefABCDEF");

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
            AddSymbol(new SymbolTerminal("-", new Expression { "-" }));
            AddSymbol(new SymbolTerminal("(", new Expression { "(" }));
            AddSymbol(new SymbolTerminal(")", new Expression { ")" }));
            AddSymbol(new SymbolTerminal("*", new Expression { "*" }));
            AddSymbol(new SymbolTerminal(",", new Expression { "," }));
            AddSymbol(new SymbolTerminal("..", new Expression { ".." }));
            AddSymbol(new SymbolTerminal("::=", new Expression { "::=" }));
            AddSymbol(new SymbolTerminal("?", new Expression { "?" }));
            AddSymbol(new SymbolTerminal("@=", new Expression { "@=" }));
            AddSymbol(new SymbolTerminal("{", new Expression { "{" }));
            AddSymbol(new SymbolTerminal("|", new Expression { "|" }));
            AddSymbol(new SymbolTerminal("}", new Expression { "}" }));
            AddSymbol(new SymbolTerminal("+", new Expression { "+" }));
            AddSymbol(new SymbolTerminal("<>", new Expression { "<>" }));
            AddSymbol(new SymbolTerminal("=", new Expression { "=" }));

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

            // Newline    = {CR}{LF} | {CR} | {LF}
            m_newlineTerminal = AddSymbol(new SymbolTerminal("Newline", new Expression
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
            m_commentLine.Group = new Group
            {
                Name = "Comment Line",
                Container = m_comment,
                Start = m_commentLine,
                End = m_newlineTerminal,
                Advance = EAdvanceMode.CharacterAdvanceMode,
                Ending = EEndingMode.OpenEndingMode,
            };
            m_commentStart.Group = new Group
            {
                Name = "Comment Block",
                Container = m_comment,
                Start = m_commentStart,
                End = m_commentEnd,
                Advance = EAdvanceMode.CharacterAdvanceMode,
                Ending = EEndingMode.OpenEndingMode,
            };
            m_commentStart.Group.Nesting.Add(m_commentLine.Group);
            m_commentStart.Group.Nesting.Add(m_commentStart.Group);
        }
    }
}
