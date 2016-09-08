using System;
using System.ComponentModel;
using System.IO;

namespace Frutsel
{
    class Program
    {
        static void Main(string[] args)
        {
            var grammar = new GoldParserGrammar();

            // now build the DFA.
            var tables = grammar.Build();

            // test the parser.
            var text = File.ReadAllText("E:\\test.grm");

            Parser parser = new Parser(tables);
            if (parser.Open(text))
            {
                for (;;)
                {
                    EParseMessage msg = parser.Parse();
                    switch (msg)
                    {
                        case EParseMessage.TokenRead:
                            break;
                        case EParseMessage.Reduction:
                            break;
                        case EParseMessage.Accept:
                            Console.WriteLine("Accept");
                            return;

                        case EParseMessage.NotLoadedError:
                            throw new Exception("NotLoadedError");
                        case EParseMessage.LexicalError:
                            throw new Exception("LexicalError");
                        case EParseMessage.SyntaxError:
                            throw new Exception("SyntaxError");
                        case EParseMessage.GroupError:
                            throw new Exception("GroupError");
                        case EParseMessage.InternalError:
                            throw new Exception("InternalError");
                    }
                }
            }
        }
    }
}
