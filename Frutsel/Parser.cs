using System;
using System.Collections.Generic;

namespace Frutsel
{
    class Parser
    {
        private readonly GrammarTables m_tables;
        private readonly Stack<Token> m_groupStack = new Stack<Token>();
        private readonly LinkedList<Token> m_inputTokens = new LinkedList<Token>();

        private string m_source;
        private int m_sourcePtr;
        private int m_lookaheadPtr;
        private int m_lookaheadLength;

        public Parser(GrammarTables tables)
        {
            m_tables = tables;
        }

        public bool Open(string text)
        {
            m_source = text;
            m_sourcePtr = 0;
            m_lookaheadPtr = 0;
            m_lookaheadLength = 0;
            return true;
        }

        public EParseMessage Parse()
        {
            for (;;)
            {
                if (m_inputTokens.Count <= 0)
                {
                    var read = ProduceToken();
                    m_inputTokens.AddLast(read);
                    return EParseMessage.TokenRead;
                }
                else
                {
                    var read = m_inputTokens.Last.Value;
                    if (m_groupStack.Count > 0)
                    {
                        // Runaway group
                        return EParseMessage.GroupError;
                    }
                    else if (read.Parent.Type == ESymbolType.Noise)
                    {
                        // Just discard. These were already reported to the user.
                        m_inputTokens.RemoveLast();
                    }
                    else if (read.Parent.Type == ESymbolType.Error)
                    {
                        return EParseMessage.LexicalError;
                    }
                    else if (read.Parent.Type == ESymbolType.End)
                    {
                        return EParseMessage.Accept;
                    }
                    else {
                        // Finally, we can parse the token.
                        Console.WriteLine("{0} : {1}", read.Parent, read.Data);
                        m_inputTokens.RemoveLast();

                        return EParseMessage.Reduction;
                    }
                }
            }
        }

        private Token ProduceToken()
        {
            bool nestGroup;
            for (;;)
            {
                var read = LookaheadDFA();
                if (read == null)
                    return null;

                // The logic - to determine if a group should be nested - requires that the top of the stack 
                // and the symbol's linked group need to be looked at. Both of these can be unset. So, this section
                // sets a Boolean and avoids errors. We will use this boolean in the logic chain below. 
                if (read.Parent.Type == ESymbolType.GroupStart)
                {
                    if (m_groupStack.Count <= 0)
                    {
                        nestGroup = true;
                    }
                    else
                    {
                        nestGroup = m_groupStack.Peek().Parent.Group.Nesting.Contains(read.Parent.Group);
                    }
                }
                else
                {
                    nestGroup = false;
                }

                // =================================
                //  Logic chain
                // =================================

                if (nestGroup)
                {
                    ConsumeBuffer(read.Data.Length);
                    m_groupStack.Push(read);
                }
                else if (m_groupStack.Count <= 0)
                {
                    // The token is ready to be analyzed.
                    ConsumeBuffer(read.Data.Length);
                    return read;
                }
                else if (m_groupStack.Peek().Parent.Group.End == read.Parent)
                {
                    // End the current group
                    Token pop = m_groupStack.Pop();

                    // === Ending logic
                    if (pop.Parent.Group.Ending == EEndingMode.ClosedEndingMode)
                    {
                        pop.Data += read.Data;            // Append text
                        ConsumeBuffer(read.Data.Length);  // Consume token
                    }

                    if (m_groupStack.Count <= 0)
                    {
                        // We are out of the group. Return pop'd token (which contains all the group text)
                        pop.Parent = pop.Parent.Group.Container;      // Change symbol to parent
                        return pop;
                    }

                    m_groupStack.Peek().Data += pop.Data;     // Append group text to parent
                }
                else if (read.Parent.Type == ESymbolType.End)
                {
                    // EOF always stops the loop. The caller function (Parse) can flag a runaway group error.
                    return read;
                }
                else {
                    // We are in a group, Append to the Token on the top of the stack.
                    // Take into account the Token group mode  
                    var top = m_groupStack.Peek();
                    if (top.Parent.Group.Advance == EAdvanceMode.TokenAdvanceMode)
                    {
                        top.Data += read.Data;             // Append all text
                        ConsumeBuffer(read.Data.Length);
                    }
                    else
                    {
                        top.Data += read.Data[0];    // Append one character
                        ConsumeBuffer(1);
                    }
                }
            }
        }

