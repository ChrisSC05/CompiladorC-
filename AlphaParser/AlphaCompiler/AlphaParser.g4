parser grammar AlphaParser;

options { tokenVocab = AlphaScanner; }

// --------------- REGLA INICIAL ---------------
program
    : CLASS IDENT LBRACE (varDecl | classDecl | methodDecl)* RBRACE
    ;

// --------------- DECLARACIONES ---------------
varDecl
    : type IDENT (COMMA IDENT)* SEMI
    ;

classDecl
    : CLASS IDENT LBRACE (varDecl | methodDecl)* RBRACE
    ;

methodDecl
    : (type | VOID) IDENT LPAREN formPars? RPAREN block
    ;

formPars
    : type IDENT (COMMA type IDENT)*
    ;

// --------------- TIPOS ------------------------
type
    : IDENT (LBRACK RBRACK)?
    ;

// --------------- SENTENCIAS -------------------
statement
    :designator ( ASSIGN expr
                 | LPAREN actPars? RPAREN
                 | INC
                 | DEC ) SEMI
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

// --------------- BLOQUE -----------------------
block
    : LBRACE (varDecl | statement)* RBRACE
    ;

// --------------- LLAMADAS / PARES -------------
actPars
    : expr (COMMA expr)*
    ;

// --------------- CONDICIONES ------------------
condition
    : condTerm (OR condTerm)*
    ;

condTerm
    : condFact (AND condFact)*
    ;

condFact
    : expr relop expr
    ;

// --------------- EXPRESIONES -----------------
expr
    : MINUS? cast? term (addop term)*
    ;

cast
    : LPAREN type RPAREN
    ;

term
    : factor (mulop factor)*
    ;

factor
    : designator (LPAREN actPars? RPAREN)?
    | NUMBER
    | CHAR_CONST
    | STRING_CONST
    | TRUE
    | FALSE
    | NEW IDENT
    | LPAREN expr RPAREN
    ;

designator
    : IDENT (DOT IDENT | LBRACK expr RBRACK)*
    ;

// --------------- OPERADORES ------------------
relop
    : EQ | NEQ | GT | GTEQ | LT | LTEQ
    ;

addop
    : PLUS | MINUS
    ;

mulop
    : STAR | DIV | MOD
    ;
// --------------- PALABRAS RESERVADAS ----------
printStmt
    : PRINT LPAREN expr (COMMA NUMBER)? RPAREN SEMI
    ;