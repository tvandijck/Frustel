using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel
{
    public sealed class SymbolNonTerminal : Symbol
    {
        public SymbolNonTerminal(string name)
        {
            Name = name;
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.NonTerminal; }
        }

        public override string ToString()
        {
            return $"<{Name}>";
        }
    }
}
