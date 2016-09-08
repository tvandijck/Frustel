using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace GoldEngine
{
    internal class Parser
    {
        // Fields
        private int m_CurrentLALR;
        private Position m_CurrentPosition = new Position();
        private SymbolList m_ExpectedSymbols = new SymbolList();
        private TokenStack m_GroupStack = new TokenStack();
        private bool m_HaveReduction;
        private TokenDeque m_InputTokens = new TokenDeque();
        private string m_LookaheadBuffer;
        private TextReader m_Source;
        private TokenStack m_Stack = new TokenStack();
        private Position m_SysPosition = new Position();
        public ParseTables m_Tables = new ParseTables();
        private bool m_TrimReductions;

        // Methods
        public Parser()
        {
            this.Restart();
            this.m_Tables.Clear();
            this.m_TrimReductions = false;
        }

        internal void Clear()
        {
            this.m_Tables.Clear();
            this.m_Stack.Clear();
            this.m_InputTokens.Clear();
            this.m_GroupStack.Clear();
            this.m_Tables.Group.Clear();
            this.Restart();
        }

        private void ConsumeBuffer(int CharCount)
        {
            if (CharCount <= this.m_LookaheadBuffer.Length)
            {
                int num2 = CharCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    Position sysPosition;
                    switch (((char)(this.m_LookaheadBuffer[i] - '\n')))
                    {
                        case '\0':
                            {
                                sysPosition = this.m_SysPosition;
                                sysPosition.Line++;
                                this.m_SysPosition.Column = 0;
                                continue;
                            }
                        case '\x0003':
                            {
                                continue;
                            }
                    }
                    sysPosition = this.m_SysPosition;
                    sysPosition.Column++;
                }
                this.m_LookaheadBuffer = this.m_LookaheadBuffer.Remove(0, CharCount);
            }
        }

        internal short CurrentLALRState()
        {
            return (short)this.m_CurrentLALR;
        }

        public Position CurrentPosition()
        {
            return this.m_CurrentPosition;
        }

        public Token CurrentToken()
        {
            return this.m_InputTokens.Top();
        }

        public Token DiscardCurrentToken()
        {
            return this.m_InputTokens.Dequeue();
        }

        public void EnqueueInput(Token TheToken)
        {
            this.m_InputTokens.Enqueue(TheToken);
        }

        internal SymbolList ExpectedSymbols()
        {
            return this.m_ExpectedSymbols;
        }

        public bool LoadTables(string Path)
        {
            bool flag;
            try
            {
                flag = this.m_Tables.Load(Path);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = false;
                return flag;
            }
            return flag;
        }

        private string Lookahead(int CharIndex)
        {
            if (CharIndex > this.m_LookaheadBuffer.Length)
            {
                int num3 = CharIndex - this.m_LookaheadBuffer.Length;
                for (int i = 1; i <= num3; i++)
                {
                    this.m_LookaheadBuffer = this.m_LookaheadBuffer + Conversions.ToString(Strings.ChrW(this.m_Source.Read()));
                }
            }
            if (CharIndex <= this.m_LookaheadBuffer.Length)
            {
                return Conversions.ToString(this.m_LookaheadBuffer[CharIndex - 1]);
            }
            return "";
        }

        private string LookaheadBuffer(int CharCount)
        {
            string str2 = "";
            if (CharCount <= this.m_LookaheadBuffer.Length)
            {
                str2 = this.m_LookaheadBuffer.Substring(0, CharCount);
            }
            return str2;
        }

        private Token LookaheadDFA()
        {
            throw new NotImplementedException();
        }

        public bool Open(string Text)
        {
            this.Restart();
            this.m_Source = new StringReader(Text);
            return true;
        }

        public bool Open(TextReader Reader)
        {
            this.Restart();
            this.m_Source = Reader;
            return true;
        }

        public ParseMessage Parse()
        {
            ParseMessage tokenRead;
            if (!this.m_Tables.IsLoaded())
            {
                return ParseMessage.NotLoadedError;
            }
            bool flag = false;
            while (!flag)
            {
                Token token;
                if (this.m_InputTokens.Count == 0)
                {
                    token = this.ProduceToken();
                    this.m_InputTokens.Push(token);
                    tokenRead = ParseMessage.TokenRead;
                    flag = true;
                }
                else
                {
                    token = this.m_InputTokens.Top();
                    this.m_CurrentPosition.Copy(token.Position);
                    if (this.m_GroupStack.Count != 0)
                    {
                        tokenRead = ParseMessage.GroupError;
                        flag = true;
                    }
                    else if (token.Type() == SymbolType.Noise)
                    {
                        this.m_InputTokens.Pop();
                    }
                    else if (token.Type() == SymbolType.Error)
                    {
                        tokenRead = ParseMessage.LexicalError;
                        flag = true;
                    }
                    else
                    {
                        ParseResult result = this.ParseLALR(token);
                        switch (((int)result))
                        {
                            case 1:
                                tokenRead = ParseMessage.Accept;
                                flag = true;
                                break;

                            case 2:
                                this.m_InputTokens.Dequeue();
                                tokenRead = ParseMessage.Shift;
                                flag = true;
                                break;

                            case 3:
                                tokenRead = ParseMessage.Reduction;
                                flag = true;
                                break;

                            case 5:
                                tokenRead = ParseMessage.SyntaxError;
                                flag = true;
                                break;

                            case 6:
                                tokenRead = ParseMessage.InternalError;
                                flag = true;
                                break;
                        }
                    }
                }
            }
            return tokenRead;
        }

        private ParseResult ParseLALR(Token NextToken)
        {
            Token token;
            short index;
            Production production;
            ParseResult reduceEliminated;
            LRAction action = this.m_Tables.LALR[this.m_CurrentLALR][NextToken.Parent];
            if (action == null)
            {
                IEnumerator enumerator;
                this.m_ExpectedSymbols.Clear();
                try
                {
                    enumerator = this.m_Tables.LALR[this.m_CurrentLALR].GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        LRAction current = (LRAction)enumerator.Current;
                        if ((current.Symbol.Type == SymbolType.Content) | (current.Symbol.Type == SymbolType.End))
                        {
                            this.m_ExpectedSymbols.Add(current.Symbol);
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
                return ParseResult.SyntaxError;
            }
            this.m_HaveReduction = false;
            switch (((int)action.Type()))
            {
                case 1:
                    this.m_CurrentLALR = action.Value();
                    NextToken.State = (short)this.m_CurrentLALR;
                    this.m_Stack.Push(NextToken);
                    return ParseResult.Shift;

                case 2:
                    production = this.m_Tables.Production[action.Value()];
                    if (!(this.m_TrimReductions & production.ContainsOneNonTerminal()))
                    {
                        short num4;
                        this.m_HaveReduction = true;
                        Reduction data = new Reduction(production.Handle().Count());
                        Reduction reduction2 = data;
                        reduction2.Parent = production;
                        index = (short)(production.Handle().Count() - 1);
                        Label_0147:
                        num4 = 0;
                        if (index >= num4)
                        {
                            reduction2[index] = this.m_Stack.Pop();
                            index = (short)(index + -1);
                            goto Label_0147;
                        }
                        reduction2 = null;
                        token = new Token(production.Head, data);
                        reduceEliminated = ParseResult.ReduceNormal;
                        break;
                    }
                    token = this.m_Stack.Pop();
                    token.Parent = production.Head;
                    reduceEliminated = ParseResult.ReduceEliminated;
                    break;

                case 3:
                    return reduceEliminated;

                case 4:
                    this.m_HaveReduction = true;
                    return ParseResult.Accept;

                default:
                    return reduceEliminated;
            }
            short state = this.m_Stack.Top().State;
            index = this.m_Tables.LALR[state].IndexOf(production.Head);
            if (index != -1)
            {
                this.m_CurrentLALR = this.m_Tables.LALR[state][index].Value();
                token.State = (short)this.m_CurrentLALR;
                this.m_Stack.Push(token);
                return reduceEliminated;
            }
            return ParseResult.InternalError;
        }

        private Token ProduceToken()
        {
            bool flag = false;
            Token token4 = null;
            Token theToken = null;
            while (!flag)
            {
                bool flag2;
                theToken = this.LookaheadDFA();
                if (theToken.Type() == SymbolType.GroupStart)
                {
                    if (this.m_GroupStack.Count == 0)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        flag2 = this.m_GroupStack.Top().Group().Nesting.Contains(theToken.Group().TableIndex);
                    }
                }
                else
                {
                    flag2 = false;
                }
                if (flag2)
                {
                    this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(theToken.Data, null, "Length", new object[0], null, null, null)));
                    this.m_GroupStack.Push(theToken);
                }
                else if (this.m_GroupStack.Count == 0)
                {
                    this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(theToken.Data, null, "Length", new object[0], null, null, null)));
                    token4 = theToken;
                    flag = true;
                }
                else
                {
                    Token token6;
                    if (this.m_GroupStack.Top().Group().End == theToken.Parent)
                    {
                        Token token = this.m_GroupStack.Pop();
                        if (token.Group().Ending == EndingMode.Closed)
                        {
                            token6 = token;
                            token6.Data = Operators.ConcatenateObject(token6.Data, theToken.Data);
                            this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(theToken.Data, null, "Length", new object[0], null, null, null)));
                        }
                        if (this.m_GroupStack.Count == 0)
                        {
                            token.Parent = token.Group().Container;
                            token4 = token;
                            flag = true;
                        }
                        else
                        {
                            token6 = this.m_GroupStack.Top();
                            token6.Data = Operators.ConcatenateObject(token6.Data, token.Data);
                        }
                    }
                    else if (theToken.Type() == SymbolType.End)
                    {
                        token4 = theToken;
                        flag = true;
                    }
                    else
                    {
                        Token token5 = this.m_GroupStack.Top();
                        if (token5.Group().Advance == AdvanceMode.Token)
                        {
                            token6 = token5;
                            token6.Data = Operators.ConcatenateObject(token6.Data, theToken.Data);
                            this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(theToken.Data, null, "Length", new object[0], null, null, null)));
                        }
                        else
                        {
                            token6 = token5;
                            token6.Data = Operators.ConcatenateObject(token6.Data, NewLateBinding.LateGet(theToken.Data, null, "Chars", new object[] { 0 }, null, null, null));
                            this.ConsumeBuffer(1);
                        }
                    }
                }
            }
            return token4;
        }

        public void PushInput(Token TheToken)
        {
            this.m_InputTokens.Push(TheToken);
        }

        public void Restart()
        {
            this.m_SysPosition.Column = 0;
            this.m_SysPosition.Line = 0;
            this.m_CurrentPosition.Line = 0;
            this.m_CurrentPosition.Column = 0;
            this.m_InputTokens.Clear();
            this.m_LookaheadBuffer = "";
            this.m_GroupStack.Clear();
            this.m_CurrentLALR = this.m_Tables.LALR.InitialState;
            this.m_Stack.Clear();
            Token theToken = new Token
            {
                State = this.m_Tables.LALR.InitialState
            };
            this.m_Stack.Push(theToken);
            this.m_HaveReduction = false;
            this.m_ExpectedSymbols.Clear();
        }

        public bool TablesLoaded()
        {
            return this.m_Tables.IsLoaded();
        }

        // Properties
        public object CurrentReduction
        {
            get
            {
                if (this.m_HaveReduction)
                {
                    return RuntimeHelpers.GetObjectValue(this.m_Stack.Top().Data);
                }
                return null;
            }
            set
            {
                if (this.m_HaveReduction)
                {
                    this.m_Stack.Top().Data = RuntimeHelpers.GetObjectValue(value);
                }
            }
        }

        internal ParseTables Tables
        {
            get
            {
                return this.m_Tables;
            }
            set
            {
                this.m_Tables = value;
            }
        }

        public bool TrimReductions
        {
            get
            {
                return this.m_TrimReductions;
            }
            set
            {
                this.m_TrimReductions = value;
            }
        }

        // Nested Types
        private enum ParseResult
        {
            Accept = 1,
            InternalError = 6,
            ReduceEliminated = 4,
            ReduceNormal = 3,
            Shift = 2,
            SyntaxError = 5
        }
    }
}