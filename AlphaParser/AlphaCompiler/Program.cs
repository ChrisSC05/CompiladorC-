using AlphaCompiler.Semantics;
using Antlr4.Runtime;

namespace AlphaCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var filePath = args.Length > 0 ? args[0] : "test.alpha";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Archivo no encontrado: {filePath}");
                return;
            }

            var code = File.ReadAllText(filePath);
            // === 1. Escaneo (lexer) ===
            var input  = new AntlrInputStream(code);
            var lexer  = new AlphaScanner(input);
            var tokens = new CommonTokenStream(lexer);

            // === 2. Análisis sintáctico (parser) ===
            var parser = new AlphaParser(tokens);
            var tree   = parser.program();

            // === 3. Construcción de la tabla de símbolos ===
            var builder = new SymbolTableBuilder();
            builder.Visit(tree); // donde `tree` es el árbol de análisis generado por ANTLR

            if (builder.Errors.Count == 0)
            {
                Console.WriteLine("✔️ No se encontraron errores semánticos.");
                //builder.DumpSymbols(); // Mostramos la tabla de símbolos
                
            }
            else
            {
                Console.WriteLine("Errores encontrados:");
                foreach (var err in builder.Errors)
                    Console.WriteLine(" - " + err);
            }

            // Opcional: mostrar árbol de análisis (LISP-style)
            //Console.WriteLine(tree.ToStringTree(parser));
        }
    }
}