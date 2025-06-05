using Antlr4.Runtime;
using System;

public class VerboseListener : BaseErrorListener
{
    public override void SyntaxError(
        TextWriter output, IRecognizer recognizer,
        IToken offendingSymbol, int charPositionInLine, int i,
        string msg, RecognitionException recognitionException)
    {
        Console.WriteLine($"[Syntax Error] Línea {offendingSymbol.Line}, Columna {charPositionInLine}: {msg}");
    }
}