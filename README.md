# NationStates Nation Manager #

## Overview ##
NationStates Nation Manager is a free and open source Windows application that displays the status of a list of nations. The application can also log into these nations or restore them if they have ceased to exist. 

The application is designed to help players maintain a large number of puppet nations.

## Features ##
* Retrieves the status of nations, including whether or not they exist and their last login date
* Logs into nations to prevent them from ceasing to exist
* Restores nations that have ceased to exist

## Documentation ##

### System Requirements ###
* The [.NET Framework 4.5](http://www.microsoft.com/en-ca/download/details.aspx?id=30653)
* A [NationStates](http://www.nationstates.net) nation

### Help ###

#### Adding a nation ####
1. Click on the menu item labelled *Nations*, then click *Add...*. 
2. Type in the name and password of a nation, and then click *OK*. The nation will appear in the list view, and its status will refresh.

#### Editing a nation ####
1. Click on the menu item labelled *Nations*, then click *Edit...*. 
2. Edit the name and password of the nation, and then click *OK*. The updated nation will appear in the list view, and its status will refresh.

#### Refreshing a nation's status #####
1. Select the nations whose statuses you wish to refresh.
2. Click on the menu item labelled *Nations*, then click *Refresh*. The nation's status will refresh.

#### Removing a nation #####
1. Select the nations you wish to remove.
2. Click on the menu item labelled *Nations*, then click *Remove*. The nation will be removed from the list view.

#### Logging into a nation #####
1. Select the nations you wish to log into.
2. Click on the menu item labelled *Nations*, then click *Login*. The application will attempt to log into the nation and display a success or error message, and its status will refresh.

**Note**: You cannot log into a nation that does not currently exist or whose status has not yet been successfully retrieved due to network issues. In the latter case, you will need to refresh the nation's status until it is successfully retrieved.

#### Restoring a nation that has ceased to exist ####
1. Select the nations you wish to restore.
2. Click on the menu item labelled *Nations*, then click *Restore*. The application will attempt to restore the nation and display a success or error message, and its status will refresh.

**Note**: You cannot log into a nation that does not currently exist or whose status has not yet been successfully retrieved due to network issues. In the latter case, you will need to refresh the nation's status until it is successfully retrieved. In addition, you cannot restore more than one nation at a time, due to NationStates scripting restrictions.

#### Creating a new list of nations ####
1. Click on the menu item labelled *File*, then click *New*. All nations will be removed from the list view. You may be prompted to save the existing list of nations.

#### Saving a list of nations ####

##### As encrypted binary data #####
1. Click on the Menu item labelled *File*, then click *Save As...*.
2. Choose a destination to save the file, then click *OK*.
3. Choose a password, then click *OK*. The list of nations will be saved to disk.

**Note**: The file containing the list of nations is encrypted using the AES algorithm and a 128 bit key. If you lose the password, you will be unable to open the file and the data contained within cannot be recovered.

##### As plain text ######
1. Click on the Menu item labelled *File*, then click *Export...*
2. Choose a destination to save the file, then click *OK*. The list of nations will be saved to disk.

**Note**: The list of nations and their passwords will be saved in a comma separated values format:

	Nation1, Password1
	Nation2, Password2
	Nation3, Password3

#### Opening a list of nations ####

##### As encrypted binary data #####
1. Click on the Menu item labelled *File*, then click *Open...*.
2. Choose the file to open, then click *OK*.
3. Enter the file's password, then click *OK*. The file will be opened and a list of nations will be read from disk. You may be prompted to save the existing list of nations.

##### As plain text ######
1. Click on the Menu item labelled *File*, then click *Open...*.
2. Choose the file to open, then click *OK*. The file will be opened and a list of nations will be read from disk. You may be prompted to save the existing list of nations.

**Note**: The nation file must be in the comma separated values format described above.

#### Changing the user agent ####
1. Click on the menu item labelled *Tools*, then click *Options*.
2. Type in a user agent in the text box labelled *User agent*, then click *OK*. The user agent will be changed.

**Note**: You cannot use the application without setting a user agent. You will be prompted to do so when the application starts for the first time.

## Downloads ##

Source code downloads are available from [the project's GitHub page](https://github.com/auralia/nationstates-puppet-manager).

Binary downloads of the latest version (0.2) are available from the [author's website](http://www.auralia.me).

## Copyright and License ##
Copyright (C) 2013 Auralia.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

This program uses icons from the [Silk Icon Set](http://www.famfamfam.com/lab/icons/silk/) and the [Tango Desktop Project](http://tango.freedesktop.org/).
