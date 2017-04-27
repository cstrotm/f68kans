\
\ test.4th ---	tests to verify the proper operation
\		of the portable forth environment
\ (duz 05Aug93)
\

CR
CR .( running "tester.fr" and "core.fr")
CR .( =================================)
CR

INCLUDE test/tester.fr
INCLUDE test/core.fr

INCLUDE test/defs.4th

WAIT

\ display environment
INCLUDE test/environ.4th

WAIT

CR
CR .( More tests:)
CR .( ===========)
CR

INCLUDE test/stack.4th		WAIT
INCLUDE test/compare.4th	WAIT
INCLUDE test/arith.4th		WAIT
INCLUDE test/float.4th		WAIT
INCLUDE test/exception.4th	WAIT


CR
CR .( Finally some benchmarks:)
CR .( ========================)
CR

USING test/benchm.blk  1 LOAD	WAIT

CR
CR .( This should be nicely formatted FORTH-source:)
CR .( =============================================)
CR

SEE DO-PRIME			WAIT

CR
CR .( System survived so far?)
CR .( Fine!)
CR

CR BYE
