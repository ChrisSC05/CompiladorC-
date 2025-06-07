// SymbolTableBuilder.cs actualizado con etiquetas (#) en el parser y manejo de arreglos

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Antlr4.Runtime;
using AlphaCompiler.Semantics;

namespace AlphaCompiler.Semantics
{
    public sealed class SymbolTableBuilder : AlphaParserBaseVisitor<object?>
    {
        private readonly SymbolTable _symtab = new();
        private readonly IList<string> _errors = new List<string>();

        private readonly Dictionary<string, int> _memory = new();
        private readonly Dictionary<string, double> _memoryDouble = new();
        private readonly Dictionary<string, bool> _memoryBool = new();
        private readonly Dictionary<string, char> _memoryChar = new();
        private readonly Dictionary<string, string> _memoryString = new();
        private readonly Dictionary<string, List<object>> _memoryArrays = new();
        private readonly Dictionary<string, ITypeInfo> _arrayElementTypes = new();

        private bool _inClass = false;
        private bool _inMethod = false;
        private bool _breakFlag = false;
        private bool _returnFlag = false;
        private object? _returnValue = null;

        public IReadOnlyList<string> Errors => (IReadOnlyList<string>)_errors;
        public SymbolTable Symbols => _symtab;

        private void Error(string msg) => _errors.Add(msg);
        private static (int line, int col) Pos(IToken tok) => (tok.Line, tok.Column);

        private ITypeInfo GetTypeFromCtx(AlphaParser.TypeContext ctx)
        {
            string name = ctx.IDENT()?.GetText()
                          ?? ctx.INT()?.GetText()
                          ?? ctx.DOUBLE_T()?.GetText()
                          ?? ctx.BOOL()?.GetText()
                          ?? ctx.CHAR_T()?.GetText()
                          ?? ctx.STRING_T()?.GetText()
                          ?? throw new InvalidOperationException($"[L{ctx.Start.Line}:{ctx.Start.Column}] Tipo no reconocido.");
            var baseType = PrimitiveOrClass(name, ctx.Start);
            return ctx.LBRACK() != null ? new ArrayTypeInfo(baseType) : baseType;
        }

        private ITypeInfo PrimitiveOrClass(string name, IToken pos)
        {
            return name switch
            {
                "int" or "double" or "bool" or "char" or "string" => new PrimitiveTypeInfo(name),
                _ => _symtab.Resolve(name) is ClassSymbol ? new ClassTypeInfo(name) : new PrimitiveTypeInfo(name)
            };
        }

        public override object? VisitProgram(AlphaParser.ProgramContext ctx)
        {
            // Funciones predefinidas
            _symtab.Add(new VarSymbol("null", new PrimitiveTypeInfo("null"), true, 0, 0), Error);

            // chr(i): int → char
            _symtab.Add(new MethodSymbol("chr", new PrimitiveTypeInfo("char"),
                new List<ParamSymbol> { new("i", new PrimitiveTypeInfo("int"), 0, 0) }, 0, 0), Error);

            // ord(c): char → int
            _symtab.Add(new MethodSymbol("ord", new PrimitiveTypeInfo("int"),
                new List<ParamSymbol> { new("c", new PrimitiveTypeInfo("char"), 0, 0) }, 0, 0), Error);

            // len(a): arreglo de cualquier tipo → int
            _symtab.Add(new MethodSymbol("len", new PrimitiveTypeInfo("int"),
                new List<ParamSymbol> { new("a", new ArrayTypeInfo(new PrimitiveTypeInfo("int")), 0, 0) }, 0, 0), Error);

            // add(e): agrega elemento a un arreglo
            _symtab.Add(new MethodSymbol("add", new PrimitiveTypeInfo("void"),
                new List<ParamSymbol> { new("e", new PrimitiveTypeInfo("int"), 0, 0) }, 0, 0), Error);

            // del(i): elimina elemento de un arreglo
            _symtab.Add(new MethodSymbol("del", new PrimitiveTypeInfo("void"),
                new List<ParamSymbol> { new("i", new PrimitiveTypeInfo("int"), 0, 0) }, 0, 0), Error);
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
            _symtab.OpenScope();
            
            foreach (var vd in ctx.classBody().varDecl()) Visit(vd);
            foreach (var md in ctx.classBody().methodDecl()) Visit(md);
            
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
                var name = id.GetText();
                var sym = new VarSymbol(name, type, _inClass && !_inMethod, ln, col);
                _symtab.Add(sym, Error);
                if (type is ArrayTypeInfo arrType)
                {
                    _memoryArrays[name] = new List<object>();
                    _arrayElementTypes[name] = arrType.ElementType;
                }
                else
                {
                    switch (type.Name)
                    {
                        case "int": _memory[name] = 0; break;
                        case "double": _memoryDouble[name] = 0d; break;
                        case "bool": _memoryBool[name] = false; break;
                        case "char": _memoryChar[name] = '\0'; break;
                        case "string": _memoryString[name] = ""; break;
                    }
                }
            }
            return null;
        }

        

