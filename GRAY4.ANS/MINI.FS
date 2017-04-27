( Copyright 1990,1994 Martin Anton Ertl )
( This program is distributed WITHOUT ANY WARRANTY. )
( See the file COPYING for the license. )

( a compiler for a tiny Pascal-like language )
( to compile a program type `s" file" mini'; the program should be in file )
( This creates a new word [its name is the name given after the program
( keyword], that you must call to execute the program )
( mini programs take their input from the stack and write their )
( output with . )

S" graylist.fs" INCLUDED

.( Loading mini ... ) CR

( scanner )
( it is implemented using gray to give an example )
( that's probably not the best way )
S" max-char" ENVIRONMENT? ?not? [IF]
255
[THEN]
1+ CONSTANT eof-char
eof-char max-member ( the whole character set )

VARIABLE mini-file

VARIABLE tokenval 0 tokenval !
: token ( -- ) ( use: token name ) ( name: -- n )
 ( defines a token that returns a unique value )
 tokenval @ CONSTANT
 1 tokenval +! ;

token ";"
token ","
token ":="
token "="
token "#"
token ">"
token "+"
token "-"
token "*"
token "("
token ")"
token Ident
token Number-token
token eof-token

WORDLIST CONSTANT keyword-wordlist
GET-CURRENT keyword-wordlist SET-CURRENT
token PROGRAM
token VAR
token BEGIN
token END
token Read
token Write
token IF
token THEN
token WHILE
token DO
SET-CURRENT

VARIABLE numval
VARIABLE identp
VARIABLE identlen

: ident-string ( -- addr u )
 identp @ identlen @ ;

: adds ( addr1 c -- addr1+1 )
 ( accumulates char to string )
 OVER C! 1+ ;

: key/ident ( addr u -- n )
 ( checks string at addr for keyword and returns token value )
 keyword-wordlist SEARCH-WORDLIST IF
  EXECUTE
 ELSE
  Ident
 endif ;

VARIABLE ch

: testchar? ( set -- f )
 ch @ member? ;
' testchar? test-vector !

CREATE c1 1 CHARS ALLOT

: ?nextchar ( f -- )
 \ this will not earn a speed record
 ?not? ABORT" non-mini character or '=' missing after ':'"
 c1 1 mini-file @ READ-FILE ABORT" read-file error"
 IF \ read <>0 chars
  c1 C@ ch !
 ELSE
  eof-char ch !
 endif ;

: .. ( c1 c2 -- set )
 ( creates a set that includes the characters c, c1<=c<=c2 )
 empty copy-set
 SWAP 1+ ROT DO
  I OVER add-member
 LOOP ;

: ` ( -- terminal ) ( use: ` c )
 ( creates anonymous terminal for the character c )
 CHAR singleton ['] ?nextchar make-terminal ;

CHAR a CHAR z ..  CHAR A CHAR Z ..  union  ' ?nextchar  terminal letter
CHAR 0 CHAR 9 ..  ' ?nextchar  terminal digit
0 32 ..  ' ?nextchar  terminal BLANK
eof-char singleton ' ?nextchar terminal eof-scanner

(( BLANK **
   (( ` ;      {{ ";"  }}
   || ` ,      {{ ","  }}
   || ` : ` =  {{ ":=" }}
   || ` =      {{ "="  }}
   || ` #      {{ "#"  }}
   || ` >      {{ ">"  }}
   || ` +      {{ "+"  }}
   || ` -      {{ "-"  }}
   || ` *      {{ "*"  }}
   || ` (      {{ "("  }}
   || ` )      {{ ")"  }}
   || eof-scanner {{ eof-token }}
   || {{ 0 }}
      (( {{ 10 * ch @ + [CHAR] 0 - }} CR digit )) ++
      {{ numval !  Number-token }}
   || {{ HERE identp ! HERE ch @ adds }} letter
      (( {{ ch @ adds }} (( letter || digit )) )) **
      {{ HERE - DUP identlen !  HERE SWAP key/ident }}
   ))
)) <- symbol

symbol parser scan


( parser )
tokenval @ 1- max-member

WORDLIST CONSTANT variable-wordlist ( for mini variables )

VARIABLE sym

: testsym? ( set -- f )
 sym @ member? ;
' testsym? test-vector !

: ?nextsym ( f -- )
 ?not? ABORT" syntax error"
 scan sym ! ;

: t ( n -- ) ( use: token t name )
 singleton ['] ?nextsym terminal ;

GET-ORDER keyword-wordlist SWAP 1+ SET-ORDER
";" t ";"
"," t ","
":=" t ":="
"=" t "="
"#" t "#"
">" t ">"
"+" t "+"
"-" t "-"
"*" t "*"
"(" t "("
")" t ")"
Ident t Ident
Number-token t number-parser
PROGRAM t "PROGRAM"
VAR t "VAR"
BEGIN t "BEGIN"
END t "END"
Read t "Read"
Write t "Write"
IF t "IF"
THEN t "THEN"
WHILE t "WHILE"
DO t "DO"
eof-token t eof-parser
PREVIOUS

: append-string ( addr1 u1 addr2 u2 -- addr1 u3 )
 3 ROLL >R ( u1 a2 u2 )
 ROT 2DUP + ( a2 u2 u1 u1+u2 )
 R> SWAP >R ( a2 u2 u1 a1 )
 DUP >R
 + SWAP MOVE
 R> R> ;

: :name ( addr u -- colon-sys )
 ( defines colon-def whose name is give by addr u )
 PAD 0 S" : " append-string 2SWAP append-string EVALUATE ;

: variable-name ( addr u -- )
 ( defines variable whose name is given by addr u )
 2DUP variable-wordlist SEARCH-WORDLIST ABORT" variable already defined"
 GET-CURRENT >R variable-wordlist SET-CURRENT
 PAD 0 S" VARIABLE " append-string 2SWAP append-string EVALUATE
 R> SET-CURRENT ;

: getvar ( addr u -- xt )
 ( get the execution token of the var whose name is given by addr u )
 variable-wordlist SEARCH-WORDLIST ?not? ABORT" variable undefined" ;

: <> ( n1 n2 -- f )
 = ?not? ;

nonterminal Stat
nonterminal Expr

(( {{ numval @ }} number-parser )) <- Number

\ (( {{ identp @ }} ident )) <- Ident

(( Number {{ POSTPONE LITERAL }}
|| {{ ident-string getvar COMPILE, POSTPONE @ }} Ident
|| "(" Expr ")"
)) <- Factor

(( Factor (( "*" Factor {{ POSTPONE * }} )) ** )) <- Term

(( Term  (( (( "+" {{ ['] + }} || "-" {{ ['] - }} )) Term {{ COMPILE, }} )) **
)) Expr rule

(( Expr
   (( "=" {{ ['] = }} || "#" {{ ['] <> }} || ">" {{ ['] > }} ))
   Expr {{ COMPILE, }}
)) <- Cond

Stat ";" && ?? <- StatSeq

(( "Read" {{ ident-string getvar COMPILE, POSTPONE ! }} Ident )) <- ReadStat

(( "Write" Expr {{ POSTPONE . }} )) <- WriteStat

(( {{ ident-string getvar }} Ident ":=" Expr {{ COMPILE, POSTPONE ! }}
)) <- AssStat

(( "IF" Cond {{ POSTPONE IF }} "THEN" StatSeq "END" {{ POSTPONE endif }}
)) <- IfStat

(( {{ POSTPONE BEGIN }} "WHILE" Cond {{ POSTPONE WHILE }} "DO"
   StatSeq "END" {{ POSTPONE REPEAT }}
)) <- WhileStat
   
(( ReadStat || WriteStat || AssStat || IfStat || WhileStat )) Stat rule

(( "VAR" {{ GET-CURRENT variable-wordlist SET-CURRENT }}
   (( {{ ident-string variable-name }} Ident )) "," &&
   {{ SET-CURRENT }}
)) <- Decl

(( "PROGRAM" {{ identlen @ ALIGNED ALLOT ident-string }} Ident  Decl ??
   {{ :name }} "BEGIN" StatSeq {{ POSTPONE ; }} "END" eof-parser 
)) <- Program

Program parser parsemini

: mini ( addr u -- )
 \ process the mini file whose name is given by addr u
 R/O OPEN-FILE ABORT" file opening error" mini-file !
 TRUE ?nextchar TRUE ?nextsym parsemini
 mini-file @ CLOSE-FILE ABORT" file closing error" ;

