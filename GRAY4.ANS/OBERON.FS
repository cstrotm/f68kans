\ Copyright 1990,1994 Martin Anton Ertl
\ This program is distributed WITHOUT ANY WARRANTY.
\ See the file COPYING for the license.

\ parser for oberon )
\ i chose oberon, because it has a moderately complex grammar, )
\ not for its qualities as a language )
\ this is just a parser, without any semantic actions )
\ it was not tested )
\ the grammar was taken from: )
\ N.Wirth, The Programming Language Oberon, )
\ Software - Practice and Experience, 18, 671-690 (July 1988)
\ corrections appeared in the january 89 issue, i believe )

\ space requirements on a 16-bit fig-forth using graylist.fs )
\ grammar:         8104 bytes )
\ generated code:  3551 bytes )
\ generated total: 5719 bytes )
\ context-stack:    220 bytes )
\ return-stack:     720 bytes   WARNING: you must enlarge TILE's return-stack )
\ the data-stack is not critical- mine can only hold 60 cells )
\ if your return-stack cannot hold much, change the does> part )
\ of method: pop 3 cells off the return stack and save them )
\ elsewhere until after the execute )
\ generating the parser takes a while: 24.5 seconds on my 4Mhz 6502 system )

\ the grammar contains four conflicts, which are all harmful, )
\ i.e. the generated parser will not parse all oberon programs )
\ in the qualident rule there is a conflict between the two idents )
\ designator doesn't know, whether a "(" means a type guard or a procedure call)
\ Procedure- and ForwardDeclaration have a conflict in a DeclarationSequence )
\ in statement there's a classical conflict between assigment and ProcedureCall)

63 max-member

VARIABLE tcount 0 tcount !
: t \ -- )
 tcount @ singleton ['] ABORT terminal
 1 tcount +! ;


t integer
t real
t CharConstant
t string
t ident
t "+"
t "-"
t "*"
t "/"
t "~"
t "&"
t "."
t ","
t ";"
t "|"
t "("
t ")"
t "["
t "]"
t ":="
t "^"
t "="
t "#"
t "<"
t ">"
t "<="
t ">="
t ":"
t ".."
t "{"
t "}"

t "ARRAY"        t "IN"        t "THEN"
t "BEGIN"        t "IS"        t "TO"
t "CASE"         t "LOOP"      t "TYPE"
t "CONST"        t "MOD"       t "UNTIL"
t "DEFINITION"   t "MODULE"    t "VAR"
t "DIV"          t "NIL"       t "WHILE"
t "DO"           t "OF"        t "WITH"
t "ELSE"         t "OR"
t "ELSIF"        t "POINTER"
t "END"          t "PROCEDURE"
t "EXIT"         t "RECORD"
t "IF"           t "REPEAT"
t "IMPORT"       t "RETURN"


: && \ syntax-expr1 syntax-expr2 -- syntax-expr3 )
 OVER concat ** concat ; 

nonterminal factor
nonterminal expression
nonterminal TYPE
nonterminal statement
nonterminal DeclarationSequence


(( integer || real )) <- number

(( (( ident "." )) ?? ident )) <- qualident

expression <- ConstExpression
(( ident "=" ConstExpression )) <- ConstantDeclaration

ConstExpression <- length
(( "ARRAY" length "," && "OF" TYPE )) <- ArrayType

ident "," && <- IdentList
(( IdentList ":" TYPE )) ?? <- FieldList
FieldList ";" && <- FieldListSequence
qualident <- BaseType
(( "RECORD" (( "(" BaseType ")" )) ?? FieldListSequence "END" )) <- RecordType

(( "POINTER" "TO" TYPE )) <- PointerType

(( (( "ARRAY" "OF" )) ** qualident )) <- FormalType
(( "(" (( "VAR" ?? FormalType )) "," && ?? ")" (( ":" qualident )) ?? ))
 <- FormalTypeList
(( "PROCEDURE" FormalTypeList ?? )) <- ProcedureType

(( qualident || ArrayType || RecordType || PointerType || ProcedureType ))
 TYPE rule
