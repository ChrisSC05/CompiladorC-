lexer grammar AlphaScanner;

// ======================================================
// PALABRAS CLAVE Y TIPOS PRIMITIVOS
// ======================================================
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
PRINT   : 'print';
fragment TRUE : 'true';
fragment FALSE : 'false';
NULL    : 'null';
CONST : 'const';
VAR     : 'var';

INT     : 'int';
DOUBLE_T: 'double';
BOOL    : 'bool';
CHAR_T  : 'char';
STRING_T: 'string';

// ======================================================
// OPERADORES Y SÍMBOLOS
// ======================================================
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

// ======================================================
// LITERALES (MEJOR TIPADOS)
// ======================================================
INTLITERAL    : DIGIT+;
DOUBLELITERAL : DIGIT+ '.' DIGIT+;
CHARLITERAL   : '\'' (ESC_SEQ | ~['\\\r\n]) '\'';
STRINGLITERAL : '"' (ESC_SEQ | ~["\\\r\n])* '"';
BOOLEANLITERAL: TRUE | FALSE;

// ======================================================
// IDENTIFICADORES
// ======================================================
IDENT
    : '_'? LETTER (LETTER | DIGIT)*
    ;

// ======================================================
// COMENTARIOS Y ESPACIOS
// ======================================================
LINE_COMMENT
    : '//' ~[\r\n]* -> channel(HIDDEN)
    ;

BLOCK_COMMENT
    : '/*' .*? '*/' -> channel(HIDDEN)
    ;
WS             : [ \t\r\n\u000B\u000C\u00A0\u2000-\u200B\u3000]+ -> channel(HIDDEN);

// ======================================================
// FRAGMENTOS
// ======================================================
fragment LETTER : [a-zA-Z_];
fragment DIGIT  : [0-9];
fragment ESC_SEQ: '\\' ( ['"\\tnr] | 'u' HEX HEX HEX HEX );
fragment HEX    : [0-9a-fA-F];
