// parser grammar AlphaParser

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
    : (varDecl | constDecl | methodDecl)*
    ;

varDecl
    : type IDENT (COMMA IDENT)* SEMI
    ;

constDecl
    : CONST type IDENT ASSIGN expr SEMI
    ;

methodDecl
    : (type | VOID) IDENT LPAREN formPars? RPAREN block
    ;

formPars
    : type IDENT (COMMA type IDENT)*
    ;

// ===================================================================
// 3) TIPOS
// ===================================================================
type
    : (INT | CHAR_T | BOOL | DOUBLE_T | STRING_T | IDENT) (LBRACK RBRACK)?
    ;

// ===================================================================
// 4) SENTENCIAS
// ===================================================================
statement
    : designator ASSIGN expr SEMI                              #assignStatement
    | designator LPAREN actPars? RPAREN SEMI                   #callStatement
    | designator INC SEMI                                      #incStatement
    | designator DEC SEMI                                      #decStatement
    | IF LPAREN condition RPAREN block (ELSE block)?           #ifStatement
    | FOR LPAREN expr? SEMI condition? SEMI expr? RPAREN statement #forStatement
    | WHILE LPAREN condition RPAREN statement                  #whileStatement
    | BREAK SEMI                                               #breakStatement
    | RETURN expr? SEMI                                        #returnStatement
    | READ LPAREN designator RPAREN SEMI                       #readStatement
    | printStmt                                                #printStatement
    | block                                                    #blockStatement
    | SEMI                                                     #emptyStatement
    ;
// ===================================================================
// 5) BLOQUES
// ===================================================================
block
    : LBRACE (varDecl | constDecl | statement)* RBRACE
    ;

// ===================================================================
// 6) LLAMADA A MÉTODOS (ACTUALES)
// ===================================================================
actPars
    : expr (COMMA expr)*
    ;

// ===================================================================
// 7) CONDICIONES
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
// 8) EXPRESIONES
// ===================================================================
expr
    : MINUS cast? term                     #unaryExpr
    | term (addop term)*                   #binaryExpr
    ;

term
    : factor (mulop factor)*              #termExpr
    ;

factor
    : designator LPAREN actPars? RPAREN   #callFactor
    | designator                          #designatorFactor
    | INTLITERAL                          #intFactor
    | DOUBLELITERAL                       #doubleFactor
    | BOOLEANLITERAL                      #boolFactor
    | CHARLITERAL                         #charFactor
    | STRINGLITERAL                       #stringFactor
    | NULL                                #nullFactor
    | NEW type LBRACK expr RBRACK         #newArrayFactor
    | LPAREN expr RPAREN                  #groupFactor
    ;
cast
    : LPAREN type RPAREN
    ;
// ===================================================================
// 9) DESIGNADOR
// ===================================================================
designator
    : IDENT (DOT IDENT | LBRACK expr RBRACK)*
    ;

// ===================================================================
// 10) OPERADORES
// ===================================================================
relop
    : EQ | NEQ | GT | GTEQ | LT | LTEQ
    ;

addop
    : PLUS | MINUS
    ;

mulop
    : STAR | DIV | MOD
    ;

// ===================================================================
// 11) PRINT
// ===================================================================
printStmt
    : PRINT LPAREN expr (COMMA INTLITERAL)? RPAREN SEMI
    ;
