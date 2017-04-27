( Copyright 1990,1994 Martin Anton Ertl )
( This program is distributed WITHOUT ANY WARRANTY. )
( See the file COPYING for the license. )
( dangling else )
( tests if gray finds ambiguity )

10 max-member

: token ( u -- )
 singleton ['] ABORT terminal ;

0 token "if"
1 token "then"
2 token "else"
3 token expr
4 token other

nonterminal stmt
(( other
|| (( "if" expr "then" stmt (( "else" stmt )) ?? ))
)) stmt rule

stmt parser test

