using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frutsel
{
    public sealed class SymbolGroupStart : SymbolTerminal
    {
        public SymbolGroupStart()
        {
        }

        public SymbolGroupStart(string name, Expression expression)
            : base(name, expression)
        {
        }

        public override ESymbolType Type
        {
            get { return ESymbolType.GroupStart; }
        }
    }
}
