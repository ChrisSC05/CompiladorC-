using AlphaCompiler.Generation;
using AlphaCompiler.Semantics;
using Antlr4.Runtime;

namespace AlphaCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var filePath = args.Length > 0 ? args[0] : "test.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Archivo no encontrado: {filePath}");
                return;
            }

            var code = File.ReadAllText(filePath);
            //Console.WriteLine("=== Contenido leído ===");
            //Console.WriteLine(code);
            //Console.WriteLine("=======================");

            // 1. Lexer
            var input  = new AntlrInputStream(code);
            var lexer  = new AlphaScanner(input);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();
            foreach (var token in tokens.GetTokens())
            {
                //Console.WriteLine(token.ToString());
            }
            // 2. Parser
            var parser = new AlphaParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new DiagnosticErrorListener());
            parser.AddErrorListener(new ConsoleErrorListener<IToken>());
            //parser.Trace = true;
            var tree   = parser.program(); // este es el punto de entrada

            // 3. Verificamos si hubo errores léxicos o sintácticos
            if (parser.NumberOfSyntaxErrors > 0)
            {
                Console.WriteLine("Se encontraron errores de sintaxis.");
                return;
            }

            // 4. Análisis semántico
            var builder = new SymbolTableBuilder();
            builder.Visit(tree);

            if (builder.Errors.Count == 0)
            {
                //Console.WriteLine("✔️ No se encontraron errores semánticos.");
                //builder.DumpSymbols(); // Muestra tabla de símbolos si lo deseas
                //builder.DumpMemory();
                var generator = new CodeGenerator(builder.Symbols);
                var outputCode = generator.Visit(tree);
                Console.WriteLine("Código generado:");
                Console.WriteLine(outputCode);
            }
            else
            {
                Console.WriteLine("Errores encontrados:");
                foreach (var err in builder.Errors)
                    Console.WriteLine(" - " + err);
            }

            // 5. Imprime el árbol si quieres depurar
            //Console.WriteLine("Árbol de análisis:");
            //Console.WriteLine(tree.ToStringTree(parser));
        }
    }
}