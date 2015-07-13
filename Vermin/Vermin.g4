grammar Vermin;

// Parser rules

var		: VAR ID								# OnVarNew
		| VAR ID '=' expr						# OnVarNewAssign
		;

funcdef	: EXTERN? FUNC ID '(' args? ')' sbody?	# OnFuncDef
		;

args	: ID (','  ID)*							# OnArgsDef
		;

return	: RETURN expr							# OnReturnExpr
		| RETURN								# OnReturn
		;

const	: STRING		# OnString
		| NUMBER		# OnNumber
		| NULL			# OnNull
		;

index	: '[' expr ']'
		;

expr	: const											# OnConst
		| ID											# OnID
		| expr index									# OnIndex
		| expr op=('*'|'/') expr						# OnMultDiv
		| expr op=('+'|'-') expr						# OnAddSub
		| ID '=' expr									# OnAssign
		| expr index '=' expr							# OnIndexAssign
		| '(' expr ')'									# OnParen
		;

body	: (var|funcdef|expr|sbody|return)* ;

sbody	: '{' body '}'		# OnScope
		;
prog	: body ;


// Lexer rules
fragment ESC: '\\' ('"' | 'n' | 'r');
STRING	: '"' (ESC | ~('\n'|'\r'))*? '"';
NUMBER	: (([0-9]+ ('.' [0-9]+)?) | ('.' [0-9]+)) ([eE] [+-]? [0-9]+)? ;

NULL	: 'null' ;
RETURN	: 'return' ;
EXTERN	: 'extern' ;
VAR		: 'var' ;
FUNC	: 'func' ;


ID		: [_a-zA-Z]+[_a-zA-Z0-9]* ;


WS	: (' '|'\r'|'\n'|'\t'|'//' .*? '\n'|'/*' .*? '*/') -> channel(HIDDEN)
	;