        public override object? VisitMethodDecl(AlphaParser.MethodDeclContext ctx)
        {
            
            var tok = ctx.IDENT().Symbol;
            var retType = ctx.VOID() is null ? GetTypeFromCtx(ctx.type()) : new PrimitiveTypeInfo("void");
            var meth = new MethodSymbol(ctx.IDENT().GetText(), retType, new List<ParamSymbol>(), tok.Line, tok.Column);
            _symtab.Add(meth, Error);
            _inMethod = true;
            _symtab.OpenScope();

            if (ctx.formPars() is { } fp)
            {
                for (int i = 0; i < fp.IDENT().Length; i++)
                {
                    var pTok = fp.IDENT(i).Symbol;
                    var pSym = new ParamSymbol(fp.IDENT(i).GetText(), GetTypeFromCtx(fp.type(i)), pTok.Line, pTok.Column);
                    meth.Params.Add(pSym);
                    _symtab.Add(pSym, Error);
                    if (pSym.Type.Name == "int") _memory[pSym.Name] = 0;
                }
            }
            Visit(ctx.block());
            _symtab.CloseScope();
            _inMethod = false;
            return null;
        }

        
        
        public override object? VisitAssignStatement(AlphaParser.AssignStatementContext ctx)
        {
            var designator = ctx.designator();
            var varName = designator.IDENT(0).GetText();
            var sym = _symtab.Resolve(varName);
            if (sym == null)
            {
                Error($"Identificador '{varName}' no declarado.");
                return null;
            }

            if (sym.Kind is not SymbolKind.Local and not SymbolKind.Field and not SymbolKind.Param)
            {
                Error($"No se puede asignar a '{varName}'.");
                return null;
            }

            var targetType = sym.Type;

            // ✅ CASO: Asignación a un elemento de un arreglo, como a[0] = 42;
            if (designator.expr().Length > 0)
            {
                if (targetType is not ArrayTypeInfo)
                {
                    Error($"'{varName}' no es un arreglo pero se intenta indexar.");
                    return null;
                }

                var indexExpr = designator.expr(0);
                var indexVal = Visit(indexExpr);
                var value = Visit(ctx.expr());

                if (indexVal is not int index || index < 0)
                {
                    Error($"Índice inválido en '{varName}'");
                    return null;
                }

                if (_memoryArrays.TryGetValue(varName, out var list))
                {
                    if (index >= list.Count)
                    {
                        Error($"Índice fuera de rango: {index} en arreglo '{varName}' (tamaño: {list.Count})");
                        return null;
                    }

                    var elemType = _arrayElementTypes[varName];
                    if (!IsCompatible(elemType, value))
                    {
                        Error($"Tipo incompatible al asignar a '{varName}[{index}]'");
                        return null;
                    }

                    list[index] = value!;
                }
                else
                {
                    Error($"Arreglo '{varName}' no inicializado.");
                }

                return null;
            }

            // ✅ CASO: Asignación completa a un arreglo → a = new int[5];
            if (targetType is ArrayTypeInfo arrInfo)
            {
                var sizeObj = Visit(ctx.expr());
                if (sizeObj is int size && size >= 0)
                {
                    _arrayElementTypes[varName] = arrInfo.ElementType;
                    var def = DefaultValue(arrInfo.ElementType);
                    _memoryArrays[varName] = Enumerable.Repeat(def, size).ToList();
                    return null;
                }

                Error($"Asignación inválida para arreglo '{varName}'");
                return null;
            }

            // ✅ CASO: Asignación a tipo escalar
            var valueToAssign = Visit(ctx.expr());
            switch (targetType.Name)
            {
                case "int" when valueToAssign is int i: _memory[varName] = i; break;
                case "double" when valueToAssign is double d: _memoryDouble[varName] = d; break;
                case "bool" when valueToAssign is bool b: _memoryBool[varName] = b; break;
                case "char" when valueToAssign is char c: _memoryChar[varName] = c; break;
                case "string" when valueToAssign is string s: _memoryString[varName] = s; break;
                default:
                    Error($"Asignación inválida para '{varName}'.");
                    break;
            }

            return null;
        }

