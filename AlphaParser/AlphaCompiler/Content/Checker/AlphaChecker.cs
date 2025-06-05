using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;                // IToken
using Antlr4.Runtime.Tree;
using AlphaCompiler.Semantics;      // Tu namespace donde están SymbolTable, ITypeInfo, etc.

namespace AlphaCompiler.Semantics
{
    /// <summary>
    ///  SymbolTableBuilder revisado:
    ///  - Se construye la tabla de símbolos (clases, variables, parámetros, métodos).
    ///  - Se inicializa la memoria para variables "int".
    ///  - Se hacen validaciones de tipos en asignaciones y expresiones aritméticas.
    ///  - Se corrige la evaluación de sumas/restas (incluido el signo unario).
    /// </summary>
    public sealed class SymbolTableBuilder : AlphaParserBaseVisitor<object?>
    {
        private readonly SymbolTable _symtab = new();
        private readonly IList<string> _errors = new List<string>();

        // 💾 “Memoria” simulada para valores de variables enteras
        private readonly Dictionary<string, int> _memory = new();

        private bool _inClass = false;
        private bool _inMethod = false;

        public IReadOnlyList<string> Errors => (IReadOnlyList<string>)_errors;
        public SymbolTable Symbols => _symtab;

        private void Error(string msg) => _errors.Add(msg);

        private static (int line, int col) Pos(IToken tok) => (tok.Line, tok.Column);

        private ITypeInfo GetTypeFromCtx(AlphaParser.TypeContext ctx)
        {
            var name = ctx.IDENT().GetText();
            var baseType = PrimitiveOrClass(name, ctx.Start);
            return ctx.LBRACK() is not null ? new ArrayTypeInfo(baseType) : baseType;
        }

        private ITypeInfo PrimitiveOrClass(string name, IToken pos)
        {
            return name switch
            {
                "int" or "float" or "bool" or "char" or "string" => new PrimitiveTypeInfo(name),
                _ => _symtab.Resolve(name) is ClassSymbol
                        ? new ClassTypeInfo(name)
                        : new PrimitiveTypeInfo(name) // provisional si no encuentra la clase
            };
        }

        // ---------------------------------------------------
        //  VISITORS PARA CREAR TABLA DE SÍMBOLOS E INICIALIZAR MEMORIA
        // ---------------------------------------------------

        public override object? VisitProgram(AlphaParser.ProgramContext ctx)
        {
            // Recorremos todos los hijos: clases, etc.
            foreach (var child in ctx.children)
                Visit(child);
            return null;
        }

        public override object? VisitClassDecl(AlphaParser.ClassDeclContext ctx)
        {
            var (ln, col) = Pos(ctx.IDENT().Symbol);
            var className = ctx.IDENT().GetText();
            var cls = new ClassSymbol(className, new Dictionary<string, Symbol>(), ln, col);
            _symtab.Add(cls, Error);

            _inClass = true;
            _symtab.OpenScope();   // nuevo scope para el cuerpo de la clase

            foreach (var vd in ctx.varDecl())
                Visit(vd);

            foreach (var md in ctx.methodDecl())
                Visit(md);

            _symtab.CloseScope();
            _inClass = false;
            return null;
        }

        public override object? VisitVarDecl(AlphaParser.VarDeclContext ctx)
        {
            var type = GetTypeFromCtx(ctx.type());
            foreach (var id in ctx.IDENT())
            {
                var (ln, col) = Pos(id.Symbol);
                var sym = new VarSymbol(id.GetText(), type, _inClass && !_inMethod, ln, col);
                _symtab.Add(sym, Error);

                // Si es variable de tipo int, inicializamos en 0
                if (type.Name == "int")
                    _memory[id.GetText()] = 0;
            }
            return null;
        }

        public override object? VisitMethodDecl(AlphaParser.MethodDeclContext ctx)
        {
            var tok = ctx.IDENT().Symbol;
            var retType = ctx.VOID() is null 
                          ? GetTypeFromCtx(ctx.type()) 
                          : new PrimitiveTypeInfo("void");

            var meth = new MethodSymbol(
                ctx.IDENT().GetText(),
                retType,
                new List<ParamSymbol>(),
                tok.Line,
                tok.Column);
            _symtab.Add(meth, Error);

            _inMethod = true;
            _symtab.OpenScope();   // scope para parámetros y variables locales

            if (ctx.formPars() is { } fp)
            {
                for (int i = 0; i < fp.IDENT().Length; i++)
                {
                    var pTok = fp.IDENT(i).Symbol;
                    var pSym = new ParamSymbol(
                        fp.IDENT(i).GetText(),
                        GetTypeFromCtx(fp.type(i)),
                        pTok.Line,
                        pTok.Column);
                    meth.Params.Add(pSym);
                    _symtab.Add(pSym, Error);

                    // Si el parámetro es int, inicializamos en 0
                    if (pSym.Type.Name == "int")
                        _memory[pSym.Name] = 0;
                }
            }

            Visit(ctx.block());
            _symtab.CloseScope();
            _inMethod = false;
            return null;
        }

