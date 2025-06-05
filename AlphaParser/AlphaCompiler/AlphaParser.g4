parser grammar AlphaParser;

options { tokenVocab = AlphaScanner; }

// ===================================================================
// 1) INICIO: permitimos múltiples clases y luego EOF
// ===================================================================
program
    : classDecl* EOF
    ;

// ===================================================================
// 2) DECLARACIONES DE CLASES, VARIABLES Y MÉTODOS
// ===================================================================
classDecl
    : CLASS IDENT LBRACE classBody RBRACE
    ;

classBody
    : (varDecl | methodDecl)*
    ;

varDecl
    : type IDENT (COMMA IDENT)* SEMI
    ;

methodDecl
    : (type | VOID) IDENT LPAREN formPars? RPAREN block
    ;

formPars
    : type IDENT (COMMA type IDENT)*
    ;

// ===================================================================
// 3) TIPOS: ahora aceptamos explícitamente INT, CHAR, BOOL, FLOAT, STRING, o cualquier IDENT (clase)
// ===================================================================
type
    : (INT | CHAR | BOOL | FLOAT | STRING | IDENT) (LBRACK RBRACK)?
    ;

// ===================================================================
// 4) SENTENCIAS
// ===================================================================
statement
    : designator (ASSIGN expr
                 | LPAREN actPars? RPAREN
                 | INC
                 | DEC
                 ) SEMI
    | IF LPAREN condition RPAREN statement (ELSE statement)?
    | FOR LPAREN expr? SEMI condition? SEMI expr? RPAREN statement
    | WHILE LPAREN condition RPAREN statement
    | BREAK SEMI
    | RETURN expr? SEMI
    | READ LPAREN designator RPAREN SEMI
    | printStmt
    | block
    | SEMI
    ;

// ===================================================================
// 5) BLOQUES
// ===================================================================
block
    : LBRACE (varDecl | statement)* RBRACE
    ;

// ===================================================================
// 6) LLAMADA A MÉTODOS (ACTUALES)
// ===================================================================
actPars
    : expr (COMMA expr)*
    ;

// ===================================================================
// 7) CONDICIONES (AND / OR / RELACIONALES)
// ===================================================================
condition
    : condTerm (OR condTerm)*
    ;

condTerm
    : condFact (AND condFact)*
    ;

condFact
    : expr relop expr
    ;

// ===================================================================
// 8) EXPRESIONES ARITMÉTICAS Y UNARIAS
// ===================================================================
expr
    : MINUS? cast? term (addop term)* 
    ;

// Cast explícito: “(type) expr”
cast
    : LPAREN type RPAREN
    ;

term
    : factor (mulop factor)*
    ;

factor
    : designator (LPAREN actPars? RPAREN)?   // variable o llamada a método
    | NUMBER
    | CHAR_CONST
    | STRING_CONST
    | TRUE
    | FALSE
    | NULL                    // <--- ahora reconoce NULL
    | NEW IDENT LBRACK expr RBRACK   // creación de arreglos: new Tipo[expr]
    | LPAREN expr RPAREN
    ;

// ===================================================================
// 9) DESIGNADOR (variables, campos, arreglos)
// ===================================================================
designator
    : IDENT (DOT IDENT | LBRACK expr RBRACK)*
    ;

// ===================================================================
// 10) OPERADORES
// ===================================================================
relop
    : EQ 
    | NEQ 
    | GT 
    | GTEQ 
    | LT 
    | LTEQ
    ;

addop
    : PLUS 
    | MINUS
    ;

mulop
    : STAR 
    | DIV 
    | MOD
    ;

// ===================================================================
// 11) PRINT
// ===================================================================
printStmt
    : PRINT LPAREN expr (COMMA NUMBER)? RPAREN SEMI
    ;
