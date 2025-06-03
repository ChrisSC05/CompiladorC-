
namespace AlphaCompiler.Semantics
{
    using System.Collections.Generic;
    using Antlr4.Runtime;
    public sealed class SymbolTableBuilder : AlphaParserBaseVisitor<ITypeInfo?>
    {
        private readonly SymbolTable _symtab = new();
        private readonly IList<string> _errors = new List<string>();

        private bool _inClass = false;
        private bool _inMethod = false;

        public IReadOnlyList<string> Errors => (IReadOnlyList<string>)_errors;
        public SymbolTable Symbols => _symtab;

        private void Error(string msg) => _errors.Add(msg);

        private static (int line, int col) Pos(IToken tok)
            => (tok.Line, tok.Column);

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
                        : new PrimitiveTypeInfo(name) // provisional
            };
        }

        public override ITypeInfo? VisitProgram(AlphaParser.ProgramContext ctx)
        {
            //Console.WriteLine("Visitando program");
            foreach (var child in ctx.children)
            {
                //Console.WriteLine($"Hijo: {child.GetType().Name} Texto: {child.GetText()}");
                Visit(child);  // <---- Aquí forzamos visitar TODO hijo
            }
            return null;
        }

        public override ITypeInfo? VisitClassDecl(AlphaParser.ClassDeclContext ctx)
        {
            
            var (ln, col) = Pos(ctx.IDENT().Symbol);
            var className = ctx.IDENT().GetText();
            //Console.WriteLine($"Procesando clase: {className} (L{ln}:{col})");

            var cls = new ClassSymbol(className, new Dictionary<string, Symbol>(), ln, col);
            _symtab.Add(cls, Error);
            //Console.WriteLine($"Agregando clase: {className} (L{ln}:{col})");

            _inClass = true;
            _symtab.OpenScope();

            foreach (var vd in ctx.varDecl())
                Visit(vd);

            foreach (var md in ctx.methodDecl())
                Visit(md);

            _symtab.CloseScope();
            _inClass = false;

            return null;
        }

        public override ITypeInfo? VisitVarDecl(AlphaParser.VarDeclContext ctx)
        {
            var type = GetTypeFromCtx(ctx.type());
            //Console.WriteLine($"Procesando declaración de variable: {ctx.GetText()} " +
                             // $"de tipo {type} (L{ctx.Start.Line}:{ctx.Start.Column})");
            foreach (var id in ctx.IDENT())
            {
                var (ln, col) = Pos(id.Symbol);
                var sym = new VarSymbol(id.GetText(), type,
                                        IsField: _inClass && !_inMethod, ln, col);
                _symtab.Add(sym, Error);
               //Console.WriteLine($"Agregando variable: {sym.Name} de tipo {sym.Type} " +
                                 // $"(L{ln}:{col}) - Field: {sym.IsField}");
            }
            return null;
        }

        public override ITypeInfo? VisitBlock(AlphaParser.BlockContext ctx)
        {
            
            _symtab.OpenScope();   // Abrimos nuevo scope para el bloque

            foreach (var child in ctx.children)
            {
                Visit(child);
            }

            _symtab.CloseScope();  // Cerramos el scope al terminar
            return null;
        }


        
        public override ITypeInfo? VisitMethodDecl(AlphaParser.MethodDeclContext ctx)
        {
            var tok = ctx.IDENT().Symbol;
            //Console.WriteLine($"Procesando declaración de método: {ctx.IDENT().GetText()} " +
                              //$"(L{tok.Line}:{tok.Column})");
            var ret = ctx.VOID() is null
                        ? GetTypeFromCtx(ctx.type())
                        : new PrimitiveTypeInfo("void");
            //Console.WriteLine($"Tipo de retorno: {ret.Name} (L{tok.Line}:{tok.Column})");

            var meth = new MethodSymbol(ctx.IDENT().GetText(), ret,
                                        new List<ParamSymbol>(),
                                        tok.Line, tok.Column);
            _symtab.Add(meth, Error);
            //Console.WriteLine($"Agregando método: {meth.Name} " +
                             // $"de tipo {meth.ReturnType} (L{tok.Line}:{tok.Column})");

            _inMethod = true;
            _symtab.OpenScope();
            //imprimir ctx
            //Console.WriteLine($"Visitando bloque del método: {ctx.block().GetText()} " +
                             // $"(L{tok.Line}:{tok.Column})");
            if (ctx.formPars() is { } fp)
            {
                for (int i = 0; i < fp.IDENT().Length; i++)
                {
                    var pTok = fp.IDENT(i).Symbol;
                    //Console.WriteLine($"Procesando parámetro: {pTok.Text} " +
                    //                  $"(L{pTok.Line}:{pTok.Column})");
                    var pSym = new ParamSymbol(
                        fp.IDENT(i).GetText(),
                        GetTypeFromCtx(fp.type(i)),
                        pTok.Line, pTok.Column);
                    //Console.WriteLine($"Tipo del parámetro: {pSym.Type} " +
                    //                      $"(L{pTok.Line}:{pTok.Column})");

                    meth.Params.Add(pSym);
                   //Console.WriteLine($"Agregando parámetro: {pSym.Name} " +
                   //                   $"de tipo {pSym.Type} (L{pTok.Line}:{pTok.Column})");
                    _symtab.Add(pSym, Error);
                }
            }
            else 
            {
                //Console.WriteLine("No hay parámetros en este método.");
            }
            Visit(ctx.block());
            _symtab.CloseScope();
            _inMethod = false;

            return null;
        }

        public override ITypeInfo? VisitDesignator(AlphaParser.DesignatorContext ctx)
        {
            var nameTok = ctx.IDENT(0).Symbol;

            // ⚠️ Evitamos resolver "print" como identificador
            if (nameTok.Text == "print")
            {
                return null; // Lo ignoramos porque 'print' es una palabra clave
            }

            var sym = _symtab.Resolve(nameTok.Text);

            if (sym is null)
            {
                Error($"[L{Pos(nameTok).line}:{Pos(nameTok).col}] " +
                      $"Identificador '{nameTok.Text}' no encontrado.");
            }

            return sym?.Type;
        }
        
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

        private IEnumerable<IEnumerable<Symbol>> GetAllScopes(SymbolTable symtab)
        {
            foreach (var scope in symtab.AllScopes)
                yield return scope.GetSymbols();
        }
        
        public override ITypeInfo? VisitPrintStmt(AlphaParser.PrintStmtContext ctx)
        {
            Console.WriteLine("Ejecutando VisitPrintStmt");
            var value = Visit(ctx.expr()); // Evaluamos la expresión que está dentro del print
            if (ctx.NUMBER() != null)
            {
                var width = int.Parse(ctx.NUMBER().GetText());
                Console.WriteLine(value.ToString().PadLeft(width));
            }
            else
            {
                Console.WriteLine(value);
            }

            return null;
        }
    }
}