        private object? ExecuteCall(string funcName, IList<object?> args)
        {
            switch (funcName)
            {
                case "chr":
                    if (args.Count == 1 && args[0] is int i) return (char)i;
                    Error("chr espera un entero");
                    break;

                case "ord":
                    if (args.Count == 1 && args[0] is char ch) return (int)ch;
                    Error("ord espera un carácter");
                    break;

                case "len":
                    if (args.Count == 1 &&
                        args[0] is string arrName &&
                        _memoryArrays.TryGetValue(arrName, out var lenList))
                        return lenList.Count;

                    Error("len espera el nombre de un arreglo válido");
                    break;

                case "add":
                    if (args.Count == 2 &&
                        args[0] is string name &&
                        _memoryArrays.TryGetValue(name, out var addList))
                    {
                        var elemType = _arrayElementTypes[name];
                        if (IsCompatible(elemType, args[1]))
                        {
                            addList.Add(args[1]!);
                            return null;
                        }

                        Error($"Tipo incompatible para add en arreglo '{name}'");
                        return null;
                    }

                    Error("add espera (nombre, valor)");
                    break;

                case "del":
                    if (args.Count == 2 &&
                        args[0] is string delName &&
                        _memoryArrays.TryGetValue(delName, out var delList))
                    {
                        if (args[1] is int index &&
                            index >= 0 &&
                            index < delList.Count)
                        {
                            delList.RemoveAt(index);
                            return null;
                        }

                        Error($"Índice inválido para del: {args[1]}");
                        return null;
                    }

                    Error("del espera (nombre, índice)");
                    break;
            }

            return null;
        }

        public override object? VisitCallStatement(AlphaParser.CallStatementContext ctx)
        {
            var funcName = ctx.designator().IDENT(0).GetText();
            var args = ctx.actPars()?.expr().Select(Visit).ToList() ?? new List<object?>();
            ExecuteCall(funcName, args);
            return null;
        }

        public override object? VisitBreakStatement(AlphaParser.BreakStatementContext ctx)
        {
            _breakFlag = true;
            return null;
        }

        public override object? VisitReturnStatement(AlphaParser.ReturnStatementContext ctx)
        {
            _returnValue = ctx.expr() != null ? Visit(ctx.expr()) : null;
            _returnFlag = true;
            return null;
        }

        public override object? VisitIfStatement(AlphaParser.IfStatementContext ctx)
        {
            var condVal = Visit(ctx.condition());
            if (condVal is bool b)
            {
                if (b)
                    Visit(ctx.statement(0));
                else if (ctx.statement().Length > 1)
                    Visit(ctx.statement(1));
            }
            else
            {
                Error("Condición no booleana en if");
            }
            return null;
        }

        public override object? VisitWhileStatement(AlphaParser.WhileStatementContext ctx)
        {
            while (true)
            {
                var condVal = Visit(ctx.condition());
                if (condVal is not bool b)
                {
                    Error("Condición no booleana en while");
                    break;
                }
                if (!b) break;

                Visit(ctx.statement());
                if (_returnFlag) break;
                if (_breakFlag)
                {
                    _breakFlag = false;
                    break;
                }
            }
            return null;
        }

        public override object? VisitForStatement(AlphaParser.ForStatementContext ctx)
        {
            var exprs = ctx.expr();
            if (exprs.Length > 0)
                Visit(exprs[0]);

            while (true)
            {
                if (ctx.condition() != null)
                {
                    var cVal = Visit(ctx.condition());
                    if (cVal is not bool cond)
                    {
                        Error("Condición no booleana en for");
                        break;
                    }
                    if (!cond) break;
                }

                Visit(ctx.statement());
                if (_returnFlag) break;
                if (_breakFlag)
                {
                    _breakFlag = false;
                    break;
                }

                if (exprs.Length > 1)
                    Visit(exprs[1]);
            }
            return null;
        }
        
        
        public override object? VisitCallFactor(AlphaParser.CallFactorContext ctx)
        {
            var funcName = ctx.designator().IDENT(0).GetText();
            var args = ctx.actPars()?.expr().Select(Visit).ToList() ?? new List<object?>();
            return ExecuteCall(funcName, args);
        }



