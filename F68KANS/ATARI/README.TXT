F68KANS
a Forth system for 680x0-computer


* experimental release  *


Notes on the Atari installation:

This file shortly introduces you to the implementation of F68KANS on
Atari ST/TT/FALCON computers. It concentrates on a TOS-only loader
without GEM-support.

F68KANS has been developed on Atari computers, mainly on an Atari TT030.
Here I used highly sophisticated tools available for this machine, which
are of very good quality.


How to start F68KANS:

The TOS loader program is called F68KANS.TTP. As indicated by the
extension of the filename (TOS Takes Parameters) this program allows
you to enter a couple of parameters on startup, which will be described
in the following.

	Filenames 

These filenames describe source files. The loader will open these files
and feed their contents to F68KANS KEY-function. The procedure is called
'streaming in' in the GEM-based loader as well. More than one file may
be specified on the commandline. F68KANS will not echo the content of
these files unless the '-v' option is set. Use this possibilty to
initialize your system interfaces (SI).


	-fFilename

If you have a couple of file for 'stream in' you can write their names
to a file and specify this file with the '-f' option. The file is read
line by line and the content of each line will be taken as a filename
for stream in.  
An example may be my file STREAM.IN which leads from the pure assembler
kernel F68KANS.IMG to a full featured ATARI Forth.


	-cFilename

The loader automatically tries to load a configuration file F68KANS.CFG.
This file contains the filename of the F68KANS binary image and the
sizes of code and date you want to give for F68KANS. Here is an example:

    image: saved.IMG
    code: 0x20000  
    data: 0x40000
    
To use another configuration file use the '-c' option to specify its name.

	-v

Echos all characters that are streamed in at system startup.









