using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel
{
    public sealed class GoldParserGrammar : Grammar
    {
        private readonly CharacterSet m_parameterCh;
        private readonly CharacterSet m_nonTerminalCh;
        private readonly CharacterSet m_terminalCh;
        private readonly CharacterSet m_literalCh;
        private readonly CharacterSet m_setLiteralCh;
        private readonly CharacterSet m_setNameCh;
        private readonly CharacterSet m_whitespaceCh;

        private readonly Symbol m_parameterName;
        private readonly Symbol m_nonTerminal;
        private readonly Symbol m_terminal;
        private readonly Symbol m_setLiteral;
        private readonly Symbol m_setName;
        private readonly Symbol m_whiteTerminal;
        private readonly Symbol m_newlineTerminal;

        private readonly Symbol m_commentLine;
        private readonly Symbol m_commentStart;
        private readonly Symbol m_commentEnd;
        private readonly Symbol m_comment;

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

            // simple symbols.
            AddSymbol(new SymbolTerminal("-", new Expression { "-" }));
            AddSymbol(new SymbolTerminal("(", new Expression { "(" }));
            AddSymbol(new SymbolTerminal(")", new Expression { ")" }));
            AddSymbol(new SymbolTerminal("*", new Expression { "*" }));
            AddSymbol(new SymbolTerminal(",", new Expression { "," }));
            AddSymbol(new SymbolTerminal("?", new Expression { "?" }));
            AddSymbol(new SymbolTerminal("{", new Expression { "{" }));
            AddSymbol(new SymbolTerminal("|", new Expression { "|" }));
            AddSymbol(new SymbolTerminal("}", new Expression { "}" }));
            AddSymbol(new SymbolTerminal("+", new Expression { "+" }));
            AddSymbol(new SymbolTerminal("=", new Expression { "=" }));
            AddSymbol(new SymbolTerminal("..", new Expression { ".." }));
            AddSymbol(new SymbolTerminal("<>", new Expression { "<>" }));
            AddSymbol(new SymbolTerminal("@=", new Expression { "@=" }));
            AddSymbol(new SymbolTerminal("::=", new Expression { "::=" }));

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

            // Terminal = {Terminal Ch}+ | '' {Literal Ch}* ''
            m_terminal = AddSymbol(new SymbolTerminal("Terminal", new Expression
            {
                new Sequence
                {
                    {EKleene.OneOrMore, m_terminalCh},
                },
                new Sequence
                {
                    {EKleene.None, new CharacterSet('\'')},
                    {EKleene.ZeroOrMore, m_literalCh},
                    {EKleene.None, new CharacterSet('\'')}
                },
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
            Console.WriteLine(((SymbolTerminal)m_setLiteral).Expression.ToString());

            // SetName = '{' {Set Name Ch}+ '}'
            m_setName = AddSymbol(new SymbolTerminal("SetName", new Expression
            {
                new Sequence
                {
                    {EKleene.None, new CharacterSet('{')},
                    {EKleene.OneOrMore, m_setNameCh},
                    {EKleene.None, new CharacterSet('}')}
                }
            }));

            // Whitespace = {Whitespace Ch}+
            m_whiteTerminal = AddSymbol(new SymbolNoise("Whitespace", new Expression
            {
                new Sequence
                {
                    {EKleene.OneOrMore, m_whitespaceCh},
                }
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


            // Setup comment blocks.
            m_commentLine = AddSymbol(new SymbolGroupStart("!", new Expression { "!" }));
            m_commentStart = AddSymbol(new SymbolGroupStart("!*", new Expression { "!*" }));
            m_commentEnd = AddSymbol(new SymbolGroupEnd("*!", new Expression { "*!" }));

            m_comment = AddSymbol(new SymbolNoise("Comment", null));
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
