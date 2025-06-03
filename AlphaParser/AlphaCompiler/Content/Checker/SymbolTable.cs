// -----------------------------------------------------------------------------
//  SymbolTable.cs  –  Tabla de símbolos corregida para el compilador Alpha
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Antlr4.Runtime;                // IToken
using Antlr4.Runtime.Tree;
using AlphaCompiler;         // Asegúrate de tener el namespace correcto

namespace AlphaCompiler.Semantics
{
    public enum SymbolKind
    {
        Class,
        Field,
        Local,
        Param,
        Method,
        Type
    }

    public interface ITypeInfo
    {
        string Name   { get; }
        bool   IsArray { get; }
    }

    public sealed record PrimitiveTypeInfo(string Name) : ITypeInfo
    {
        public bool IsArray => false;
        public override string ToString() => Name;
    }

    public sealed record ArrayTypeInfo(ITypeInfo ElementType) : ITypeInfo
    {
        public string Name  => $"{ElementType.Name}[]";
        public bool   IsArray => true;
        public override string ToString() => Name;
    }

    public sealed record ClassTypeInfo(string Name) : ITypeInfo
    {
        public bool IsArray => false;
        public override string ToString() => Name;
    }

    public abstract record Symbol(
        string Name,
        SymbolKind Kind,
        ITypeInfo Type,
        int Line,
        int Column);

    public sealed record VarSymbol(string Name, ITypeInfo Type,
                                   bool IsField, int Line, int Column)
        : Symbol(Name,
                 IsField ? SymbolKind.Field : SymbolKind.Local,
                 Type, Line, Column);

    public sealed record ParamSymbol(string Name, ITypeInfo Type,
                                     int Line, int Column)
        : Symbol(Name, SymbolKind.Param, Type, Line, Column);

    public sealed record MethodSymbol(
        string Name,
        ITypeInfo ReturnType,
        IList<ParamSymbol> Params,
        int Line, int Column)
        : Symbol(Name, SymbolKind.Method, ReturnType, Line, Column);

    public sealed record ClassSymbol(
        string Name,
        IDictionary<string, Symbol> Members,
        int Line, int Column)
        : Symbol(Name, SymbolKind.Class, new ClassTypeInfo(Name), Line, Column);

    public sealed class Scope
    {
        public IEnumerable<Symbol> GetSymbols() => _symbols.Values;
        private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);
        public Scope? Parent { get; }

        public Scope(Scope? parent) => Parent = parent;

        public void Add(Symbol sym, Action<string>? dupError = null)
        {
            if (_symbols.ContainsKey(sym.Name))
            {
                dupError?.Invoke($"[L{sym.Line}:{sym.Column}] " +
                                 $"El identificador '{sym.Name}' ya está definido en este ámbito.");
                return;
            }
            _symbols[sym.Name] = sym;
        }

        public Symbol? Resolve(string name)
        {
            for (Scope? s = this; s is not null; s = s.Parent)
                if (s._symbols.TryGetValue(name, out var sym))
                    return sym;
            return null;
        }
    }

    public sealed class SymbolTable
    {
        private Scope _current = new(null);
        private readonly List<Scope> _allScopes = new();

        public SymbolTable()
        {
            _allScopes.Add(_current); // Agregamos el scope global
        }

        public void OpenScope()
        {
            var newScope = new Scope(_current);
            _current = newScope;
            _allScopes.Add(newScope); // Guardamos el nuevo scope
        }

        public void CloseScope()
        {
            _current = _current.Parent
                       ?? throw new InvalidOperationException("No más scopes.");
        }

        public void Add(Symbol sym, Action<string>? dupError = null)
            => _current.Add(sym, dupError);

        public Symbol? Resolve(string name) => _current.Resolve(name);

        public IEnumerable<Scope> AllScopes => _allScopes;
    }

    
}
