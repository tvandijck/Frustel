using System;

namespace GoldEngine
{
    public class ParserException : Exception
    {
        // Fields
        public string Method;

        // Methods
        public ParserException(string Message) : base(Message)
        {
            this.Method = "";
        }

        public ParserException(string Message, Exception Inner, string Method) : base(Message, Inner)
        {
            this.Method = Method;
        }
    }
}