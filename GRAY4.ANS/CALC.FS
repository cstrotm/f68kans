( Copyright 1990,1994 Martin Anton Ertl )
( This program is distributed WITHOUT ANY WARRANTY. )
( See the file COPYING for the license. )
( a little calculator )
( a usage example: )
( you: ? 2*3-5/4= )
( calc: 5 )
( the grammar is a bit unconventional in its treatment of unary - )
\ e.g., you have to write 3*(-5) instead of 3*-5

DECIMAL
255 max-member ( the whole character set )
2VARIABLE input ( a string in `addr count' representation )

10 stack expected

: sym ( -- c )
 input 2@ IF
  C@
 ELSE
  DROP [CHAR] =
 endif ;

: testsym ( set -- f )
 DUP expected push
 sym member? ;

' testsym test-vector !

: ?syntax-error ( f -- )
 ?not? IF
  empty BEGIN
   expected top union
   expected pop
   expected clear? UNTIL
  ." expected: " ['] EMIT apply-to-members CR TRUE ABORT" syntax error"
 endif ;

: ?readnext ( f -- )
 ?syntax-error
 expected clear
 input 2@
 DUP IF
  1 CHARS - SWAP CHAR+ SWAP
 endif
 input 2! ;

: init ( -- )
 BL WORD COUNT input 2! ;

: t ( -- ) ( use: t c name )
 ( make terminal name with the token c )
 CHAR singleton ['] ?readnext terminal ;

: x ( set1 -- set2 )
 ( read a char from the input and include it in the set )
 CHAR singleton union ;

( make a terminal that accepts all digits )
empty x 0 x 1 x 2 x 3 x 4 x 5 x 6 x 7 x 8 x 9 ' ?readnext terminal digit

t ( "("
t ) ")"
t + "+"
t - "-"
t * "*"
t / "/"
t = "="

nonterminal expr

(( {{ 0 }}
   (( {{ 10 * sym [CHAR] 0 - + }} digit )) ++
)) <- num ( -- n )

(( num
|| "(" expr ")"
)) <- factor ( -- n )

(( factor (( "*" factor {{ * }}
          || "/" factor {{ / }}
          )) **
)) <- term ( -- n )

(( (( term
   || "-" term {{ 0 SWAP - }} ))
   (( "+" term {{ + }}
   || "-" term {{ - }} )) **
)) expr rule ( -- n )

(( {{ init }} expr "=" {{ . }} )) parser ? ( -- )
