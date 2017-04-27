( Copyright 1990,1994 Martin Anton Ertl )
( This program is distributed WITHOUT ANY WARRANTY. )
( See the file COPYING for the license. )

( list construct for parsing )
( a b && is the same as < a < b a > * > )

( simple solution )
( : && \ syntax-expr1 syntax-expr2 -- syntax-expr3 )
( over concat ** concat ; )

binary-syntax-expr
 cell context-var test-set
CONSTANT list-syntax-expr

: compute-list ( -- first follow )
 operand1 compute DUP IF
  SWAP operand2 get-first union SWAP
 endif ;

: propagate-list ( follow -- )
 operand2 compute IF
  operand1 get-first union
 endif
 union
 DUP operand1 propagate ( follow1 )
 operand1 compute IF
  union
 ELSE
  SWAP DROP
 endif
 operand2 propagate ;

: generate-list ( -- )
 [COMPILE] BEGIN
 operand1 generate
 test-set @ compile-test
 [COMPILE] WHILE
 operand2 generate
 [COMPILE] REPEAT ;

: pass2-list ( -- )
 operand2 compute IF
  operand1 get-first union
 endif
 DUP test-set !
 follow-set @ check-conflict
 pass2-binary ;

CREATE list-map
', compute-list
', propagate-list
', generate-list
', pass2-list

: && ( syntax-expr1 syntax-expr2 -- syntax-expr3 )
 list-map make-binary 0 , ;

