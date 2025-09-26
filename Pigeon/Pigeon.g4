grammar Pigeon;

program : functionDecl* main=mainFunction functionDecl* EOF ;
functionDecl : TYPE ID '(' functionParams? ')' stmtBlock ;
functionParams : TYPE ID (',' TYPE ID)* ;
functionCall : ID '(' functionArgs? ')' ;
functionArgs : expr (',' expr)* ;
mainFunction : 'void main()' stmtBlock ;

varDecl
    : keyword='let' ID '=' expr SEP
    | keyword='const' ID '=' expr SEP
    ;

varAssignLhs
    : ID
    | ID '[' index=expr ']'
    ;

varAssign
    : varAssignLhs op='=' expr SEP
    | varAssignLhs op='+=' expr SEP
    | varAssignLhs op='-=' expr SEP
    | varAssignLhs op='*=' expr SEP
    | varAssignLhs op='/=' expr SEP
    | varAssignLhs op='%=' expr SEP
    | varAssignLhs op='^=' expr SEP
    ;

stmt
    : varDecl                                 # variableDeclarationStatement
    | varAssign                               # variableAssignmentStatement
    | functionCall SEP                        # functionCallStatement
    | 'if' expr stmtBlock ('else' stmtBlock)? # ifStatement
    | 'for' ID '=' expr 'to' expr stmtBlock   # forStatement
    | 'while' expr stmtBlock                  # whileStatement
    | 'do' stmtBlock 'while' expr             # doWhileStatement
    | 'break' SEP                             # breakStatement
    | 'continue' SEP                          # continueStatement
    | 'return' expr? SEP                      # returnStatement
    ;

stmtBlock
    : ';'
    | stmt
    | '{' stmt* '}'
    ;

expr
    : ID                                      # variableExpression
    | ID '[' index=expr ']'                   # listElementExpression
    | NUMBER                                  # numberLiteral
    | STRING                                  # stringLiteral
    | BOOL                                    # boolLiteral
    | 'int[]'                                 # emptyIntListLiteral
    | 'float[]'                               # emptyFloatListLiteral
    | 'bool[]'                                # emptyBoolListLiteral
    | 'string[]'                              # emptyStringListLiteral
    | '{}'                                    # emptySetLiteral
    | '(' expr ')'                            # parenthesizedExpression
    | functionCall                            # functionCallExpression
    | op='-' expr                             # unaryExpression
    | op='+' expr                             # unaryExpression
    | op='!' expr                             # unaryExpression
    |<assoc=right> expr '^' expr              # binaryExpression
    | expr op='%' expr                        # binaryExpression
    | expr op='/' expr                        # binaryExpression
    | expr op='*' expr                        # binaryExpression
    | expr op='+' expr                        # binaryExpression
    | expr op='-' expr                        # binaryExpression
    | expr op='<=' expr                       # binaryExpression
    | expr op='>=' expr                       # binaryExpression
    | expr op='<' expr                        # binaryExpression
    | expr op='>' expr                        # binaryExpression
    | expr op='==' expr                       # binaryExpression
    | expr op='!=' expr                       # binaryExpression
    | expr op='&&' expr                       # binaryExpression
    | expr op='||' expr                       # binaryExpression
    |<assoc=right> expr '?' expr ':' expr     # ternaryExpression
    ;

STRING : '"' (ESCAPE|.)*? '"' ;

COMMENT : ('//' .*? '\r'? '\n' | '/*' .*? '*/') -> channel(HIDDEN) ;

TYPE
    : 'void'
    | 'int'
    | 'float'
    | 'string'
    | 'bool'
    | 'list<int>'
    | 'list<float>'
    | 'list<string>'
    | 'list<bool>'
    | 'set'
    ;

NUMBER
    : DIGIT+
    | '.' DIGIT+
    | DIGIT+ '.' DIGIT*
    ;

BOOL
    : 'true'
    | 'false'
    ;

ID : ('_'|LETTER)('_'|'.'|LETTER|DIGIT)* ;
SEP : ';' ;
WHITESPACE : [ \r\t]+ -> skip ;

fragment
DIGIT : [0-9] ;

fragment
LETTER : [a-zA-Z] ;

fragment
ESCAPE
    : '\\"'
    | '\\n'
    | '\\t'
    | '\\\\'
    ;