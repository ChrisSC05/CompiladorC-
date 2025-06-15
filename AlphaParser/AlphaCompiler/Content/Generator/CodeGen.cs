using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AlphaCompiler.Semantics;
using Antlr4.Runtime.Misc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AlphaCompiler.Generation
{
    public abstract class IRNode { public abstract string Emit(); }

    public class IRProgram : IRNode
    {
        public List<IRStatement> Statements = new();

        public override string Emit()
        {
            var lines = new List<string>
            {
                "using System;",
                "using System.Collections.Generic;",
                "using System.Linq;",
                "",
                "public class Program",
                "{",
                "    public static List<T> add<T>(List<T> list, T item) { list.Add(item); return list; }",
                "    public static int len<T>(List<T> list) => list.Count;",
                "    public static void del<T>(List<T> list, int index) => list.RemoveAt(index);",
                "    public static int ord(char c) => (int)c;",
                "    public static char chr(int i) => (char)i;",
                "",
                "    public static void Main()",
                "    {"
            };

            foreach (var stmt in Statements)
            {
                var code = stmt.Emit();
                //Console.WriteLine($"[Emit Statement]: {code}");
                lines.Add("        " + code);
            }

            lines.Add("    }");
            lines.Add("}");

            return string.Join("\n", lines);
        }
    }
    
    public abstract class IRExpression
    {
        public abstract string Emit();
    }   
    
    public abstract class IRStatement : IRNode { }

    public class IRPrint : IRStatement
    {
        public string Expression;
        public IRPrint(string expr) => Expression = expr;

        public override string Emit()
        {
            if (Expression.StartsWith("arr") || Expression.StartsWith("list") || Expression.StartsWith("Program.add"))
            {
                return $"Console.WriteLine(\"[\" + string.Join(\", \", {Expression}) + \"]\");";
            }

            return $"Console.WriteLine({Expression});";
        }
    }
    
    public class IRIf : IRStatement
    {
        public string Condition { get; }
        public List<IRStatement> ThenStatements { get; } = new();
        public List<IRStatement> ElseStatements { get; } = new();

        public IRIf(string condition)
        {
            Condition = condition;
        }

        public override string Emit()
        {
            
            var lines = new List<string>
            {
                $"if {Condition}",
                "{"
            };

            lines.AddRange(ThenStatements.Select(stmt => "    " + stmt.Emit()));
            lines.Add("}");

            if (ElseStatements.Count > 0)
            {
                lines.Add("else");
                lines.Add("{");
                lines.AddRange(ElseStatements.Select(stmt => "    " + stmt.Emit()));
                lines.Add("}");
            }

            return string.Join("\n", lines);
        }
    }

    public class IRWhile : IRStatement
    {
        public string ConditionText { get; }
        public List<IRStatement> BodyStatements { get; }

        public IRWhile(string conditionText, List<IRStatement> bodyStatements)
        {
            ConditionText = conditionText;
            BodyStatements = bodyStatements;
        }

        public override string Emit()
        {
            var bodyCode = string.Join("\n", BodyStatements.Select(s => s.Emit()));
            return $"while ({ConditionText}) {{\n{bodyCode}\n}}";
        }
    }

    
    public class IRAssignment : IRStatement
    {
        public string Target;
        public string Expression;
        public IRAssignment(string target, string expr) { Target = target; Expression = expr; }
        public override string Emit()
        {
            var code = $"{Target} = {Expression};";
            //Console.WriteLine($"[Emit Assignment]: {code}");
            return code;
        }
    }

    public class IRVarDecl : IRStatement
    {
        public string Name;
        public string Type;
        public bool IsArray;

        public IRVarDecl(string name, string type, bool isArray = false)
        {
            Name = name;
            Type = type;
            IsArray = isArray;
        }

        public override string Emit()
        {
            if (IsArray)
            {
                // Convertir tipo "int[]" a "int"
                var typeClean = Type.Replace("[]", "").Trim();
                return $"List<{typeClean}> {Name} = new List<{typeClean}>();";
            }
            else
            {
                return $"{Type} {Name};";
            }
        }
    }

    // Nueva clase para representar llamadas a funciones (built-ins o normales)
    public class IRCall : IRNode
    {
        public string MethodName;
        public List<string> Arguments;

        public IRCall(string methodName, List<string> args)
        {
            MethodName = methodName;
            Arguments = args;
        }

        public override string Emit()
        {
            var builtins = new HashSet<string> { "add", "len", "del", "ord", "chr" };
            if (builtins.Contains(MethodName))
                return $"Program.{MethodName}({string.Join(", ", Arguments)})";

            return $"{MethodName}({string.Join(", ", Arguments)})";
        }

        public override string ToString() => Emit();
    }
    
    public class IRExprStmt : IRStatement
    {
        public IRCall Call;

        public IRExprStmt(IRCall call)
        {
            Call = call;
        }

        public override string Emit()
        {
            return Call.Emit() + ";";
        }
    }

    
    public class IRBuilder : AlphaParserBaseVisitor<object?>
    {
        public readonly IRProgram Program = new();

        public override object? VisitProgram(AlphaParser.ProgramContext context)
        {
            foreach (var cls in context.classDecl())
                Visit(cls);
            return null;
        }

        public override object? VisitClassDecl(AlphaParser.ClassDeclContext context)
        {
            foreach (var decl in context.classBody().varDecl())
                Visit(decl);
            foreach (var method in context.classBody().methodDecl())
                Visit(method);
            return null;
        }

        public override object? VisitVarDecl(AlphaParser.VarDeclContext context)
        {
            var type = context.type().GetText();
            var isArray = context.children.Any(c => c.GetText().Contains("["));
            foreach (var id in context.IDENT())
            {
                Program.Statements.Add(new IRVarDecl(id.GetText(), type, isArray));
            }
            return null;
        }

        public override object? VisitMethodDecl(AlphaParser.MethodDeclContext context)
        {
            return Visit(context.block());
        }

        public override object? VisitBlock(AlphaParser.BlockContext context)
        {
            foreach (var v in context.varDecl())
                Visit(v);
            foreach (var s in context.statement())
                Visit(s);
            return null;
        }
        
        public override object? VisitIfStatement(AlphaParser.IfStatementContext context)
        {
            Console.WriteLine("⚙️ Visitando IF");
            Console.WriteLine("Tiene ELSE? " + (context.ELSE() != null));
            Console.WriteLine("Condición: " + context.condition()?.GetText());

            var irIf = new IRIf(context.condition().GetText());

            var thenBlock = context.block(0);
            if (thenBlock == null)
            {
                Console.WriteLine(" thenBlock es null");
            }
            else
            {
                foreach (var stmt in thenBlock.statement())
                {
                    var ir = Visit(stmt);
                    Console.WriteLine("Sentencia then visitada: " + ir);
                    irIf.ThenStatements.Add((IRStatement)ir);
                }
            }

            if (context.ELSE() != null)
            {
                var elseBlock = context.block(1);
                if (elseBlock == null)
                {
                    Console.WriteLine(" elseBlock es null");
                }
                else
                {
                    foreach (var stmt in elseBlock.statement())
                    {
                        var ir = Visit(stmt);
                        Console.WriteLine("✔️ Sentencia else visitada: " + ir);
                        irIf.ElseStatements.Add((IRStatement)ir);
                    }
                }
            }

            return irIf;
        }
        
        public override object VisitWhileStatement(AlphaParser.WhileStatementContext context)
        {
            Console.WriteLine("⚙️ Visitando WHILE");
            var conditionText = context.condition()?.GetText();

            if (string.IsNullOrEmpty(conditionText))
                throw new Exception("❌ La condición del WHILE es null o vacía");

            Console.WriteLine($"Condición del WHILE: {conditionText}");

            // Guardamos las instrucciones que se agreguen para el cuerpo del while
            var bodyStatements = new List<IRStatement>();
            var oldStatements = Program.Statements;

            // Redirigimos las instrucciones para que se agreguen al cuerpo del while
            Program.Statements = bodyStatements;

            if (context.statement() == null)
                throw new Exception("❌ El cuerpo del WHILE es null");

            // Visitamos el cuerpo para llenar bodyStatements
            Visit(context.statement());

            // Restauramos la lista original
            Program.Statements = oldStatements;

            // Agregamos el IRWhile con condición en texto y cuerpo
            Program.Statements.Add(new IRWhile(conditionText, bodyStatements));

            return null;
        }




        
        public override object? VisitAssignStatement(AlphaParser.AssignStatementContext context)
        {
            var target = context.designator().GetText();
            var exprObj = Visit(context.expr());

            string expr;
            //Console.WriteLine($"[VisitAssignStatement] Target: {target}, Expr: {exprObj?.ToString() ?? "null"}");
            if (exprObj is IRCall call)
            {
                expr = call.Emit();
                //Console.WriteLine("If case in VisitAssignStatement");
                //Console.WriteLine($"[VisitAssignStatement] Target: {target}, Expr: {expr}");
                if (call.MethodName == "add")
                {
                    // add devuelve la lista modificada, asignamos para conservar el cambio
                    expr = $"Program.add({string.Join(", ", call.Arguments)})";
                    Program.Statements.Add(new IRAssignment(target, expr));
                    return null;
                }
            }
            else
            {
                expr = exprObj?.ToString() ?? "default";
                //Console.WriteLine("Else case in VisitAssignStatement");
                //Console.WriteLine($"[VisitAssignStatement] Target: {target}, Expr: {expr}");
            }

            Program.Statements.Add(new IRAssignment(target, expr));
            return null;
        }

        public override object? VisitPrintStmt(AlphaParser.PrintStmtContext context)
        {
            var exprObj = Visit(context.expr());
            string expr;
            if (exprObj is IRCall call)
                expr = call.Emit();
            else
                expr = exprObj?.ToString() ?? "default";

            var irPrint = new IRPrint(expr);
            Program.Statements.Add(irPrint);
            return irPrint;  // Devuelve el nodo IR que representa esta sentencia
        }
        
        public override object? VisitCallStatement(AlphaParser.CallStatementContext context)
        {
            var name = context.designator().GetText();
            var args = context.actPars()?.expr()
                .Select(e =>
                {
                    var res = Visit(e);
                    return res is IRCall c ? c.Emit() : res?.ToString() ?? "default";
                })
                .ToList() ?? new List<string>();

            var call = new IRCall(name, args);

            if (name == "add")
            {
                var arrayName = args[0];
                var exprCode = call.Emit();
                Program.Statements.Add(new IRAssignment(arrayName, exprCode));
            }
            else if (name == "del")
            {
                Program.Statements.Add(new IRExprStmt(call));
            }
            else
            {
                Program.Statements.Add(new IRExprStmt(call));
            }

            return null;
        }

        
        public override object? VisitBinaryExpr(AlphaParser.BinaryExprContext context)
        {
            var left = Visit(context.term(0))?.ToString() ?? "0";
            for (int i = 0; i < context.addop().Length; i++)
            {
                var op = context.addop(i).GetText();
                var right = Visit(context.term(i + 1))?.ToString() ?? "0";
                left = $"({left} {op} {right})";
            }
            return left;
        }

        public override object? VisitTermExpr(AlphaParser.TermExprContext context)
        {
            var left = Visit(context.factor(0))?.ToString() ?? "1";
            for (int i = 0; i < context.mulop().Length; i++)
            {
                var op = context.mulop(i).GetText();
                var right = Visit(context.factor(i + 1))?.ToString() ?? "1";
                left = $"({left} {op} {right})";
            }
            return left;
        }

        public override object? VisitIntFactor(AlphaParser.IntFactorContext context) => context.INTLITERAL().GetText();
        public override object? VisitDoubleFactor(AlphaParser.DoubleFactorContext context) => context.DOUBLELITERAL().GetText();
        public override object? VisitCharFactor(AlphaParser.CharFactorContext context) => context.CHARLITERAL().GetText();
        public override object? VisitBoolFactor(AlphaParser.BoolFactorContext context) => context.BOOLEANLITERAL().GetText();
        public override object? VisitStringFactor(AlphaParser.StringFactorContext context) => context.STRINGLITERAL().GetText();
        public override object? VisitDesignatorFactor(AlphaParser.DesignatorFactorContext context) => context.designator().GetText();
        public override object? VisitGroupFactor(AlphaParser.GroupFactorContext context) => "(" + Visit(context.expr()) + ")";
    
        public override object? VisitCondFact(AlphaParser.CondFactContext context)
        {
            var left = Visit(context.expr(0));
            var op = context.relop().GetText();
            var right = Visit(context.expr(1));
            return $"({left} {op} {right})";
        }

        public override object? VisitCondTerm(AlphaParser.CondTermContext context)
        {
            var left = Visit(context.condFact(0));
            for (int i = 1; i < context.condFact().Length; i++)
            {
                var right = Visit(context.condFact(i));
                left = $"({left} && {right})";
            }
            return left;
        }

        public override object? VisitCondition(AlphaParser.ConditionContext context)
        {
            var left = Visit(context.condTerm(0));
            for (int i = 1; i < context.condTerm().Length; i++)
            {
                var right = Visit(context.condTerm(i));
                left = $"({left} || {right})";
            }
            return left;
        }
        
        
        
        public override object? VisitCallFactor(AlphaParser.CallFactorContext context)
        {
            var name = context.designator().GetText();
            var args = context.actPars()?.expr()
                .Select(e => 
                    {
                        var res = Visit(e);
                        // Si el resultado es IRCall, llamar Emit() para cadena
                        if (res is IRCall c) return c.Emit();
                        return res?.ToString() ?? "default";
                    })
                .ToList() ?? new List<string>();

            // Retornar nodo IRCall en vez de solo string
            return new IRCall(name, args);
        }

        public override object? VisitNewArrayFactor(AlphaParser.NewArrayFactorContext context)
        {
            var type = context.type().GetText();
            var sizeExpr = Visit(context.expr())?.ToString() ?? "0";

            string defaultVal = type switch
            {
                "int" => "0",
                "char" => "'\\0'",
                "string" => "\"\"",
                "double" => "0.0",
                "bool" => "false",
                _ => "default"
            };

            return $"Enumerable.Repeat({defaultVal}, {sizeExpr}).ToList()";
        }
    }

    public static class CodeRunner
    {
        public static string CompileAndRun(string fullCode)
        {
            Console.WriteLine("\n=== Código Generado ===\n");
            //Console.WriteLine(fullCode);
            Console.WriteLine("\n========================\n");

            var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);

            var refs = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>();

            var compilation = CSharpCompilation.Create("GeneratedApp")
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(refs)
                .AddSyntaxTrees(syntaxTree);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                Console.WriteLine("=== Errores de compilación ===");
                foreach (var diag in result.Diagnostics)
                    Console.WriteLine(diag.ToString());
                Console.WriteLine("===============================");
                return string.Join("\n", result.Diagnostics.Select(d => d.ToString()));
            }

            ms.Seek(0, SeekOrigin.Begin);
            var asm = Assembly.Load(ms.ToArray());

            var originalOut = Console.Out;
            var sw = new StringWriter();
            try
            {
                Console.SetOut(sw);
                asm.EntryPoint?.Invoke(null, null);
            }
            catch (Exception ex)
            {
                return $" Excepción durante ejecución:\n{ex.InnerException?.Message ?? ex.Message}";
            }
            finally
            {
                Console.SetOut(originalOut); // Restaurar la salida original SIEMPRE
            }

            return sw.ToString();
        }
    }

}
