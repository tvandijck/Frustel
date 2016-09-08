namespace GoldEngine
{
    public class Position
    {
        public int Column;
        public int Line;

        public Position()
        {
            Line = 0;
            Column = 0;
        }

        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        internal void Copy(Position pos)
        {
            Column = pos.Column;
            Line = pos.Line;
        }
    }
}