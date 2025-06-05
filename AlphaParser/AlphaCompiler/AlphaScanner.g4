lexer grammar AlphaScanner;

// ===================================================================
// 1) PALABRAS RESERVADAS Y TIPOS BÁSICOS (deben ir antes de IDENT)
// ===================================================================
CLASS    : 'class';
IF       : 'if';
ELSE     : 'else';
FOR      : 'for';
WHILE    : 'while';
BREAK    : 'break';
RETURN   : 'return';
READ     : 'read';
VOID     : 'void';
NEW      : 'new';
TRUE     : 'true';
FALSE    : 'false';
PRINT    : 'print';

// ------------------------------------------------
// Tipos primitivos exigidos por MiniC#
// ------------------------------------------------
INT      : 'int';
FLOAT    : 'float';
BOOL     : 'bool';
CHAR     : 'char';
STRING   : 'string';

// ------------------------------------------------
// Constante nula
// ------------------------------------------------
NULL     : 'null';

// ===================================================================
// 2) OPERADORES, DELIMITADORES Y SÍMBOLOS
// ===================================================================
INC      : '++';
DEC      : '--';
OR       : '||';
AND      : '&&';

EQ       : '==';
NEQ      : '!=';
GTEQ     : '>='; 
LTEQ     : '<='; 
GT       : '>';  
LT       : '<';  

ASSIGN   : '=';
PLUS     : '+';
MINUS    : '-';
STAR     : '*';
DIV      : '/';
MOD      : '%';

LBRACE   : '{';
RBRACE   : '}';
LPAREN   : '(';
RPAREN   : ')';
LBRACK   : '[';
RBRACK   : ']';
SEMI     : ';';
COMMA    : ','; 
DOT      : '.';

// ===================================================================
// 3) IDENTIFICADORES Y LITERALES
// ===================================================================
IDENT
    : '_'? LETTER (LETTER | DIGIT)*
    ;

// Número entero (sin signo, para simplificar)
NUMBER
    : DIGIT+
    ;

// Char constant: admite secuencias escapadas como '\n', '\t', '\'' ­­
CHAR_CONST
    : '\'' (ESC_SEQ | ~['\\\r\n]) '\''
    ;

// String constant: admite escapado dentro de comillas dobles
STRING_CONST
    : '"' (ESC_SEQ | ~["\\\r\n])* '"'
    ;

// ===================================================================
// 4) COMENTARIOS ANIDADOS Y DE LÍNEA
// ===================================================================
// 4.1) Comentario de línea (// …)
LINE_COMMENT
    : '//' ~[\r\n]* -> channel(HIDDEN)
    ;

// 4.2) Comentarios de bloque ANIDADOS usando modo dedicado
BLOCK_COMMENT_OPEN
    : '/*' -> pushMode(COMMENT_MODE), skip
    ;

mode COMMENT_MODE;
    // Si aparece otro '/*', anidamos: volvemos a entrar en COMMENT_MODE
    COMMENT_NESTED_OPEN  : '/*'           -> pushMode(COMMENT_MODE), skip ;
    // Cuando aparece '*/', salimos de un nivel de anidamiento
    COMMENT_CLOSE        : '*/'           -> popMode, skip ;
    // Cualquier carácter dentro del comentario se descarta
    COMMENT_CONTENT      : .              -> skip ;
    

// ===================================================================
// 5) ESPACIOS EN BLANCO (se descartan)
// ===================================================================
WS 
    : [ \t\r\n]+ -> channel(HIDDEN)
    ;

// ===================================================================
// 6) FRAGMENTOS AUXILIARES
// ===================================================================
fragment LETTER : [a-zA-Z_];
fragment DIGIT  : [0-9];
fragment ESC_SEQ 
    : '\\' ( ['"\\tnr] | 'u' HEX HEX HEX HEX )
    ;
fragment HEX    : [0-9a-fA-F];
