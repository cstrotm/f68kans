F68KANS
a Forthsystem for 680x0-computer


* experimental release *



Important note:

F68KANS is a low-cost product, made by a community of sophisticated
developers. Thereby the main goal was to achieve the greatest possible
security and reliablity. So yau as a user or a programmer can be sure to
get high-tech in spite of low-cost.

But: F68KANS is not a professional product (in the sense of the word, no
in the sense often used). Professional products may be (and often are) 
maintained and supported better. They have better documentation and
finish. Professional products are made for their users. F68KANS was made
for its  developers.  

So if you miss something, do not blame the language Forth, but this
implementation of the language. If you want to have a professional
system, you also should buy a professional system.


If you have any question about F68KANS or Forth in general contact:

Dipl. Phys. Joerg Plewe
Haarzopfer Str. 32
4330 Muelheim an der Ruhr
+49 (0)208/497068
jps@Forth-eV.de



What is it about?

F68KANS is an ANSI Forth system for any m680x0 computer. It is highly
portable even as a binary image.

This release contains all to run F68KANS on Atari computers. All the
necessary to make F68KANS run on any other machine is also included.

The implementation on Atari machines is highly elaborated. So there is really 
a lot of stuff usable on Atari only. When you port F68KANS, you will have to 
decide, what you can or should use. All the thing not located under the ATARI
subdirectory are of broader interest, which does not mean that things in this
subdirectory are of no interest.  



Installation:

Just copy all the stuff from floppy to your harddisk or even leave it on
the floppy. Do not forget to make a backup!!


What did you get?
What should you have?

I now will try to describe whta is in the distribtution of this release.
To keep this part of the documentation flexible, I will only describe
files and directories at one level. You can expect README.TXT files in
some of the subdirectories describing its contents.

I strongly advice you not to store any of your own sources under the
F68KANS subdirectory. This will make it possible for you just to
exchange the whole F68KANS directory with the next release. You should
think of this possibilty also when you intend to change sources of the
distribution. For the same reason you should be careful to use
non-standard worsin your application, because there is no garantuee that
these words still exist in another release or keep their meaning.


File in F68KANS:

	README.TXT
This file.

	Sometimes: F68KANS.IMG
This is the binary system image. It contains the most basic part of the
system written in assembler. By default, I store it somewhere in the
ATARI subdirectory for practical reasons. Look for it, it is important!


Directories in F68KANS:

	KERNEL
Here are all the assembler sources of the basic kernel. They will lead
to the file F68KANS.IMG. There are some system dependent steps in the
generation of F68KANS.IMG, which I do not wnat to explain here, because
you never will have to do the generation of the kernel.  


	SYSFTH
This directory contains Forth sources necessary to build a full featured
Forth system from the basic kernel. In this turn, non-ANSI-words are
defined as well. You should avoid to any cost to use these words,
because they may be a matter of change without notice. Do never rely on them! 
The only reason to change any of the sources contained in SYSFTH is due
to porting the system itself. for application programmers, these files
should be considered as read-only.


	APPFTH
Here are all sources which are based on ANSI-kernel, but are strongly
related to the development environment- Here, e.g. graphics may be
defined. So these sources of course may be system dependent. Alter them
as a system provider, but do not touch them as an application
programmer.
 

	TST
Some files to test the functionality of the thing in SYSFTH and APPFTH.
Take them as examples and demos!


	ATARI
All the ATARI stuff, especially the ATARI loader, with all its
connections to the GEM/TOS operating system. Some project files and
utilities of my environment can be found here.

