# NationStates Nation Manager #

## Overview ##
NationStates Nation Manager is a free and open source Windows application that displays the status of a user-specified list of nations. The application can also log into these nations or restore them if they have ceased to exist. 

The application is designed help players maintain a large number of puppets.

## Features ##
* Retrieves existence status and last login date for nations
* Logs into nations to prevent them from ceasing to exist (CTE'ing)
* Restores nations that have ceased to exist (CTE'd)

## Documentation ##

### System Requirements ###
* The [.NET Framework 4.5](http://www.microsoft.com/en-ca/download/details.aspx?id=30653)
* A [NationStates](http://www.nationstates.net) nation

### Help ###

#### Adding a nation ####
1. Click on the menu item labelled *Nations*, then click *Add...*. 
2. Type in the name and password of a nation, and then click *OK*.

#### Editing a nation ####
1. Click on the menu item labelled *Nations*, then click *Edit...*. 
2. Edit the name and password of the nation, and then click *OK*.

#### Refreshing a nation's status #####
1. Select the nations whose statuses you wish to refresh.
2. Click on the menu item labelled *Nations*, then click  *Refresh*.
3. The status field of each nation will update.

#### Removing a nation #####
1. Select the nations you wish to remove.
2. Click on the menu item labelled *Nations*, then click  *Remove*.

#### Logging into a nation #####
1. Select the nations you wish to log into.
2. Click on the menu item labelled *Nations*, then click  *Login*.
3. the status field of each nation will update. If the login attempt succeeded, the last login date will change to the present date and time. If the login attempt failed, an error message will appear.

**Note**: You cannot log into a nation that does not currently exist or whose status has not yet been successfully retrieved. In the latter case, you will need to refresh the nation's status until it is successfully retrieved.

#### Restoring a nation ####
1. Select the nations you wish to restore.
2. Click on the menu item labelled *Nations*, then click  *Restore*.

**Note**: You cannot log into a nation that does not currently exist or whose status has not yet been successfully retrieved. In the latter case, you will need to refresh the nation's status until it is successfully retrieved.

In addition, you cannot restore more than one nation at a time, due to NationStates scripting restrictions.

#### Creating a new list of nations ####
1. Click on the menu item labelled *File*, then click  *New*.

#### Saving a list of nations ####

##### As encrypted binary data #####
1. Click on the Menu item labelled *File*, then click *Save As...*.
2. Choose a destination to save the file, then click *OK*.
3. Choose a password, then click *OK*.

**Note**: The file containing the list of nations is encrypted using the AES algorithm and a 128 bit key. If you lose the password, you will be unable to open the file and the data contained within cannot be recovered.

##### As plain text ######
1. Click on the Menu item labelled *File*, then click *Export...*
2. Choose a destination to save the file, then click *OK*.

**Note**: The list of nations (and their corresponding passwords) will be saved in the comma separated values file format:

	Nation1, Password1
	Nation2, Password2
	Nation3, Password3
	[...]

#### Opening a list of nations ####

##### As encrypted binary data #####
1. Click on the Menu item labelled *File*, then click *Open...*.
2. Choose the file to open, then click *OK*.
3. Enter the file's password, then click *OK*. 

##### As plain text ######
1. Click on the Menu item labelled *File*, then click *Open...*.
2. Choose the file to open, then click *OK*.

#### Changing the user agent ####

1. Click on the menu item labelled *Tools*, then click *Options*.
2. Type in a user agent in the text box labelled *User agent*, then click *OK*.

**Note**: You cannot use the application without setting a user agent. You will be prompted to do so when the application starts for the first time.

## Downloads ##

Source code downloads are available from [the project's GitHub page](https://github.com/auralia/nationstates-puppet-manager).

Binary downloads of the latest version (0.2) are available from the [author's website](http://www.auralia.me).

## Changelog ##
* Version 0.2:
    * Redesigned user interface
    * Status functionality
    * Restore functionality
* Version 0.1:
	* Initial public release

## Copyright and License ##
Copyright (C) 2013 Auralia.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

This program uses icons from the [Silk Icon Set](http://www.famfamfam.com/lab/icons/silk/) and the [Tango Desktop Project](http://tango.freedesktop.org/).