        public override object? VisitUnaryExpr(AlphaParser.UnaryExprContext ctx)
        {
            var inner = Visit(ctx.term());
            if (inner is int i) return -i;
            Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Operando unario no es int.");
            return 0;
        }

        public override object? VisitBinaryExpr(AlphaParser.BinaryExprContext ctx)
        {
            //Console.WriteLine($"11 VisitBinaryExpr → expresión: {ctx.GetText()}");

            var left = Visit(ctx.term(0));
            if (ctx.addop().Length == 0)
                return left;

            if (left is not int)
            {
                Error("Operando izquierdo inválido");
                return 0;
            }

            int result = (int)left;

            for (int i = 0; i < ctx.addop().Length; i++)
            {
                var op = ctx.addop(i).GetText();
                var right = Visit(ctx.term(i + 1));
                //console.WriteLine($"44 right value #{i + 1}: {right} (type: {right?.GetType().Name ?? "null"}) con operador '{op}'");

                if (right is not int)
                {
                    Error("Operando derecho inválido");
                    continue;
                }

                int val = (int)right;
                result = op switch
                {
                    "+" => result + val,
                    "-" => result - val,
                    _ => result
                };
            }

            //Console.WriteLine($"   => Resultado final: {result}");
            return result;
        }

        public override object? VisitCondition(AlphaParser.ConditionContext ctx)
        {
            bool result = ToBool(Visit(ctx.condTerm(0)));
            for (int i = 1; i < ctx.condTerm().Length; i++)
                result |= ToBool(Visit(ctx.condTerm(i)));
            return result;
        }

        public override object? VisitCondTerm(AlphaParser.CondTermContext ctx)
        {
            bool result = ToBool(Visit(ctx.condFact(0)));
            for (int i = 1; i < ctx.condFact().Length; i++)
                result &= ToBool(Visit(ctx.condFact(i)));
            return result;
        }

        public override object? VisitCondFact(AlphaParser.CondFactContext ctx)
        {
            var left = Visit(ctx.expr(0));
            var right = Visit(ctx.expr(1));
            var op = ctx.relop().GetText();
            return EvaluateRelational(op, left, right);
        }

        public override object? VisitNewArrayFactor(AlphaParser.NewArrayFactorContext ctx)
        {
            var size = Visit(ctx.expr());
            //Console.WriteLine($"33 VisitNewArrayFactor → tamaño del arreglo: {size}");
            if (size is int s && s >= 0)
                return s;

            Error("Tamaño inválido en 'new T[n]'");
            return null;
        }

        public override object? VisitDoubleFactor(AlphaParser.DoubleFactorContext ctx)
            => double.Parse(ctx.DOUBLELITERAL().GetText(), CultureInfo.InvariantCulture);
        public override object? VisitIntFactor(AlphaParser.IntFactorContext ctx)
            => int.Parse(ctx.INTLITERAL().GetText());

        public override object? VisitBoolFactor(AlphaParser.BoolFactorContext ctx)
            => ctx.BOOLEANLITERAL().GetText() == "true";

        public override object? VisitCharFactor(AlphaParser.CharFactorContext ctx)
        {
            var text = ctx.CHARLITERAL().GetText();
            var unquoted = text.Substring(1, text.Length - 2);
            return unquoted.Length > 0 ? unquoted[0] : '\0';
        }

        public override object? VisitStringFactor(AlphaParser.StringFactorContext ctx)
            => ctx.STRINGLITERAL().GetText().Trim('"');

        public override object? VisitDesignatorFactor(AlphaParser.DesignatorFactorContext ctx)
        {
            var designator = ctx.designator();
            var name = designator.IDENT(0).GetText();
            var sym = _symtab.Resolve(name);

            //  Acceso a elemento del arreglo: a[2]
            if (designator.expr().Length > 0)
            {
                if (sym?.Type is not ArrayTypeInfo)
                {
                    Error($"'{name}' no es un arreglo");
                    return 0;
                }
                var indexVal = Visit(designator.expr(0));
                if (indexVal is not int index || index < 0)
                {
                    Error($"Índice inválido en acceso a '{name}'");
                    return 0;
                }

                if (_memoryArrays.TryGetValue(name, out var list))
                {
                    if (index >= list.Count)
                    {
                        Error($"Índice fuera de rango en '{name}[{index}]'");
                        return 0;
                    }

                    return list[index];
                }

                Error($"Arreglo '{name}' no inicializado.");
                return 0;
            }

            //  Valor escalar o nombre de arreglo
            if (sym == null) return name;

            return sym.Type switch
            {
                ArrayTypeInfo => name, // para llamadas como len(a)
                PrimitiveTypeInfo t => t.Name switch
                {
                    "int" => _memory.TryGetValue(name, out var i) ? i : 0,
                    "double" => _memoryDouble.TryGetValue(name, out var d) ? d : 0d,
                    "bool" => _memoryBool.TryGetValue(name, out var b) ? b : false,
                    "char" => _memoryChar.TryGetValue(name, out var ch) ? ch : '\0',
                    "string" => _memoryString.TryGetValue(name, out var s) ? s : "",
                    _ => null
                },
                _ => null
            };
        }


