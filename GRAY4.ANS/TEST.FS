( Copyright 1990, 1994 Martin Anton Ertl )
( This program is distributed WITHOUT ANY WARRANTY. )
( See gray.doc or gray.f83 for the license. )
S" gray.fs" INCLUDED
CR .( eps )
1 max-member
eps parser x
CR
.( else )
S" else.fs" INCLUDED
CR
.( oberon )
S" oberon.fs" INCLUDED
CR
\ s" graylist.fs" included
\ s" oberon.fs " included
.( calc ) CR
S" calc.fs" INCLUDED
CR
? 2*3-5/4=
CR
.( mini ) CR
S" mini.fs" INCLUDED
S" test.min" mini
CR
5 test CR

\ BYE ( never use BYE in the F68KANS GEM-environment!!! )

