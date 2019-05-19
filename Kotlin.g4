grammar Kotlin;

kotlin :NL* classdecl* NL* main NL* classdecl*;
main: FUNKWD 'main' LBRACKET ARGSKWD COLON MAINPARAM RBRACKET NL* block NL*;
block: LBRACE NL* code NL* RBRACE;//((eq NL* SEMICOLON)|(eq NL)|(NL)|(NL SEMICOLON))* eq? (NL|SEMICOLON)*
code: ((eq (((NL|(SEMICOLON NL))+ SEMICOLON?)|SEMICOLON))* eq ((NL|(SEMICOLON NL))* SEMICOLON?)|SEMICOLON)|(SEMICOLON NL+)*;//(e1| e2|e3 |e4 |dowhile)*;
eq: (decl|init|declinit|typeddeclinit|print|dowhile);// (SEMICOLON|NL);
print: 'print(' (VARNAME|fieldname|operation) ')';

decl: VAR (VARNAME) COLON (TYPE|VARNAME);
init: (VARNAME|fieldname) ASSIGN operation;
declinit: VAR (VARNAME) ASSIGN operation;
typeddeclinit: VAR (VARNAME) COLON (TYPE|VARNAME) ASSIGN operation;

operation: VARNAME|stringoperation|intoperation|booloperation|ternar|fieldname|classcontruct;

stringoperation: stringplus;
stringeq: stringplus ((EQUAL|NOTEQUAL|GT|LT|GTEQ|LTEQ) stringplus);
stringplus: (STRING|VARNAME|fieldname|stringbr) (PLUS (STRING|VARNAME|fieldname|stringbr|intasstringplus|boolasstringplus))*;
stringbr: LBRACKET stringplus RBRACKET;


intoperation: inteq|intadditive;
intasstringplus: intmult (PLUS intmult)*; //(extendedintoperation|stringoperation|extendedbooloperation);


inteq: intadditive ((EQUAL|NOTEQUAL|GT|LT|GTEQ|LTEQ) (intadditive));
intadditive: intmult ((PLUS|MINUS) intmult)*;
intmult: (intbr|INT|VARNAME|fieldname) ((MULT|MOD) (intbr|INT|VARNAME|fieldname))*;
intbr: LBRACKET (intadditive|inteq) RBRACKET;
//intlogbr: LBRACKET inteq RBRACKET;


booloperation: booleq|boolor|inteq|stringeq;
booleq: (boolor ((EQUAL|NOTEQUAL|GT|LT|GTEQ|LTEQ) boolor)+);
boolor: booland (OR booland)*;
booland: boolxor (AND boolxor)*;
boolxor: (BOOL|VARNAME|fieldname|boolneg|boolbr|inteq|stringeq) (XOR (BOOL|VARNAME|fieldname|boolneg|boolbr|booleq|inteq|stringeq))*;
boolneg: NEGATION (BOOL|VARNAME|fieldname|boolbr);
boolbr: LBRACKET (booleq|inteq|stringeq|boolor) RBRACKET;

boolasstringplus: (BOOL|boolbr|boolneg) (PLUS (BOOL|boolbr|boolneg))*; //(extendedintoperation|stringoperation|extendedbooloperation);;
//booland: BOOL '&&' (booloperation);

//unop: ;

//value: INT|STRING|BOOL;

ternar: IF LBRACKET (booloperation) RBRACKET (intoperation|booloperation|stringoperation) ELSE (intoperation|booloperation|stringoperation);
dowhile: DO NL* loopblock NL* WHILE LBRACKET (booloperation) RBRACKET;
loopblock: LBRACE NL* loopcode NL* RBRACE;
loopcode: (((eq|BREAK) (((NL|(SEMICOLON NL))+ SEMICOLON?)|SEMICOLON))* (eq|BREAK) ((NL|(SEMICOLON NL))* SEMICOLON?)|SEMICOLON)|(SEMICOLON NL+)*;

classdecl: CLASS VARNAME NL* classblock NL*;
classblock: LBRACE NL* classcode NL* RBRACE;
classcode: ((classeq (((NL|(SEMICOLON NL))+ SEMICOLON?)|SEMICOLON))* classeq ((NL|(SEMICOLON NL))* SEMICOLON?)|SEMICOLON)|(SEMICOLON NL+)*;
classeq: declinit|typeddeclinit;
classcontruct: VARNAME LBRACKET RBRACKET;

FUNKWD: 'fun';


ARGSKWD: 'args';
LBRACKET: '(';
RBRACKET: ')';
COLON: ':';
SEMICOLON: ';';
MAINPARAM: 'Array<String>';
LBRACE: '{';
RBRACE: '}';
VAR: 'var';
TYPE: 'Boolean'|'Int'|'String';
AND: '&&';
OR: '||';
XOR: 'xor';
PLUS: '+';
MINUS: '-';
MULT: '*';
MOD: '%';
ASSIGN: '=';
EQUAL: '==';
NOTEQUAL: '!=';
NEGATION: '!';
GT: '>';
LT: '<';
GTEQ: '>=';
LTEQ: '<=';
QUEST: '?';
IF: 'if';
ELSE: 'else';
DO: 'do';
WHILE: 'while';
CLASS: 'class';
BREAK: 'break';
NL: '\n';
BOOL: 'true' | 'false';
DOT: '.';

VARNAME: [a-zA-Z] [a-zA-Z0-9_]*;
fieldname: VARNAME (DOT VARNAME)+;
INT :(MINUS|PLUS)? (([1-9][0-9]*)|'0');
STRING : '"' ~[\r\n"]* '"';
//semi: NL+ | SEMICOLON | SEMICOLON NL+;
WS : [ \t\r]+ -> skip;