        public override object? VisitPrintStmt(AlphaParser.PrintStmtContext ctx)
        {
            var exprText = ctx.expr().GetText();
            //Console.WriteLine(exprText);
            if (_memoryArrays.TryGetValue(exprText, out var arr))
            {
                Console.WriteLine("[" + string.Join(", ", arr) + "]");
                return null;
            }

            var value = Visit(ctx.expr());

            switch (value)
            {
                case int i: Console.WriteLine(i); break;
                case double d: Console.WriteLine(d); break;
                case bool b: Console.WriteLine(b); break;
                case char c: Console.WriteLine(c); break;
                case string s: Console.WriteLine(s); break;
                case null:
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Valor nulo en print.");
                    break;
                default:
                    Error($"[L{ctx.Start.Line}:{ctx.Start.Column}] Tipo no imprimible: {value.GetType().Name}");
                    break;
            }

            return null;
        }

        public void DumpSymbols()
        {
            Console.WriteLine("---- Tabla de Símbolos ----");
            int i = 0;
            foreach (var scope in GetAllScopes(_symtab))
            {
                Console.WriteLine($"Scope #{i++}:");
                foreach (var sym in scope)
                    Console.WriteLine($"  {sym.Kind} {sym.Name} : {sym.Type}");
            }

            foreach (var kv in _memoryArrays)
                Console.WriteLine($"  array {kv.Key} : [{string.Join(", ", kv.Value)}]");
        }

        public void DumpMemory()
        {
            Console.WriteLine("---- Memoria ----");
            foreach (var kv in _memory) Console.WriteLine($"{kv.Key} = {kv.Value}");
        }

        private static bool ToBool(object? value)
        {
            if (value is bool b) return b;
            return false;
        }

        private static bool EvaluateRelational(string op, object? left, object? right)
        {
            switch (left, right)
            {
                case (int li, int ri):
                    return op switch
                    {
                        "==" => li == ri,
                        "!=" => li != ri,
                        ">" => li > ri,
                        ">=" => li >= ri,
                        "<" => li < ri,
                        "<=" => li <= ri,
                        _ => false
                    };
                case (double ld, double rd):
                    return op switch
                    {
                        "==" => ld == rd,
                        "!=" => ld != rd,
                        ">" => ld > rd,
                        ">=" => ld >= rd,
                        "<" => ld < rd,
                        "<=" => ld <= rd,
                        _ => false
                    };
                case (char lc, char rc):
                    return op switch
                    {
                        "==" => lc == rc,
                        "!=" => lc != rc,
                        ">" => lc > rc,
                        ">=" => lc >= rc,
                        "<" => lc < rc,
                        "<=" => lc <= rc,
                        _ => false
                    };
                case (string ls, string rs):
                    return op switch
                    {
                        "==" => ls == rs,
                        "!=" => ls != rs,
                        _ => false
                    };
                case (bool lb, bool rb):
                    return op switch
                    {
                        "==" => lb == rb,
                        "!=" => lb != rb,
                        _ => false
                    };
            }
            return false;
        }

        private IEnumerable<IEnumerable<Symbol>> GetAllScopes(SymbolTable symtab)
        {
            foreach (var scope in symtab.AllScopes)
                yield return scope.GetSymbols();
        }

        private static object DefaultValue(ITypeInfo type)
            => type.Name switch
            {
                "int" => 0,
                "double" => 0d,
                "bool" => false,
                "char" => '\0',
                "string" => string.Empty,
                _ => new object()
            };

        private static bool IsCompatible(ITypeInfo type, object? value)
        {
            return type.Name switch
            {
                "int" => value is int,
                "double" => value is double,
                "bool" => value is bool,
                "char" => value is char,
                "string" => value is string,
                _ => true
            };
        }
    }
}
