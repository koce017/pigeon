grammar Pigeon;

program : (stmt | functionDecl)+ EOF ;
functionDecl : TYPE ID '(' functionParams? ')' stmtBlock ;
functionParams : TYPE ID (',' TYPE ID)* ;
functionCall : ID '(' functionArgs? ')' ;
functionArgs : expr (',' expr)* ;

varDecl
    : keyword='let' ID '=' expr SEP
    | keyword='const' ID '=' expr SEP
    ;

varAssignLhs
    : ID
    | ID '[' expr ']'
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
    : varDecl                                              # variableDeclarationStatement
    | varAssign                                            # variableAssignmentStatement
    | functionCall SEP                                     # functionCallStatement
    | 'return' expr? SEP                                   # returnStatement
    | 'break' SEP                                          # breakStatement
    | 'continue' SEP                                       # continueStatement
    | 'while' expr stmtBlock                               # whileStatement
    | 'do' stmtBlock 'while' expr                          # doWhileStatement
    | 'if' expr stmtBlock ('else' stmtBlock)?              # ifStatement
    | 'for' ID '=' expr dir=('to'|'downto') expr stmtBlock # forStatement
    ;

stmtBlock
    : ';'
    | stmt
    | '{' stmt* '}'
    ;

expr
    : '[]'                                # emptyListLiteral
    | '{}'                                # emptyDictionaryLiteral
    | '{/}'                               # emptySetLiteral
    | ID '[' expr ']'                     # collectionElementExpression
    | ID                                  # variableExpression
    | BOOL                                # boolLiteral
    | NUMBER                              # numberLiteral
    | STRING                              # stringLiteral
    | '(' expr ')'                        # parenthesizedExpression
    | functionCall                        # functionCallExpression
    | op='-' expr                         # unaryExpression
    | op='+' expr                         # unaryExpression
    | op='!' expr                         # unaryExpression
    |<assoc=right> expr '^' expr          # binaryExpression
    | expr op='%' expr                    # binaryExpression
    | expr op='/' expr                    # binaryExpression
    | expr op='*' expr                    # binaryExpression
    | expr op='+' expr                    # binaryExpression
    | expr op='-' expr                    # binaryExpression
    | expr op='<=' expr                   # binaryExpression
    | expr op='>=' expr                   # binaryExpression
    | expr op='<' expr                    # binaryExpression
    | expr op='>' expr                    # binaryExpression
    | expr op='==' expr                   # binaryExpression
    | expr op='!=' expr                   # binaryExpression
    | expr op='&&' expr                   # binaryExpression
    | expr op='||' expr                   # binaryExpression
    |<assoc=right> expr '?' expr ':' expr # ternaryExpression
    ;

STRING : '"' (ESCAPE|.)*? '"' ;

COMMENT : ('//' .*? '\r'? '\n' | '/*' .*? '*/') -> channel(HIDDEN) ;

BOOL
    : 'true'
    | 'false'
    ;

TYPE
    : 'int'
    | 'float'
    | 'string'
    | 'bool'
    | 'void'
    ;

NUMBER
    : DIGIT+
    | '.' DIGIT+
    | DIGIT+ '.' DIGIT*
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