        public override object? VisitBlock(AlphaParser.BlockContext ctx)
        {
            _symtab.OpenScope();
            foreach (var vd in ctx.varDecl())
                Visit(vd);
            foreach (var stmt in ctx.statement())
                Visit(stmt);
            _symtab.CloseScope();
            return null;
        }

        public override object? VisitDesignator(AlphaParser.DesignatorContext ctx)
        {
            var nameTok = ctx.IDENT(0).Symbol;

            // Si es “print(…)” no lo consideramos variable
            if (nameTok.Text == "print")
                return null;

            var sym = _symtab.Resolve(nameTok.Text);
            if (sym is null)
            {
                Error($"[L{Pos(nameTok).line}:{Pos(nameTok).col}] Identificador '{nameTok.Text}' no encontrado.");
                return null;
            }

            return sym.Type;
        }

        // ---------------------------------------------------
        //  VISITOR DE SENTENCIAS: ASIGNACIONES + VALIDACIONES
        // ---------------------------------------------------

        public override object? VisitStatement(AlphaParser.StatementContext ctx)
        {
            // 1) ASIGNACIÓN: designator ASSIGN expr SEMI
            if (ctx.ASSIGN() != null)
            {
                // Lado izquierdo: tomamos solo la variable simple
                var leftDesignator = ctx.designator();
                var varName = leftDesignator.IDENT(0).GetText();

                // Verificamos que exista el símbolo
                var sym = _symtab.Resolve(varName);
                if (sym is null)
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Identificador '{varName}' no declarado.");
                    return null;
                }

                // Solo se puede asignar a Var, Param o Field
                if (sym.Kind is not SymbolKind.Local and not SymbolKind.Field and not SymbolKind.Param)
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] No es posible asignar a '{varName}'.");
                    return null;
                }

