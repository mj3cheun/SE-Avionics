#Space Engineers Fighter Avionics
The SE Avionics suite is software written to be run on the programmable block found in the Space Engineers video game. Its purpose is to provide a highly customisable framework for the accumulation and display of real time combat flight data. As such, statistics such as power usage, ammunition count, and speed are supported.

The software runs on the programmable block, using a timer to set up a loop. By default, it will look for blocks labeled with "[LCD]" and access the CustomData field for instructions. The screen output is output onto the PublicText field on the LCD block, which is then used to refresh the display. Blocks labelled with "[DEBUG]" will be used to output debug information.

The SE Avionics suite prioritizes flexibility over ease of use. As result, one must explicitly insert line breaks and add spaces and labels to information where necessary. In the default version, there are several delimiters which are used to organize input. The semicolon delimiter is used to seperate commands from one another. The colon delimiter is used to seperate the command from the arguments. The comma delimiter is used to seperate different arguments. All delimiter characters can not be used as input. As such, the delimiter character used for each task can be changed in the code as necessary.

Valid syntax which can be used to specify information displayed are listed below.

##Instructions
**prefix**

The prefix instruction adds the given argument to the beginnning of every new line thereafter. The argument can consist of whitespace.

Examples
```
prefix:   ;
```

**nl**

The nl instruction adds a new line wherever the cursor is at.

**viewportSize**

The viewportSize instruction sets the width and height that the output can use thereafter. It is primarily used for setting the width of percentage bars and the like. This setting can be used multiple times in the same instruction set. It has two arguments setting width and height.

Examples
```
viewportSize:30,1;
```

**echo**

The echo instruction is used to print text onto the screen. All characters except delimiter and newline characters are valid input which will be displayed exactly as typed. In order to insert newline characters, either end the echo argument and use the nl instruction, or use the dedicated newline character, which by default is "\<br\>".

Examples
```
echo:This is the first line<br>This is the second line;
```

**totalPowerUsed**

The totalPowerUsed instruction is used to display the percentage of power that is being consumed by the ship out of its total available power at the moment the command is executed. There are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar.

- displayPercentage:
Returns the data in the form of a numerical percentage.

- displayPowerUsed:
Returns the amount of power being used in numerical form.

Examples
```
totalPowerUsed:displayPercentageBar;
```

**totalPowerAvailable**

The totalPowerAvailable instruction returns the maximum amount of power that can be provided by all of the ship's power sources in numerical form.

**totalBatteryStored**

The totalBatteryStored instruction is used to display the percentage of power left inside all of the ship's batteries out of its maximum battery power capacity at the moment the command is executed. There are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar.

- displayPercentage:
Returns the data in the form of a numerical percentage.

- displayBatteryStored:
Returns the amount of power being used in numerical form.

Examples
```
totalBatteryStored:displayPercentageBar;
```

**totalBatteryMax**

The totalPowerAvailable instruction returns the maximum amount of power that can be stored by all of the ship's batteries in numerical form.
