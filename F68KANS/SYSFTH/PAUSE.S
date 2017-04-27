.text
GLOBL main
main:	lea 0(a3,d5.l),a0
		movem.l a6-a7/d6-d7,-(a0) 
		move.l 0(a3,d5.l),d5
		lea -16(a3,d5.l),a0
		movem.l (a0)+,a6-a7/d6-d7
