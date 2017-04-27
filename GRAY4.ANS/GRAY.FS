\ Copyright 1990, 1991, 1994 Martin Anton Ertl

\     This program is free software; you can redistribute it and/or modify
\     it under the terms of the GNU General Public License as published by
\     the Free Software Foundation; either version 2 of the License, or
\     (at your option) any later version.

\     This program is distributed in the hope that it will be useful,
\     but WITHOUT ANY WARRANTY; without even the implied warranty of
\     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\     GNU General Public License for more details.

\     You should have received a copy of the GNU General Public License
\     along with this program; if not, write to the Free Software
\     Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

\ recursive descent parser generator )

.( Loading Gray ... Copyright 1990-1994 Martin Anton Ertl; NO WARRANTY ) CR

\ misc )
: noop ;

: ?not? ( f -- f )
 0= ;

1 CELLS CONSTANT cell
S" ADDRESS-UNIT-BITS" ENVIRONMENT? ?not? [IF]
  .( environmental attribute address-units-bits unknown, assuming 8 bits/au )
  \ if your machine has more bits/au, this assumption wastes space
  \ if your machine has fewer bits/au, gray will not work
  CR
  8
[THEN]
CELLS CONSTANT bits/cell

: 2, ( w1 w2 -- )
 HERE 2 CELLS ALLOT 2! ;

: endif POSTPONE THEN ; IMMEDIATE

: ?pairs ( n1 n2 -- )
 ( aborts, if the numbers are not equal )
 = ?not? ABORT" mismatched parenthesis" ;
 
: ', \ -- ) ( use: ', name )
 ' , ;

: under+ ( n1 w n2 -- n3 w )
 ROT + SWAP ;

\ stack administration )
\ this implementation is completely unsafe )

: stack \ n -- )
\ use: n stack word )
\ creates a stack called word with n cells )
\ the first cell is the stackpointer )
 CREATE HERE , CELLS ALLOT ;

: push \ n stack -- )
 cell OVER +! @ ! ;

: top \ stack -- n )
 @ @ ;

: pop \ stack -- )
 [ -1 CELLS ] LITERAL SWAP +! ;

: clear? \ stack -- f )
 DUP @ = ;

: clear \ stack -- )
 DUP ! ;


\ sets - represented as bit arrays )
\ bits that represent no elements, must be 0 )
\ all operations assume valid parameters )
\ elements must be unsigned numbers )
\ the max. element size must be declared with max-member )
\ no checking is performed )
\ set operations allot memory )

: decode \ u -- w )
\ returns a cell with bit# u set and everyting else clear )
 1 SWAP LSHIFT ;

VARIABLE cells/set 0 cells/set !
VARIABLE empty-ptr 0 empty-ptr ! \ updatd by max-member )
: empty \ -- set )
 empty-ptr @ ;

: max-member \ u -- )
\ declares u to be the maximum member of sets generated afterwards )
\ must be called before using any set word except member?, add-member )
 bits/cell / 1+
 DUP cells/set !
 HERE empty-ptr ! \ make empty set )
 0 DO 0 , LOOP ;

: copy-set \ set1 -- set2 )
\ makes a copy of set1 )
 HERE SWAP
 cells/set @ 0 DO
  DUP @ ,
  CELL+ LOOP
 DROP ;

: normalize-bit-addr \ addr1 u1 -- addr2 u2 )
\ addr1*bits/cell+u1=addr2*bits/cell+u2, u2<bits/cell )
 bits/cell /MOD
 CELLS under+ ;
\ the /mod could be optimized into a RSHIFT and an AND, if bits/cell is
\ a power of 2, but in an interpreted implementation this would only be
\ faster if the machine has very slow division and in a native code
\ implementation the compiler should be intelligent enough to optimize
\ without help.

: add-member \ u set -- )
\ changes set to include u )
 SWAP normalize-bit-addr
 decode
 OVER @ OR SWAP ! ;

: singleton \ u -- set )
\ makes a set that contains u and nothing else )
 empty copy-set SWAP OVER add-member ;

: member? \ set u -- f )
\ returns true if u is in set )
 normalize-bit-addr
 decode
 SWAP @ AND
 0= ?not? ;

