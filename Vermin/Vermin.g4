grammar Vermin;

// Parser rules

var		: 'var' ID				# OnVarNew
		| 'var' ID '=' expr		# OnVarNewAssign
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

body	: var
		| expr
		| sbody
		;

sbody	: '{' body* '}' ;
prog	: body* ;


// Lexer rules
fragment ESC: '\\' ('"' | 'n' | 'r');
STRING	: '"' (ESC | ~('\n'|'\r'))*? '"';
NUMBER	: (([0-9]+ ('.' [0-9]+)?) | ('.' [0-9]+)) ([eE] [+-]? [0-9]+)? ;
NULL	: 'null' ;



ID		: [_a-zA-Z]+[_a-zA-Z0-9]* ;


WS	: (' '|'\r'|'\n'|'\t') -> channel(HIDDEN)
	;
