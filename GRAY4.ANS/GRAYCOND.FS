\ Copyright 1990,1994 Martin Anton Ertl
\ This program is distributed WITHOUT ANY WARRANTY.
\ See the file COPYING for the license.

\ semantic conditions
\ This is just a quick hack, I'm not sure about the best syntax and
\ semantics yet. Suggestions welcome

\ The syntax is this:
\ a b cond
\ It takes a flag from the stack. If it is true, "a" is parsed, otherwise "b"
\ I.e., similar to alt, except that the condition is already on the stack.

: compute-condition ( -- first maybe-empty )
 operand1 compute
 operand2 compute
 ROT OR >R union R> ;

: propagate-condition ( follow -- )
 DUP
 operand1 propagate
 operand2 propagate ;

: generate-condition ( -- )
 POSTPONE IF
 operand1 generate
 POSTPONE ELSE
 operand2 generate
 POSTPONE endif ;

CREATE condition-map
', compute-condition
', propagate-condition
', generate-condition
', pass2-binary

: cond ( syntax-exp1 syntax-exp2 -- syntax-exp3 )
 condition-map make-binary ;
