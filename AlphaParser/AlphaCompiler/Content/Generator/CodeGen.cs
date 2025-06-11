using System;
using System.Collections.Generic;
using AlphaCompiler.Semantics;
using Antlr4.Runtime.Misc;

namespace AlphaCompiler.Generation
{
    public class CodeGenerator : AlphaParserBaseVisitor<object?>
    {
        public readonly List<string> Instructions = new();
        private readonly SymbolTable _symbols;

        public CodeGenerator(SymbolTable symbols)
        {
            _symbols = symbols;
        }

        public override object? VisitProgram(AlphaParser.ProgramContext context)
        {
            foreach (var cls in context.classDecl())
                Visit(cls);
            return string.Join("\n", Instructions);
        }

        public override object? VisitClassDecl(AlphaParser.ClassDeclContext context)
        {
            foreach (var member in context.classBody().varDecl())
                Visit(member);
            foreach (var method in context.classBody().methodDecl())
                Visit(method);
            return null;
        }

        public override object? VisitVarDecl(AlphaParser.VarDeclContext context)
        {
            foreach (var id in context.IDENT())
            {
                var name = id.GetText();
                Instructions.Add($"var {name};");
            }
            return null;
        }

        public override object? VisitMethodDecl(AlphaParser.MethodDeclContext context)
        {
            var name = context.IDENT().GetText();
            Instructions.Add($"func {name}() {{");
            Visit(context.block());
            Instructions.Add("}");
            return null;
        }

        public override object? VisitBlock(AlphaParser.BlockContext context)
        {
            foreach (var decl in context.varDecl())
                Visit(decl);
            foreach (var stmt in context.statement())
                Visit(stmt);
            return null;
        }

        public override object? VisitAssignStatement(AlphaParser.AssignStatementContext context)
        {
            var target = context.designator().GetText();
            var expr = Visit(context.expr());
            Instructions.Add($"{target} = {expr};");
            return null;
        }

        public override object? VisitPrintStmt(AlphaParser.PrintStmtContext context)
        {
            var expr = Visit(context.expr());
            Instructions.Add($"print({expr});");
            return null;
        }

        public override object? VisitBinaryExpr(AlphaParser.BinaryExprContext context)
        {
            var left = Visit(context.term(0));
            for (int i = 0; i < context.addop().Length; i++)
            {
                var op = context.addop(i).GetText();
                var right = Visit(context.term(i + 1));
                left = $"({left} {op} {right})";
            }
            return left;
        }

        public override object? VisitUnaryExpr(AlphaParser.UnaryExprContext context)
        {
            var inner = Visit(context.term());
            return $"(-{inner})";
        }

        public override object? VisitTermExpr(AlphaParser.TermExprContext context)
        {
            var left = Visit(context.factor(0));
            for (int i = 0; i < context.mulop().Length; i++)
            {
                var op = context.mulop(i).GetText();
                var right = Visit(context.factor(i + 1));
                left = $"({left} {op} {right})";
            }
            return left;
        }

        public override object? VisitIntFactor(AlphaParser.IntFactorContext context)
            => context.INTLITERAL().GetText();

        public override object? VisitDoubleFactor(AlphaParser.DoubleFactorContext context)
            => context.DOUBLELITERAL().GetText();

        public override object? VisitCharFactor(AlphaParser.CharFactorContext context)
            => context.CHARLITERAL().GetText();

        public override object? VisitBoolFactor(AlphaParser.BoolFactorContext context)
            => context.BOOLEANLITERAL().GetText();

        public override object? VisitStringFactor(AlphaParser.StringFactorContext context)
            => context.STRINGLITERAL().GetText();

        public override object? VisitDesignatorFactor(AlphaParser.DesignatorFactorContext context)
            => context.designator().GetText();

        public override object? VisitNewArrayFactor(AlphaParser.NewArrayFactorContext context)
        {
            var type = context.type().GetText();
            var size = Visit(context.expr());
            return $"new {type}[{size}]";
        }

        public override object? VisitCallFactor(AlphaParser.CallFactorContext context)
        {
            var name = context.designator().GetText();
            var args = new List<string>();
            if (context.actPars() != null)
            {
                foreach (var arg in context.actPars().expr())
                    args.Add(Visit(arg)?.ToString() ?? "");
            }
            return $"{name}({string.Join(", ", args)})";
        }

        public override object? VisitGroupFactor(AlphaParser.GroupFactorContext context)
        {
            var inner = Visit(context.expr());
            return $"({inner})";
        }
    }
}
