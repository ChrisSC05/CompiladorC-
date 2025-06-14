//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/chris/OneDrive/Documentos/Proyectos Git/Proyecto 2 Bases 2/CompiladorC-/AlphaParser/AlphaCompiler/AlphaParser.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="AlphaParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public interface IAlphaParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] AlphaParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] AlphaParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.classDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassDecl([NotNull] AlphaParser.ClassDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.classDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassDecl([NotNull] AlphaParser.ClassDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.classBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassBody([NotNull] AlphaParser.ClassBodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.classBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassBody([NotNull] AlphaParser.ClassBodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarDecl([NotNull] AlphaParser.VarDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarDecl([NotNull] AlphaParser.VarDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.constDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstDecl([NotNull] AlphaParser.ConstDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.constDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstDecl([NotNull] AlphaParser.ConstDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.methodDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethodDecl([NotNull] AlphaParser.MethodDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.methodDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethodDecl([NotNull] AlphaParser.MethodDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.formPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormPars([NotNull] AlphaParser.FormParsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.formPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormPars([NotNull] AlphaParser.FormParsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] AlphaParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] AlphaParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>assignStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignStatement([NotNull] AlphaParser.AssignStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>assignStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignStatement([NotNull] AlphaParser.AssignStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>callStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallStatement([NotNull] AlphaParser.CallStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>callStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallStatement([NotNull] AlphaParser.CallStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>incStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIncStatement([NotNull] AlphaParser.IncStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>incStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIncStatement([NotNull] AlphaParser.IncStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>decStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDecStatement([NotNull] AlphaParser.DecStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>decStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDecStatement([NotNull] AlphaParser.DecStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ifStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatement([NotNull] AlphaParser.IfStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ifStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatement([NotNull] AlphaParser.IfStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>forStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatement([NotNull] AlphaParser.ForStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>forStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatement([NotNull] AlphaParser.ForStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>whileStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] AlphaParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>whileStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] AlphaParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>breakStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] AlphaParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>breakStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] AlphaParser.BreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>returnStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] AlphaParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>returnStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] AlphaParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>readStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReadStatement([NotNull] AlphaParser.ReadStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>readStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReadStatement([NotNull] AlphaParser.ReadStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>printStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrintStatement([NotNull] AlphaParser.PrintStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>printStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrintStatement([NotNull] AlphaParser.PrintStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>blockStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlockStatement([NotNull] AlphaParser.BlockStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>blockStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlockStatement([NotNull] AlphaParser.BlockStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>emptyStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmptyStatement([NotNull] AlphaParser.EmptyStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>emptyStatement</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmptyStatement([NotNull] AlphaParser.EmptyStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock([NotNull] AlphaParser.BlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock([NotNull] AlphaParser.BlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.actPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterActPars([NotNull] AlphaParser.ActParsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.actPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitActPars([NotNull] AlphaParser.ActParsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCondition([NotNull] AlphaParser.ConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCondition([NotNull] AlphaParser.ConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.condTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCondTerm([NotNull] AlphaParser.CondTermContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.condTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCondTerm([NotNull] AlphaParser.CondTermContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.condFact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCondFact([NotNull] AlphaParser.CondFactContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.condFact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCondFact([NotNull] AlphaParser.CondFactContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>unaryExpr</c>
	/// labeled alternative in <see cref="AlphaParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryExpr([NotNull] AlphaParser.UnaryExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>unaryExpr</c>
	/// labeled alternative in <see cref="AlphaParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryExpr([NotNull] AlphaParser.UnaryExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binaryExpr</c>
	/// labeled alternative in <see cref="AlphaParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinaryExpr([NotNull] AlphaParser.BinaryExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binaryExpr</c>
	/// labeled alternative in <see cref="AlphaParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinaryExpr([NotNull] AlphaParser.BinaryExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>termExpr</c>
	/// labeled alternative in <see cref="AlphaParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTermExpr([NotNull] AlphaParser.TermExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>termExpr</c>
	/// labeled alternative in <see cref="AlphaParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTermExpr([NotNull] AlphaParser.TermExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>callFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallFactor([NotNull] AlphaParser.CallFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>callFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallFactor([NotNull] AlphaParser.CallFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>designatorFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDesignatorFactor([NotNull] AlphaParser.DesignatorFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>designatorFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDesignatorFactor([NotNull] AlphaParser.DesignatorFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>intFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntFactor([NotNull] AlphaParser.IntFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>intFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntFactor([NotNull] AlphaParser.IntFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>doubleFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoubleFactor([NotNull] AlphaParser.DoubleFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>doubleFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoubleFactor([NotNull] AlphaParser.DoubleFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>boolFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBoolFactor([NotNull] AlphaParser.BoolFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>boolFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBoolFactor([NotNull] AlphaParser.BoolFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>charFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCharFactor([NotNull] AlphaParser.CharFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>charFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCharFactor([NotNull] AlphaParser.CharFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>stringFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringFactor([NotNull] AlphaParser.StringFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>stringFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringFactor([NotNull] AlphaParser.StringFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>nullFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullFactor([NotNull] AlphaParser.NullFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>nullFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullFactor([NotNull] AlphaParser.NullFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>newArrayFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNewArrayFactor([NotNull] AlphaParser.NewArrayFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>newArrayFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNewArrayFactor([NotNull] AlphaParser.NewArrayFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>groupFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGroupFactor([NotNull] AlphaParser.GroupFactorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>groupFactor</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGroupFactor([NotNull] AlphaParser.GroupFactorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.cast"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCast([NotNull] AlphaParser.CastContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.cast"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCast([NotNull] AlphaParser.CastContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.designator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDesignator([NotNull] AlphaParser.DesignatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.designator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDesignator([NotNull] AlphaParser.DesignatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.relop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelop([NotNull] AlphaParser.RelopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.relop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelop([NotNull] AlphaParser.RelopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.addop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAddop([NotNull] AlphaParser.AddopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.addop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAddop([NotNull] AlphaParser.AddopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.mulop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMulop([NotNull] AlphaParser.MulopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.mulop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMulop([NotNull] AlphaParser.MulopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AlphaParser.printStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrintStmt([NotNull] AlphaParser.PrintStmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AlphaParser.printStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrintStmt([NotNull] AlphaParser.PrintStmtContext context);
}
