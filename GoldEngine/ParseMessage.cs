namespace GoldEngine
{
    public enum ParseMessage
    {
        TokenRead,
        Reduction,
        Accept,
        NotLoadedError,
        LexicalError,
        SyntaxError,
        GroupError,
        InternalError,
        Shift
    }
}