: binary-set-operation \ set1 set2 [w1 w2 -- w3] -- set )
\ creates set from set1 and set2 by applying [w1 w2 -- w3] on members )
\ e.g. ' or binary-set-operation  is the union operation )
 HERE >R
 cells/set @ 0 DO >R
  OVER @ OVER @ R@ EXECUTE ,
  CELL+ SWAP CELL+ SWAP
 R> LOOP
 DROP 2DROP R> ;

: union1 \ set1 set2 -- set )
 ['] OR binary-set-operation ;

: intersection \ set1 set2 -- set )
 ['] AND binary-set-operation ;

: binary-set-test? \ set1 set2 [w1 w2 -- w3] -- f )
\ returns true, if [w1 w2 -- w3] binary-set-operation returns empty )
\ e.g. set1 set2 ' and binary-set-test?  is true, if set1 and set2
\ are disjoint, i.e. they contain no common members )
 >R TRUE ROT ROT R>
 cells/set @ 0 DO >R
  OVER @ OVER @ R@ EXECUTE 0= ?not? IF
   ROT DROP FALSE ROT ROT
  endif
  CELL+ SWAP CELL+ SWAP
 R> LOOP
 DROP 2DROP ;

: notb&and \ w1 w2 -- w3 )
 -1 XOR AND ;

: subset? \ set1 set2 -- f )
\ returns true if every member of set1 is in set2 )
 ['] notb&and binary-set-test? ;

: disjoint? \ set1 set2 -- f )
\ returns true if set1 and set2 heve no common members )
 ['] AND binary-set-test? ;

: apply-to-members \ set [ u -- ] -- )
\ executes [ u -- ] for every member of set )
 cells/set @ bits/cell * 0 DO
  OVER I member? IF
   I OVER EXECUTE
  endif
 LOOP
 2DROP ;

: union \ set1 set2 -- set )
\ just a little more space-efficient ) 
 2DUP subset? IF
  SWAP DROP
 ELSE 2DUP SWAP subset? IF
  DROP
 ELSE
  union1
 endif endif ;


\ tests )
VARIABLE test-vector ' ABORT test-vector !
\ here you should store the execution address of a word ( set -- f )
\ that returns true if the token of the current symbol is in set )

: compile-test \ set -- )
 POSTPONE LITERAL
 test-vector @ COMPILE, ;


\ context management )
500 stack context-stack
\ this stack holds the syntax-exprs currently being treated )
\ enlarge it, if your grammar is large and complex )
context-stack clear

: this \ -- syntax-expr )
\ get current syntax-expr )
 context-stack top ;

: new-context \ syntax-expr -- )
 context-stack push ;

: old-context \ -- )
 context-stack pop ;


\ structures )
: <builds-field \ n1 n2 -- n3 ) ( defining-word )
\ n1 is the offset of the field, n2 its length, n3 the offset of the
\ next field; creates a word that contains the offset )
 CREATE OVER , + ;

0 CONSTANT struct
\ initial offset

: context-var \ use: < offset > size context-var name < offset2 > )
\ name returns the address of the offset field of "this" )
 <builds-field \ n1 n2 -- n3 )
 DOES> \ -- addr )
  @ this + ;

: context-const \ use: < offset > context-const name < offset2 > )
\ name returns the contents of the field of this at offset )
 cell <builds-field \ n1 -- n2 )
 DOES> \ -- n )
  @ this + @ ;


\ syntax-exprs )
struct
 ALIGNED context-const methods
        \ table of words applicable to the syntax-expr (a map)
 1 context-var mark-propagate \ used to ensure that "propagate" is
        \ called at least once for each syntax-expr )
 1 context-var mark-pass2
        \ make sure pass2 is called exactly once )
 ALIGNED cell context-var first-set
        \ all tokens a nonempty path may begin with )
        \ if it's equal to 0, the first-set has not been computed yet )
 1 context-var maybe-empty
        \ true if the syntax-expr can derive eps )
 ALIGNED cell context-var follow-set
	\ the tokens OF the terminals that can follow the syntax-expr )
 \ !! replace the stuff until constant with something working on your system
 ALIGNED 3 CELLS context-var source-location
        \ for error msgs
 80 CHARS context-var error-info
        \ string
