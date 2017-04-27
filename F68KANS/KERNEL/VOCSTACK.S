.DATA

*****************************************************************
*                >PART 'vocabulary stack'

*************************************************************************
*       vocabulary stack                                                *
*************************************************************************
                DC.L 0          ;last    user's buffer
                DC.L 144        ;length is 144 bytes
VOCPA:          DC.L 8         ;height of voc-stacks
                DC.L (last_forth-datas) ;context (pfa)
                DC.L (last_forth-datas) ;transient (pfa)
                DS.L 33         ;room for another 32
                EVEN

*                ENDPART
