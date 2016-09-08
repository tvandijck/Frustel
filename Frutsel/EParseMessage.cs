namespace Frutsel
{
    public enum EParseMessage
    {
        TokenRead = 0,         // A new token is read
        Reduction = 1,         // A production is reduced
        Accept = 2,            // Grammar complete
        NotLoadedError = 3,    // The tables are not loaded
        LexicalError = 4,      // Token not recognized
        SyntaxError = 5,       // Token is not expected
        GroupError = 6,        // Reached the end of the file inside a block
        InternalError = 7,     // Something is wrong, very wrong
    }
}