        public Token LookaheadDFA()
        {
            Token result = new Token();
            DFAState target = null;
            DFAState lastAcceptState = null;
            DFAState currentDFA = m_tables.DFA[m_tables.DfaStartState];

            int currentPosition = 1;
            int lastAcceptPosition = -1;

            char ch = Lookahead(1);
            if ((ch != 0) && (ch != 65536))
            {
                bool done = false;
                while (!done)
                {
                    // This code searches all the branches of the current DFA state
                    // for the next character in the input Stream. if found the
                    // target state is returned.
                    ch = Lookahead(currentPosition);
                    bool found;
                    if (ch == 0)
                    {
                        // End reached, do not match
                        found = false;
                    }
                    else
                    {
                        found = false;
                        foreach (var edge in currentDFA.Edges)
                        {
                            // ==== Look for character in the Character Set Table
                            if (edge.Characters.Contains(ch))
                            {
                                found = true;
                                target = m_tables.DFA[edge.Target];
                            }
                        }
                    }

                    //  This block-if statement checks whether an edge was found from the current state. if so, the state and current
                    //  position advance. Otherwise it is time to exit the main loop and report the token found (if there was one). 
                    //  if the LastAcceptState is -1, then we never found a match and the Error Token is created. Otherwise, a new 
                    //  token is created using the Symbol in the Accept State and all the characters that comprise it.
                    if (found)
                    {
                        // This code checks whether the target state accepts a token.
                        // if so, it sets the appropiate variables so when the
                        // algorithm in done, it can return the proper token and
                        // number of characters.
                        if (target.Accept != null)
                        {
                            // NOT is very important!
                            lastAcceptState = target;
                            lastAcceptPosition = currentPosition;
                        }

                        currentDFA = target;
                        currentPosition++;
                    }
                    else
                    {
                        // No edge found
                        done = true;
                        if (lastAcceptState == null)
                        {
                            // Lexer cannot recognize symbol
                            result.Parent = GetFirstOfType(ESymbolType.Error);
                            result.Data = LookaheadBuffer(1);
                        }
                        else {
                            // Create Token, read characters
                            result.Parent = lastAcceptState.Accept;
                            result.Data = LookaheadBuffer(lastAcceptPosition);  // Data contains the total number of accept characters
                        }
                    }
                    // DoEvents
                }
            }
            else {
                // End of file reached, create End Token
                result.Parent = GetFirstOfType(ESymbolType.End);
                result.Data = string.Empty;
            }

            return result;
        }

        private Symbol GetFirstOfType(ESymbolType type)
        {
            foreach (var symbol in m_tables.Symbols)
            {
                if (symbol.Type == type)
                    return symbol;
            }
            return null;
        }

        private char Lookahead(int charIndex)
        {
            // Return single char at the index. This function will also increase 
            // buffer if the specified character is not present. It is used 
            // by the DFA algorithm.

            // Check if we must read characters from the Stream
            if (charIndex > m_lookaheadLength)
            {
                var readCount = charIndex - m_lookaheadLength;
                var maxRead = m_source.Length - m_sourcePtr;

                readCount = Math.Min(maxRead, readCount);
                m_lookaheadLength += readCount;
                m_sourcePtr += readCount;
            }

            // if the buffer is still smaller than the index, we have reached
            // the end of the text. In this case, return a null string - the DFA
            // code will understand.
            if (charIndex <= m_lookaheadLength)
            {
                return m_source[m_lookaheadPtr + charIndex - 1];
            }

            return '\0';
        }

        private string LookaheadBuffer(int count)
        {
            // Return Count characters from the lookahead buffer. DO NOT CONSUME
            // This is used to create the text stored in a token. It is disgarded
            // separately. Because of the design of the DFA algorithm, count should
            // never exceed the buffer length. The If-Statement below is fault-tolerate
            // programming, but not necessary.
            if (count > m_lookaheadLength)
            {
                count = m_lookaheadLength;
            }
            return m_source.Substring(m_lookaheadPtr, count);
        }

        private void ConsumeBuffer(int charCount)
        {
            // Consume/Remove the characters from the front of the buffer. 
            if (charCount <= m_lookaheadLength)
            {
                m_lookaheadPtr += charCount;
                m_lookaheadLength -= charCount;
            }
        }
    }
}