(( ident "=" TYPE )) <- TypeDeclaration

(( IdentList ":" TYPE )) <- VariableDeclaration

expression "," && <- ExpList
(( qualident (( "." ident || "[" ExpList "]" || "(" qualident ")" || "^" )) **
)) <- designator


(( "(" ExpList ?? ")" )) <- ActualParameters
(( expression (( ".." expression )) ?? )) <- element
(( "{" element "," && ?? "}" )) <- set
(( number || CharConstant || string || "NIL" || set ||
   designator ActualParameters ?? || "(" expression ")" || "~" factor ))
 factor rule
(( "*" || "/" || "DIV" || "MOD" || "&" )) <- MulOperator
factor MulOperator && <- term
(( "+" || "-" || "OR" )) <- AddOperator
(( (( "+" || "-" )) ?? term AddOperator && )) <- SimpleExpression
(( "=" || "#" || "<" || "<=" || ">" || ">=" || "IN" || "IS" )) <- relation
(( SimpleExpression (( relation SimpleExpression )) ?? )) expression rule

(( designator ":=" expression )) <- assignment

(( designator ActualParameters ?? )) <- ProcedureCall

statement ";" && <- StatementSequence

(( "IF" expression "THEN" StatementSequence
   (( "ELSIF" expression "THEN" StatementSequence )) **
   (( "ELSE" StatementSequence )) ??
   "END"
)) <- IfStatement

(( ConstExpression (( ".." ConstExpression )) ?? )) <- CaseLabels
CaseLabels "," && <- CaseLabelList
(( CaseLabelList ":" StatementSequence )) ?? <- CASE
(( "CASE" expression "OF" CASE "|" && (( "ELSE" StatementSequence )) ?? "END" ))
<- CaseStatement

(( "WHILE" expression "DO" StatementSequence "END" )) <- WhileStatement

(( "REPEAT" StatementSequence "UNTIL" expression )) <- RepeatStatement

(( "LOOP" StatementSequence "END" )) <- LoopStatement

(( "WITH" qualident ":" qualident "DO" StatementSequence "END" )) <- WithStatement

(( assignment || ProcedureCall ||
   IfStatement || CaseStatement || WhileStatement || RepeatStatement ||
   LoopStatement || WithStatement || "EXIT" || "RETURN" expression ??
)) ?? statement rule

(( "VAR" ?? IdentList ":" FormalType )) <- FPSection
(( "(" FPSection ";" && ?? ")" (( ":" qualident )) ?? )) <- FormalParameters

(( DeclarationSequence (( "BEGIN" StatementSequence )) ?? "END" )) <- ProcedureBody
(( "PROCEDURE" "*" ?? ident FormalParameters ?? )) <- ProcedureHeading
(( ProcedureHeading ";" ProcedureBody ident )) <- ProcedureDeclaration
(( "PROCEDURE" "^" ident FormalParameters ?? )) <- ForwardDeclaration
(( (( "CONST" (( ConstantDeclaration ";" )) ** )) ??
   (( "TYPE" (( TypeDeclaration ";" )) ** )) ??
   (( "VAR" (( VariableDeclaration ";" )) ** )) ??
   (( ProcedureDeclaration ";" || ForwardDeclaration ";" )) **
)) DeclarationSequence rule

(( (( "CONST" (( ConstantDeclaration ";" )) ** )) ??
   (( "TYPE" (( TypeDeclaration ";" )) ** )) ??
   (( "VAR" (( VariableDeclaration ";" )) ** )) ??
   (( ProcedureHeading ";" )) **
)) <- DefSequence
(( ident (( ":" ident )) ?? )) <- import
(( "IMPORT" import "," && ";" )) <- ImportList
(( "MODULE" ident ";" ImportList ?? DeclarationSequence
    (( "BEGIN" StatementSequence )) ?? "END" ident "." )) <- module
(( "DEFINITION" ident ";" ImportList ?? DefSequence "END" ident "." )) <- definition
(( module || definition )) <- CompilationUnit

CompilationUnit parser oberon

