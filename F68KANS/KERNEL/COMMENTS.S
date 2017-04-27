*************************************************************************
*                                                                       *
*                                                                       *
*                               F68KANS                                 *
*                       a portable ANS-FORTHsystem                      *
*                                                                       *
*                            Version 1.0                                *
*                                                                       *
*                                by                                     *
*                             Joerg Plewe                               *
*                          Haarzopfer Str. 32                           *
*                       45472 Muelheim a.d. Ruhr                        *
*                                                                       *
*                       started 7jan1993 at 11.11pm                     *
*                                                                       *
*                                                                       *
*                                                                       *
*************************************************************************

*************************************************************************
*       usage of registers:                                             *
*                                                                       *
* us       equr     d5      ;userarea                                   *
* seg      equr     a2      ;pointer to actual segment                  *
* DT       equr     a3      ;pointer to data segment                    *
* fs       equr     a4      ;Floatingpointstack                         *
* SB       equr     a5      ;pointer to start of system+$8000           *
* ds       equr     a6      ;data-stackpointer                          *
* rp       equr     a7      ;returnstackpointer                         *
*       d6,d7 are used for DO...LOOP                                    *
*                                                                       *
*       a0-a2,d0-d4 are for common use                                  *
*       BE CAREFULL!    not all words save their registers!!!           *
*************************************************************************
*                                                                       *
*************************************************************************
*                                                                       *
*       Bitusage in the controlword:                                    *
*                                                                       *
*       Bit0    smudge, word cannot be found                            *
*       Bit1    immediate, word will execute during compilation         *
*       Bit2    restrict, word may only be used in compiletime          *
*       Bit3    macro, word is a macro                                  *
*                                                                       *
*************************************************************************
*                ENDPART