                // Verificamos que el tipo de destino sea “int”
                var targetType = sym.Type;
                if (targetType.Name != "int")
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Asignación inválida: '{varName}' no es de tipo int.");
                    return null;
                }

                // Lado derecho: puede ser cualquier expresión
                var exprCtx = ctx.expr().FirstOrDefault();
                var rightValObj = Visit(exprCtx);
                if (rightValObj is int rightVal)
                {
                    // Asignación válida: guardamos el valor en memoria
                    _memory[varName] = rightVal;
                }
                else
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Asignación inválida a '{varName}': la expresión no es int.");
                }

                return null;
            }

            // 2) PRINT: lo delegamos a VisitPrintStmt
            if (ctx.printStmt() is { } printCtx)
            {
                return VisitPrintStmt(printCtx);
            }

            // 3) Otras sentencias (IF, FOR, WHILE, etc.): dejamos que el visitante genérico las procese
            return base.VisitStatement(ctx);
        }

        // ---------------------------------------------------
        //  VISITOR DE EXPRESIONES: VALIDACIÓN Y EVALUACIÓN
        // ---------------------------------------------------

        public override object? VisitExpr(AlphaParser.ExprContext ctx)
        {
            // Caso 1: signo unario “-” al inicio y sin operadores binarios
            //    expr: MINUS cast? term (addop term)* 
            // La sobresaturación original omitía tratar el MINUS unario.
            // Si hay MINUS al inicio y no hay addop, lo tratamos como “- term(0)”.
            if (ctx.MINUS() != null && ctx.addop().Length == 0)
            {
                // Por ejemplo: “-5” o “- (alguna subexpresión)”
                var termCtx = ctx.term(0);
                var inner = Visit(termCtx);
                if (inner is int i)
                    return -i;
                Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Operando después de ‘-’ no es int.");
                return 0;
            }

            // Caso 2: no hay ningún addop → devolvemos simplemente el valor del primer término
            if (ctx.addop().Length == 0)
            {
                return Visit(ctx.term(0));
            }

            // Caso 3: hay uno o más operadores de suma/resta
            //   expr → term ( addop term )*
            // Primero calculamos el valor del primer término
            var leftObj = Visit(ctx.term(0));
            if (leftObj is not int)
            {
                Error($"[L{ctx.term(0).Start.Line}:{ctx.term(0).Start.Column}] Operando izquierdo de suma/resta no es int.");
                // Aun así devolvemos 0 para evitar nulls en cascada
                return 0;
            }

            int accumulated = (int)leftObj;
            for (int i = 0; i < ctx.addop().Length; i++)
            {
                var opSymbol = ctx.addop(i).GetText();           // “+” o “-”
                var rightObj = Visit(ctx.term(i + 1));           // siguiente término
                if (rightObj is not int)
                {
                    Error($"[L{ctx.term(i + 1).Start.Line}:{ctx.term(i + 1).Start.Column}] Operando derecho de suma/resta no es int.");
                    continue;
                }

                int rightVal = (int)rightObj;
                accumulated = opSymbol switch
                {
                    "+" => accumulated + rightVal,
                    "-" => accumulated - rightVal,
                    _ => accumulated
                };
            }

            return accumulated;
        }

        public override object? VisitTerm(AlphaParser.TermContext ctx)
        {
            // term → factor ( mulop factor )*
            // En este ejemplo solo haremos factorización sencilla 
            // (no validamos STAR, DIV, MOD). Devolvemos el valor del primer factor.
            return Visit(ctx.factor(0));
        }

        public override object? VisitFactor(AlphaParser.FactorContext ctx)
        {
            // Casos posibles en factor:
            // 1) NUMBER → literal entero
            if (ctx.NUMBER() != null)
            {
                return int.Parse(ctx.NUMBER().GetText());
            }

            // 2) designator sin llamada a método (variable simple)
            //    → devolvemos su valor en memoria
            if (ctx.designator() != null && ctx.LPAREN() is null)
            {
                var name = ctx.designator().IDENT(0).GetText();
                var sym = _symtab.Resolve(name);
                if (sym is null)
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Identificador '{name}' no declarado.");
                    return 0;
                }

                // Solo permitimos lectura si es “int”
                if (sym.Type.Name != "int")
                {
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] La variable '{name}' no es int y no se puede usar en aritmética.");
                    return 0;
                }

                // Devolvemos el valor almacenado (o 0 si no existía)
                return _memory.TryGetValue(name, out var val) ? val : 0;
            }

            // 3) Paréntesis: “( expr )”
            if (ctx.LPAREN() != null && ctx.expr() != null)
            {
                return Visit(ctx.expr());
            }

            // 4) Cualquier otro caso (llamada a método, true/false, char, string, new, etc.)
            //    → no lo admitimos en expresiones aritméticas de tipo int
            if (ctx.LPAREN() != null 
                || ctx.TRUE() != null 
                || ctx.FALSE() != null 
                || ctx.STRING_CONST() != null 
                || ctx.CHAR_CONST() != null 
                || ctx.NEW() != null)
            {
                Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Factor no válido en expresión aritmética: '{ctx.GetText()}'.");
                return 0;
            }

            // Si llegamos aquí, devolvemos 0 (por defecto)
            return 0;
        }

        // ---------------------------------------------------
        //  VISITOR DE PRINT: IMPRIME ENTEROS
        // ---------------------------------------------------

        public override object? VisitPrintStmt(AlphaParser.PrintStmtContext ctx)
        {
            var exprValObj = Visit(ctx.expr());
            if (exprValObj is int intVal)
            {
                if (ctx.NUMBER() != null)
                {
                    int width = int.Parse(ctx.NUMBER().GetText());
                    Console.WriteLine(intVal.ToString().PadLeft(width));
                }
                else
                {
                    Console.WriteLine(intVal);
                }
            }
            else
            {
                Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Print: expresión no es int.");
            }
            return null;
        }

        // ---------------------------------------------------
        //  MÉTODOS AUXILIARES PARA DEPURAR
        // ---------------------------------------------------

        public void DumpSymbols()
        {
            Console.WriteLine("---- Tabla de Símbolos ----");
            int i = 0;
            foreach (var scope in GetAllScopes(_symtab))
            {
                Console.WriteLine($"Scope #{i++} con {scope.Count()} símbolos:");
                foreach (var sym in scope)
                {
                    Console.WriteLine($"  {sym.Kind} {sym.Name} : {sym.Type}");
                }
                Console.WriteLine();
            }
        }

        public void DumpMemory()
        {
            Console.WriteLine("---- Valores en Memoria ----");
            foreach (var pair in _memory)
            {
                Console.WriteLine($"{pair.Key} = {pair.Value}");
            }
        }

        private IEnumerable<IEnumerable<Symbol>> GetAllScopes(SymbolTable symtab)
        {
            foreach (var scope in symtab.AllScopes)
                yield return scope.GetSymbols();
        }
    }
}
