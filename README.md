#Space Engineers Fighter Avionics
The SE Avionics suite is software written to be run on the programmable block found in the Space Engineers video game. Its purpose is to provide a highly customisable framework for the accumulation and display of real time combat flight data. As such, statistics such as power usage, ammunition count, and speed are supported.

The software runs on the programmable block, using a timer to set up a loop. By default, it will look for blocks labeled with "[LCD]" and access the CustomData field for instructions. The screen output is output onto the PublicText field on the LCD block, which is then used to refresh the display. Blocks labelled with "[DEBUG]" will be used to output debug information.

The SE Avionics suite prioritizes flexibility over ease of use. As result, one must explicitly insert line breaks and add spaces and labels to information where necessary. In the default version, there are several delimiters which are used to organize input. The semicolon delimiter is used to seperate commands from one another. The colon delimiter is used to seperate the command from the arguments. The comma delimiter is used to seperate different arguments. All delimiter characters can not be used as input. As such, the delimiter character used for each task can be changed in the code as necessary.

Valid syntax which can be used to specify information displayed are listed below.

##Syntax
**prefix**

The prefix instruction adds the given argument to the beginnning of every new line thereafter. The argument can consist of whitespace.

Examples
```
prefix:   ;
```

**nl**

The nl instruction adds a new line wherever the cursor is at.

**invertBar**

The invertBar instruction reverses the direction in which any added percentage bar travels thereafter from left to right, which is the default direction, to right to left. It takes one argument, which is a boolean setting invertBar true and false.

**viewportSize**

The viewportSize instruction sets the width and height that the output can use thereafter. It is primarily used for setting the width of percentage bars and the like. This setting can be used multiple times in the same instruction set. It has two arguments setting width and height.

Examples
```
viewportSize:30,1;
```

**setStyle**

The setStyle instruction sets the font size and colour of the LCD panel. Note that each LCD can only have font size and colour, repeated setStyle instructions will just result in the newest style overriding all others. It uses 4 arguments in total. The first argument sets the font size. The next three arguments set the colour using an RGB code, where each successive argument determines R, G, and B in order, each number representing the intensity of each colour ranging from 0 to 255.

Examples
```
setStyle:4,255,0,0;
```

**echo**

The echo instruction is used to print text onto the screen. All characters except delimiter and newline characters are valid input which will be displayed exactly as typed. In order to insert newline characters, either end the echo argument and use the nl instruction, or use the dedicated newline character, which by default is "\<br\>".

Examples
```
echo:This is the first line<br>This is the second line;
```

**getRealTime**

The getRealTime instruction is used to print the time on the screen using computer time settings. There are two arguments that can be used:

- 24hr:
Returns the time using a 24 hour format, which includes hour, minute, and second.

- 12hr:
Returns the time using a 12 hour format, which includes hour, minute, second, and AM or PM.

Examples
```
getRealTime:24hr;
getRealTime:12hr;
```

**getAirspeed**

The getAirspeed instruction is used to display the speed of the ship relative to the air. There are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar. Requires a third argument, which specifies how fast the ship should be going to reach 100%.

- displayPercentage:
Returns the data in the form of a numerical percentage. Requires a third argument, which specifies how fast the ship should be going to reach 100%.

- displayAirspeed:
Returns the airspeed in numerical form.

Examples
```
getAirspeed:displayPercentageBar,105;
getAirspeed:displayAirspeed;
```

**getAltimeter**

The getAltimeter instruction is used to display the altitude of the ship relative to the ground. There are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar. Requires a third argument, which specifies how high the ship should be to reach 100%.

- displayPercentage:
Returns the data in the form of a numerical percentage. Requires a third argument, which specifies how high the ship should be to reach 100%.

- displayAltitude:
Returns the altitude in numerical form.

Examples
```
getAltimeter:displayPercentageBar,5000;
getAltimeter:displayAltitude;
```

**getCompass**

The getCompass instruction is used to display the heading of the ship relative to absolute north. There are two arguments that can be used:

- displayCompass:
Creates an animated compass which places the rough heading of the ship in the middle. Requires a second argument, which specifies the distance, more specifically, the number of dashes that should be used in between each number interval. This instruction depends on the viewportSize to get the width of the display.

- displayHeading:
Returns the altitude in numerical form.

Examples
```
getCompass:displayCompass,4;
getCompass:displayHeading;
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

**getShipInv**

The getShipInv instruction is used to find the number of a specified item which are accessible to the programmable block by iterating through all available inventories at the moment the command is executed. The first argument is always the item identifier of the item that you want to enumerate. A full list of the valid item identifiers along with their common names can be found at the bottom of the document. Just following, there are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar. Requires a third argument, which specifies how many items it should take to reach 100%.

- displayPercentage:
Returns the data in the form of a numerical percentage. Requires a third argument, which specifies how many items it should take to reach 100%.

- displayNumItems:
Returns the number of specified item found in current grid.

Examples
```
getShipInv:Missile200mm,displayPercentageBar, 100;
getShipInv:Missile200mm,displayNumItems;
```

**getShipInvByConName**

The getShipInvByConName instruction is used to find the number of a specified item which are accessible by the programmable block at the moment the command is executed. The first and second arguments are always the name of the targeted inventory and the item identifier of the item that you want to enumerate. A full list of the valid item identifiers along with their common names can be found at the bottom of the document. Just following, there are three arguments that can be used:

- displayPercentageBar:
Returns the data in the form of a percentage bar. Requires a fourth argument, which specifies how many items it should take to reach 100%.

- displayPercentage:
Returns the data in the form of a numerical percentage. Requires a fourth argument, which specifies how many items it should take to reach 100%.

- displayNumItems:
Returns the number of specified item found in current grid.

Examples
```
getShipInvByConName:Connector 2,NATO_25x184mm,displayPercentageBar,1000;
getShipInvByConName:Connector 2,NATO_25x184mm,displayNumItems;
```

**terrainWarning**

The terrainWarning instruction is used to display a warning when the program detects an imminent crash based on the sink rate, sink rate acceleration, and altitude. In addition, it will try to activate any sound blocks with the [WARNING] identifier. There are three arguments that must follow afterwards. The next argument is the time-to-impact threshold, which sets the minimum calculated time to impact in seconds which is required to trigger the warning. The next argument is the minimum speed threshold, which turns off the warning under a certain speed in m/s. The last argument sets the text to be displayed on the LCD when the warning is active.

Examples
```
terrainWarning:4,25,   TERRAIN;
```

**groundGearWarning**

The groundGearWarning instruction is used to display a warning when the program detects that all landing gears labeled with the ground gear identifier, which by default is "[GROUND]", are locked. If only a fraction of the gears are locked, the warning will not show. It takes one argument, which sets the text that is displayed on the LCD when the warning is active.

Examples
```
groundGearWarning:  LOCKED;
```