ALIGNED CONSTANT syntax-expr   \ length of a syntax-expr )

: make-syntax-expr \ map -- syntax-expr )
\ allocate a syntax-expr and initialize it )
 HERE SWAP , FALSE C, FALSE C,
 ALIGN 0 , FALSE C, ALIGN empty ,
 \ source location. !! replace the stuff until `;' with your stuff
 SOURCE 80 MIN >R  HERE 3 CELLS + R@ CMOVE
 HERE 3 CELLS + ,  R> ,  >IN @ 80 MIN ,  80 CHARS ALLOT ALIGN ;

\ warnings and errors )
: .in \ -- )
\ prints the info stored in source-location in a usable way )
\ prints where the error happened
\ !! again, do it your way
 source-location DUP 2@ SWAP CR TYPE
 CR 2 CELLS + @ 2 - SPACES ." ^" CR  ." ::: " ;

: gray-error ABORT ;

: internal-error
 CR .in ." you found a bug" gray-error ;

VARIABLE print-token ' . print-token !
\ contains execution address of a word < token -- > to print a token )

: check-conflict \ set1 set2 -- )
\ print the intersection of set1 and set2 if it isn't empty )
 2DUP disjoint? ?not? IF
  CR .in ." conflict:"
  intersection print-token @ apply-to-members
 ELSE
  2DROP
 endif ;


\ methods and maps )
: method \ use: < offset > method name < offset2 > )
\ executes the word whose execution address is stored in the field
\ at offset of a table pointed to by the "methods" field of "this" ) 
 cell <builds-field \ n1 -- n2 )
 DOES>
  @ methods + @ EXECUTE ;

\ method table for syntax-exprs
struct
 method compute-method
 method propagate-method
 method generate-method
 method pass2-method
CONSTANT syntax-expr-methods


\ general routines )
: compute \ syntax-expr -- first-set maybe-empty )
\ compute the first-set and maybe-empty of a syntax-expr )
\ a bit of memoization is used here )
 new-context
 first-set @ 0= IF
  compute-method
  maybe-empty C!
  first-set !
 endif
 first-set @ maybe-empty C@
 old-context ;

: get-first \ syntax-expr -- first-set )
 compute DROP ;

: check-cycle \ syntax-expr -- )
\ just check for left recursion )
 compute 2DROP ;

: propagate \ follow-set syntax-expr -- )
\ add follow-set to the follow set of syntax-expr and its children ) 
 new-context
 DUP follow-set @ subset? ?not?  \ would everything stay the same
 mark-propagate C@ ?not? OR IF   \ and was propagate here already
  TRUE mark-propagate C!       \ NO, do propagate
  follow-set @ union DUP follow-set !
  propagate-method
 ELSE
  DROP
 endif
 old-context ;

: generate \ syntax-expr -- )
\ this one gets things done )
 new-context generate-method old-context ;

: pass2 \ syntax-expr -- )
\ computes all necessary first sets, checks for left recursions
\ and conflicts and generates code for rules )
 new-context
 mark-pass2 C@ ?not? IF
  TRUE mark-pass2 C!
  this check-cycle
  pass2-method
 endif
 old-context ;


\ main routine )
: parser \ syntax-expr -- )
\ use: syntax-expr parser xxx )
 context-stack clear
 empty OVER propagate
 DUP pass2
 \ : should not be immediate
 >R : R> generate POSTPONE ; ;


\ eps - empty syntax-expr )
CREATE eps-map
', internal-error
', DROP
', noop
', noop


CREATE eps1
\ the eps syntax-expr proper
 eps-map make-syntax-expr
DROP


: eps \ -- syntax-expr )
\ just adjusts eps1 and returns it
 eps1 new-context
 empty first-set ! ( empty changes due to max-member )
 empty follow-set !
 TRUE maybe-empty C!
 old-context
 eps1 ;


\ terminals )
\ a terminal is a syntax-expr with an extra field )
syntax-expr
 context-const check&next
        \ contains address of a word < f -- > that checks
        \ if f is true and reads the next terminal symbol )
CONSTANT terminal-syntax-expr

: generate-terminal \ -- )
 this get-first compile-test
 check&next COMPILE, ;

CREATE terminal-map
', internal-error
', DROP
', generate-terminal
', noop

: make-terminal \ first-set cfa -- syntax-expr )
 terminal-map make-syntax-expr
 new-context
 ,
 first-set !
 this old-context ;

: terminal \ first-set cfa -- )
 CREATE make-terminal DROP ;


\ binary syntax-exprs )
syntax-expr
 context-const operand1
 context-const operand2
CONSTANT binary-syntax-expr

: make-binary \ syntax-expr1 syntax-expr2 map -- syntax-expr )
 make-syntax-expr ROT , SWAP , ;

: pass2-binary
 operand1 pass2
 operand2 pass2 ;


\ concatenations )
: compute-concatenation \ -- first maybe-empty )
 operand1 compute DUP IF
  DROP
  operand2 compute
  >R union R>
 endif ;

: propagate-concatenation \ follow-set -- )
 operand2 compute IF
  OVER union
 endif \ follow follow1 )
 operand1 propagate
 operand2 propagate ;

: generate-concatenation \ -- )
 operand1 generate
 operand2 generate ;

CREATE concatenation-map
', compute-concatenation
', propagate-concatenation
', generate-concatenation
', pass2-binary

: concat \ syntax-expr1 syntax-expr2 -- syntax-expr )
 concatenation-map make-binary ;
\ this is the actual concatenation operator )
\ but for safety and readability the parenthesised notation )
\ is preferred )


\ alternatives )
: compute-alternative \ -- first maybe-empty )
 operand1 compute
 operand2 compute
 ROT 2DUP AND IF
  CR .in ." warning: two branches may be empty" endif
 OR >R union R> ;

: propagate-alternative \ follow -- )
 DUP operand1 propagate
 operand2 propagate ;

: generate-alternative1 \ -- )
 operand1 get-first compile-test
 POSTPONE IF
 operand1 generate
 POSTPONE ELSE
 operand2 generate
 POSTPONE endif ;

: generate-alternative2 \ -- )
 operand1 get-first compile-test POSTPONE ?not?
 operand2 get-first compile-test POSTPONE AND
 POSTPONE IF
 operand2 generate
 POSTPONE ELSE
 operand1 generate
 POSTPONE endif ;

: generate-alternative \ -- )
 operand1 compute IF
  generate-alternative2
 ELSE
  generate-alternative1
 endif
 DROP ;

: pass2-alternative \ -- )
 this compute IF
  follow-set @ check-conflict
 ELSE
  DROP
 endif
 operand1 get-first operand2 get-first check-conflict
 pass2-binary ;

CREATE alternative-map
', compute-alternative
', propagate-alternative
', generate-alternative
', pass2-alternative

: alt \ syntax-expr1 syntax-expr2 -- syntax-expr )
 alternative-map make-binary ;
\ this is the actual alternative operator )
\ but for safety and readability the parenthesised notation )
\ is preferred )


\ unary syntax-exprs )
syntax-expr
 context-const operand
CONSTANT unary-syntax-expr

: make-unary \ syntax-expr1 map -- syntax-expr2 )
 make-syntax-expr SWAP , ;


\ options and repetitions )
: pass2-option&repetition \ -- )
 follow-set @ operand get-first check-conflict
 operand pass2 ;


\ options )
: compute-option \ -- set f )
 operand compute IF
  CR .in ." warning: unnessesary option" endif
 TRUE ;

: propagate-option \ follow -- )
 operand propagate ;

: generate-option \ -- )
 operand get-first compile-test
 POSTPONE IF
 operand generate
 POSTPONE endif ;

CREATE option-map
', compute-option
', propagate-option
', generate-option
', pass2-option&repetition

: ?? \ syntax-expr1 -- syntax-expr2 )
 option-map make-unary ;


\ repetitions )
: propagate-repetition \ follow-set -- )
 operand get-first union operand propagate ;


\ *-repetitions )
: compute-*repetition \ -- set f )
 operand compute IF
  CR .in ." warning: *repetition of optional term" endif
 TRUE ;

: generate-*repetition \ -- )
 POSTPONE BEGIN
 operand get-first compile-test
 POSTPONE WHILE
 operand generate
 POSTPONE REPEAT ;

CREATE *repetition-map
', compute-*repetition
', propagate-repetition
', generate-*repetition
', pass2-option&repetition

: ** \ syntax-expr1 -- syntax-expr2 )
 *repetition-map make-unary ;


\ +-repetitions )
: compute-+repetition \ -- set f )
 operand compute ;

: generate-+repetition \ -- )
 POSTPONE BEGIN
 operand generate
 operand get-first compile-test
 POSTPONE ?not? POSTPONE UNTIL ;

CREATE +repetition-map
', compute-+repetition
', propagate-repetition
', generate-+repetition
', pass2-option&repetition

: ++ \ syntax-expr1 -- syntax-expr2 )
 +repetition-map make-unary ;


\ actions )
syntax-expr
 context-const action
CONSTANT action-syntax-expr

: generate-action \ syntax-expr -- )
 action COMPILE, ;

CREATE action-map
', internal-error
', DROP
', generate-action
', noop

: {{ \ -- syntax-expr addr colon-sys )
 action-map make-syntax-expr
 new-context
 empty first-set !
 TRUE maybe-empty C!
 this old-context
 \ ?exec !csp )
 HERE cell ALLOT
 :NONAME ;

: }} \ syntax-expr addr colon-sys -- syntax-expr )
 \ ?csp )
 POSTPONE ;
 SWAP !
; IMMEDIATE


\ nonterminals )
syntax-expr
 1 context-var mark-compute
 ALIGNED cell context-var rule-body \ in forth left side of rule )
 cell context-var exec            \ cfa of code for rule )
CONSTANT nt-syntax-expr

: get-body \ -- syntax-expr )
\ get the body of the rule for the nt in "this" )
  rule-body @ IF
   rule-body @
  ELSE
   CR .in ." no rule for nonterminal" gray-error
  endif ;

: compute-nt \ -- set f )
 mark-compute C@ IF
  CR .in ." left recursion" gray-error
 ELSE
  TRUE mark-compute C!
  get-body compute
 endif ;

: propagate-nt \ follow-set -- )
  get-body propagate ;

: code-nt \ -- )
\ generates the code for a rule )
 :NONAME 
 get-body generate
 POSTPONE ;
 exec ! ;

: generate-nt \ -- )
\ generates a call to the code for the rule )
\ since the code needs not be generated yet, an indirect call is used )
 exec POSTPONE LITERAL
 POSTPONE @
 POSTPONE EXECUTE ;

: pass2-nt \ -- )
\ apart from the usual duties, this pass2 also has to code-nt )
 get-body pass2
 code-nt ;

CREATE nt-map
', compute-nt
', propagate-nt
', generate-nt
', pass2-nt

: make-nt \ syntax-expr -- nt )
 nt-map make-syntax-expr
 FALSE C, ALIGN SWAP , 0 , ;

: <- \ use: syntax-expr <- xxx )
     \ xxx: -- syntax-expr )
 CREATE make-nt DROP ;

: nonterminal \ use: nonterminal xxx )
 0 <- ;       \ forward declaration )

: rule \ syntax-expr nt -- )
\ makes a rule )
 new-context
 rule-body @ IF
  .in ." multiple rules for nonterminal" gray-error endif
 rule-body !
 old-context ;


\ syntactic sugar )
: reduce \ 0 x1 ... [x2 x3 -- x4] -- x )
\ e.g. 0 5 6 7 ' + reduce  =  5 6 7 + +  =  18 )
 >R DUP 0= IF
  ." no operand" ABORT
 endif
 BEGIN
  OVER 0= ?not? WHILE
  R@ EXECUTE
 REPEAT \ 0 x )
 SWAP DROP R> DROP ;

7 CONSTANT concatenation-id
: (- \ -- n 0 )
 concatenation-id 0 ;
: -) \ n 0 syntax-expr1 syntax-expr2 .. -- syntax-expr )
 ['] concat reduce
 SWAP concatenation-id ?pairs ;

8 CONSTANT alternative-id
: (| \ -- n 0 )
 alternative-id 0 ;
: |) \ n 0 syntax-expr1 syntax-expr2 .. -- syntax-expr )
 ['] alt reduce
 SWAP alternative-id ?pairs ;

: (( (| (- ;
: )) -) |) ;
: || -) (- ;
