# Puppet AutoLogin #

## Introduction ##
Puppet AutoLogin automatically logs into NationStates nations.

## Documentation ##

### System Requirements ###
* The [.NET Framework 4 Full](http://www.microsoft.com/download/en/details.aspx?id=17851)
* A [NationStates](http://www.nationstates.net) nation
* An email address

### Tutorial ###
1. Click on the tab labeled *Puppets*, then type your puppet login information in the textbox below. TYpe in your email address as well.
	* Each nation name and password should be separated by a comma (no spaces). One nation name and password per line.
	* Your email address is required in addition to your username and password so that the NationStates administrators can contact you in the event of a problem with the program.
2. Click on the tab labeled *Login* button, and start the process by clicking *Start*.
	* You should always save the log after the process completes by right-clicking the log and clicking *Save As*, in case there is a problem with the program.
	* You can cancel the process at any time by pressing *Cancel*. There may be a delay before the process is cancelled since it is running on a separate thread.

#### Technical ####
* This program reports its UserAgent to the NationStates API as follows:
	`Pupppet AutoLogin <version>
	Program author (not responsible for use): Auralia (federal.republic.of.auralia@gmail.com)
	Current user (responsible for use): <username> (<email>)`

## Changelog ##
* Version 0.1:
	* Initial public release

## Copyright and License ##
Copyright (C) [Auralia](http://www.nationstates.net/nation=auralia).

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

This program includes [code from Coffee and Crack](http://forum.nationstates.net/viewtopic.php?p=8502718) and [code from Microsoft](http://msdn.microsoft.com/en-us/library/01escwtf.aspx).

This program uses icons from the [Silk Icon Set](http://www.famfamfam.com/lab/icons/silk/) and the [Tango Desktop Project](http://tango.freedesktop.org/).