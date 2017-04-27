/*

		This file was created with ACS2.20 (c) Stefan Bachert 1991, 1992, 1993, 1994
		The contents is based on file: C:\F\F68KANS\ATARI\ACS\F68KACS.ACS
		compiled at: Wed Mar 01 22:36:12 1995
*/


/*	 PLATFORM  */
#define ACS_ATARI


/*	 STRINGS	 */



/*	 ICONS	 */



/*	 MENU TREES	 */



/*	 OBJECT TREES	 */



/*	 WINDOWS	 */



extern Adescr ACSdescr;



/*		ANSI-C Prototypes		*/

extern INT16 CDECL A_arrows (PARMBLK* pb);
extern INT16 CDECL A_ftext (PARMBLK* pb);
extern INT16 CDECL A_innerframe (PARMBLK* pb);
extern INT16 Auo_ftext (OBJECT* entry, INT16 task, void* in_out);
extern INT16 Auo_string (OBJECT* entry, INT16 task, void* in_out);
extern void doloadscrap (void);
extern void doloadsel_from_interpreter (void);
extern INT16 editor_key (Awindow* wi, INT16 kstate, INT16 key);
extern void handle_caps (void);
extern void startupstream (void);


