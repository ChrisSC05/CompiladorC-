lexer grammar AlphaScanner;

// ---------- PALABRAS CLAVE ----------
CLASS   : 'class';
IF      : 'if';
ELSE    : 'else';
FOR     : 'for';
WHILE   : 'while';
BREAK   : 'break';
RETURN  : 'return';
READ    : 'read';
VOID    : 'void';
NEW     : 'new';
TRUE    : 'true';
FALSE   : 'false';

// ---------- OPERADORES Y SÍMBOLOS ----------
INC     : '++';
DEC     : '--';
OR      : '||';
AND     : '&&';

EQ      : '==';
NEQ     : '!=';
GTEQ    : '>=';
LTEQ    : '<=';
GT      : '>';
LT      : '<';

ASSIGN  : '=';
PLUS    : '+';
MINUS   : '-';
STAR    : '*';
DIV     : '/';
MOD     : '%';

LBRACE  : '{';
RBRACE  : '}';
LPAREN  : '(';
RPAREN  : ')';
LBRACK  : '[';
RBRACK  : ']';
SEMI    : ';';
COMMA   : ',';
DOT     : '.';

// ---------- IDENTIFICADORES Y LITERALES ----------
IDENT
    : LETTER (LETTER | DIGIT)*
    ;

NUMBER
    : '0'
    | NON_ZERO_DIGIT DIGIT* ('.' DIGIT+)?          // int o float
    ;

CHAR_CONST
    : '\'' (ESC_SEQ | ~['\\\r\n]) '\''  // char literal
    ;

STRING_CONST
    : '"' (ESC_SEQ | ~["\\r\n])* '"'
    ;

// ---------- COMENTARIOS ----------
COMMENT
    : '/' -> pushMode(COMMENT_MODE)
    ;

LINE_COMMENT
    : '//' ~[\r\n] -> channel(HIDDEN)
    ;

// ---------- ESPACIOS ----------
WS
    : [ \t\r\n\u000C]+ -> channel(HIDDEN)
    ;

// ---------- FRAGMENTOS ----------
fragment LETTER         : [a-zA-Z_] ;
fragment DIGIT          : [0-9] ;
fragment NON_ZERO_DIGIT : [1-9] ;
fragment ESC_SEQ : '\\' (['"\\tnr] | 'u' HEX HEX HEX HEX) ;
fragment HEX            : [0-9a-fA-F] ;

// ---------- MODO PARA COMENTARIOS ANIDADOS ----------
mode COMMENT_MODE;

  COMMENT_START
      : '/' -> pushMode(COMMENT_MODE)
      ;

  COMMENT_END
      : '/' -> popMode, channel(HIDDEN)
      ;

  COMMENT_CHAR
      : . -> channel(HIDDEN)
      ;
// ---------- FUNCIONES -------------
PRINT : 'print'; 
  