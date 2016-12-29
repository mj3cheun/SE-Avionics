// Avionics Firmware Suite
// Orbital Dynamics
// Modded Version

public static string LCDIdentifier = "[LCD]";
public static string debugIdentifier = "[DEBUG]";
public static string controllerIdentifier = "[MAIN]";
public static string warningSoundIdentifier = "[WARNING]";
public static string groundGearIdentifier = "[GROUND]";
public static bool debugOn = true;

public static bool firstRun = true;
public static IMyGridTerminalSystem _GridTerminalSystem;
public static display LCD;
public static state ship;
public static navigation navigationService;
public static double dLastRun;
public static double elapsedTime;

void Main()
{
    dLastRun = Runtime.TimeSinceLastRun.TotalSeconds;
    elapsedTime += dLastRun;
    _GridTerminalSystem = GridTerminalSystem;
    initialize();
    LCD.executeInstructions();
}

public static void initialize()
{
    LCD = new display();
    LCD.debugWrite("DEBUG START", false);
    if (firstRun == true)
    {
        ship = new state();
        navigationService = new navigation();
        firstRun = false;
    }
    else
    {
        ship.refresh();
        navigationService.refresh();
    }
}

public class executor
{
    public int xLength = 80;
    public int yLength = 50;
    public float fontSize = 1f;
    public Color fontColor = Color.White;
    public bool invertBar = false;
    public string prefix = "";
    public string br = "\n";
    public string brDelimiter = "<br>";
    public string instructionSet = "";
    public char instructionDelimiter = ';';
    public char argumentDelimiter = ':';
    public char subArgumentDelimiter = ',';
    private string output = "";

    public string executeInstruction(string instructions)
    {
        this.instructionSet = instructions;
        string[] instructionSet = this.instructionSet.Split(instructionDelimiter);
        for (int i = 0; i < instructionSet.Length; i++)
        {
            string[] operation = instructionSet[i].Replace("\n", "").Replace(brDelimiter, "\n").Split(argumentDelimiter);

            switch (operation[0].Trim())
            {
                case "prefix":
                    LCD.debugWrite(operation[0], true);
                    this.prefix = operation[1];
                    break;

                case "nl":
                    output += br + prefix;
                    break;

                case "invertBar":
                    LCD.debugWrite(operation[0], true);
                    this.invertBar = Convert.ToBoolean(operation[1]);
                    break;

                case "viewportSize":
                    {
                        LCD.debugWrite(operation[0], true);
                        string[] args = operation[1].Split(subArgumentDelimiter);
                        this.xLength = int.Parse(args[0]);
                        this.yLength = int.Parse(args[1]);
                        LCD.debugWrite(this.xLength.ToString() + "," + this.yLength.ToString(), true);
                    }
                    break;

                case "setStyle":
                    {
                        LCD.debugWrite(operation[0], true);
                        string[] args = operation[1].Split(subArgumentDelimiter);
                        this.fontSize = float.Parse(args[0]);
                        this.fontColor = new Color(Int32.Parse(args[1]), Int32.Parse(args[2]), Int32.Parse(args[3]));
                    }
                    break;

                case "echo":
                    {
                        LCD.debugWrite(operation[0], true);

                        for (int j = 1; j < operation.Length; j++)
                        {
                            output += operation[j];
                        }
                    }
                    break;

                case "getRealTime":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "24hr":
                                output += DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                break;

                            case "12hr":
                                output += DateTime.Now.ToString("hh:mm:ss tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "getAirspeed":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(navigationService.currentPosition.velocityMagnitude / double.Parse(args[1]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(navigationService.currentPosition.velocityMagnitude / double.Parse(args[1]) * 100.0) + "%";
                                break;

                            case "displayAirspeed":
                                output += navigationService.currentPosition.velocityMagnitude.ToString() + "m/s";
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "getAltimeter":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(navigationService.currentPosition.altitude / double.Parse(args[1]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(navigationService.currentPosition.altitude / double.Parse(args[1]) * 100.0) + "%";
                                break;

                            case "displayAltitude":
                                output += navigationService.currentPosition.altitude.ToString() + "m";
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "totalPowerUsed":
                    {
                        LCD.debugWrite(operation[0], true);
                        double powerUsed = ship.powerUsed();
                        double percentage = powerUsed / ship.powerAvailable();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(percentage, xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(percentage * 100.0) + "%";
                                break;

                            case "displayPowerUsed":
                                output += helper.returnFormattedPower(powerUsed);
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "totalPowerAvailable":
                    LCD.debugWrite(operation[0], true);
                    output += helper.returnFormattedPower(ship.powerAvailable());
                    break;

                case "totalBatteryStored":
                    {
                        LCD.debugWrite(operation[0], true);
                        double batteryStored = ship.batteryPowerStored();
                        double percentage = batteryStored / ship.batteryPowerMax();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(percentage, xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(percentage * 100.0) + "%";
                                break;

                            case "displayBatteryStored":
                                output += helper.returnFormattedPower(batteryStored) + "h";
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "totalBatteryMax":
                    LCD.debugWrite(operation[0], true);
                    output += helper.returnFormattedPower(ship.batteryPowerMax()) + "h";
                    break;

                case "getShipInv":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        double numItems = ship.getShipInv(args[0]);

                        switch (args[1].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(numItems / double.Parse(args[2]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(numItems / double.Parse(args[2]) * 100.0) + "%";
                                break;

                            case "displayNumItems":
                                output += numItems;
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "getShipInvByConName":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        double numItems = ship.getShipInvbyName(args[0], args[1]);

                        switch (args[2].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(numItems / double.Parse(args[3]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += helper.roundNumber(numItems / double.Parse(args[3]) * 100.0) + "%";
                                break;

                            case "displayNumItems":
                                output += numItems.ToString();
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "terrainWarning":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        double secondsToImpactWarn = Convert.ToDouble(args[0]);
                        double speedThreshold = Convert.ToDouble(args[1]);
                        string warningMessage = args[2];

                        if((helper.secondsToDisplace(navigationService.currentPosition.altitude, navigationService.currentPosition.sinkRate, navigationService.currentPosition.sinkRateAcceleration) < secondsToImpactWarn) && (navigationService.currentPosition.sinkRate > speedThreshold))
                        {
                            output += warningMessage;
                            ship.changeNamedSoundState(warningSoundIdentifier, true);
                        }
                        else
                        {
                            ship.changeNamedSoundState(warningSoundIdentifier, false);
                        }
                    }
                    break;

                case "groundGearWarning":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        string warningMessage = args[0];

                        if (ship.getLandingGearStatus(groundGearIdentifier) == true)
                        {
                            output += warningMessage;
                        }
                    }
                    break;

                default:
                    LCD.debugWrite("Unknown Command: " + operation[0], true);
                    break;
            }
        }
        return output != "" ? output : " ";
    }
}

public class navigation
{
    List<IMyTerminalBlock> controller = new List<IMyTerminalBlock>();
    IMyShipController mainControl = null;
    public position currentPosition;
    public position pastPosition;

    public navigation ()
    {
        _GridTerminalSystem.SearchBlocksOfName(controllerIdentifier, controller);
        if(controller.Count > 0)
        {
            mainControl = controller[0] as IMyShipController;
            this.refresh();
        }
    }

    public void refresh ()
    {
        if(mainControl != null)
        {
            if (currentPosition != null)
            {
                pastPosition = currentPosition;
            }
            currentPosition = new position(mainControl, pastPosition);
        }
    }
}

public class position
{
    public readonly double timeSinceLast;
    public readonly Vector3D currentPosition;
    public readonly double velocityMagnitude;
    public readonly Vector3D linearVelocity;
    public readonly Vector3D angularVelocity;
    public readonly double linearAcceleration;
    public readonly Vector3D gravityVector;
    public readonly Vector3D gravityVectorNormal;
    public readonly double gravityMagnitude;
    public readonly Vector3D forwardVector;
    public readonly Vector3D upVector;
    public readonly Vector3D rightVector;

    public readonly double altitude;
    public readonly double sinkRate;
    public readonly double sinkRateAcceleration;

    public readonly bool isInverted;
    public readonly double bankRad;
    public readonly double pitchRad;
    public readonly double heading;
    public readonly Vector3D trueNorth = new Vector3D(0.342063708833718, -0.704407897782847, -0.621934025954579);

    public position(IMyShipController reference, position prevPosition = null)
    {
        MyShipVelocities shipVelocity = reference.GetShipVelocities();
        MyShipMass shipMass = reference.CalculateShipMass();

        timeSinceLast = dLastRun;
        currentPosition = reference.GetPosition();
        velocityMagnitude = reference.GetShipSpeed();
        linearVelocity = shipVelocity.LinearVelocity;
        angularVelocity = shipVelocity.AngularVelocity;
        linearAcceleration = prevPosition != null ? ((velocityMagnitude - prevPosition.velocityMagnitude) / timeSinceLast) : 0;
        gravityVector = reference.GetNaturalGravity();
        gravityMagnitude = gravityVector.Length();
        forwardVector = reference.WorldMatrix.Forward;
        upVector = reference.WorldMatrix.Up;
        rightVector = reference.WorldMatrix.Right;

        if (gravityMagnitude > 0)
        {
            gravityVectorNormal = Vector3D.Normalize(gravityVector);
            reference.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude);
            sinkRate = linearVelocity.Dot(gravityVectorNormal);
            sinkRateAcceleration = prevPosition != null ? ((sinkRate - prevPosition.sinkRate) / timeSinceLast) : 0;

            isInverted = (gravityVectorNormal - upVector).Length() < Math.Sqrt(2.0);
            bankRad = Math.Acos(gravityVectorNormal.Dot(rightVector)) - Math.PI / 2.0;
            pitchRad = Math.Acos(gravityVectorNormal.Dot(forwardVector)) - Math.PI / 2.0;
            heading = computeHeading();
        }
        else
        {
            gravityVectorNormal = gravityVector;
            altitude = -1;
            sinkRate = 0;
            sinkRateAcceleration = 0;

            isInverted = false;
            bankRad = 0;
            pitchRad = 0;
            heading = 0;
        }
    }

    private double computeHeading()
    {
        Vector3D westVector = trueNorth.Cross(gravityVectorNormal);
        Vector3D northVector = gravityVectorNormal.Cross(westVector);
        double westComponent = forwardVector.Dot(westVector);
        double northComponent = forwardVector.Dot(northVector);

        double headingAngle = Math.Atan(northComponent / westComponent) + Math.PI / 2;
        return headingAngle + (westComponent < 0 ? 0 : Math.PI);
    }
}

public class state
{
    List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> reactors = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> reloadableRockets = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> gatlings = new List<IMyTerminalBlock>();
    public Dictionary<string, bool> soundStates = new Dictionary<string, bool>();

    public void refresh()
    {
        _GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors);
        _GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);
        _GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
        _GridTerminalSystem.GetBlocksOfType<IMySmallMissileLauncherReload>(reloadableRockets);
        _GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(gatlings);
    }

    public double powerUsed()
    {
        double totalOutput = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if (reactors[i].DetailedInfo.Contains("Current Output"))
            {
                string iString = reactors[i].DetailedInfo.Substring(reactors[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("Current Output"))
            {
                string iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return totalOutput;
    }

    public double powerAvailable()
    {
        double availableOutput = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if ((reactors[i].DetailedInfo.Contains("Current Output")) && reactors[i].IsWorking)
            {
                string iString = reactors[i].DetailedInfo.Substring(reactors[i].DetailedInfo.IndexOf("Max Output") + 12);
                availableOutput += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if ((batteries[i].DetailedInfo.Contains("Current Output")) && batteries[i].IsWorking)
            {
                string iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Max Output") + 12);
                availableOutput += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return availableOutput;
    }

    public double batteryPowerMax()
    {
        double maxPower = 0;
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("Max Stored Power"))
            {
                string iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Max Stored Power") + 18);
                maxPower += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return maxPower;
    }

    public double batteryPowerStored()
    {
        double storedPower = 0;
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("\nStored power"))
            {
                string iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Stored power") + 14);
                storedPower += double.Parse(iString.Substring(0, iString.IndexOf(" "))) * (helper.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? helper.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return storedPower;
    }

    public bool getLandingGearStatus(string groundGearName)
    {
        List<IMyTerminalBlock> groundGearList = new List<IMyTerminalBlock>();
        _GridTerminalSystem.SearchBlocksOfName(groundGearName, groundGearList);
        int numLockedGears = 0;

        if(groundGearList.Count > 0)
        {
            foreach (IMyTerminalBlock groundGear in groundGearList)
            {
                StringBuilder lockState = new StringBuilder();
                groundGear.GetActionWithName("SwitchLock").WriteValue(groundGear, lockState);

                numLockedGears += (lockState.ToString() == "Locked") ? 1 : 0;
            }
        }
        else
        {
            numLockedGears = 1;
        }

        return numLockedGears == groundGearList.Count ? true : false;
    }

    public double getShipInv(string itemType)
    {
        List<IMyTerminalBlock> shipInv = new List<IMyTerminalBlock>();
        List<IMyInventoryItem> itemsList = new List<IMyInventoryItem>();
        _GridTerminalSystem.GetBlocks(shipInv);
        double numItems = 0;
        string invstr = "";

        for (int i = 0; i < shipInv.Count; i++)
        {
            IMyInventory inv = shipInv[i].GetInventory(0);
            if (inv != null)
            {
                List<IMyInventoryItem> items = inv.GetItems();
                itemsList.AddRange(items);
            }
        }

        for (int i = 0; i < itemsList.Count; i++)
        {
            //LCD.debugWrite(itemsList[i].Content.SubtypeName, true);
            invstr = itemsList[i].Content.SubtypeName;
            if (itemType == itemsList[i].Content.SubtypeName)
            {
                numItems += double.Parse(itemsList[i].Amount.ToString());
            }
        }

        return numItems;
    }

    public double getShipInvbyName(string invName, string itemType)
    {
        List<IMyTerminalBlock> shipInv = new List<IMyTerminalBlock>();
        List<IMyInventoryItem> itemsList = new List<IMyInventoryItem>();
        _GridTerminalSystem.SearchBlocksOfName(invName, shipInv);
        double numItems = 0;
        string invstr = "";

        for (int i = 0; i < shipInv.Count; i++)
        {
            IMyInventory inv = shipInv[i].GetInventory(0);
            if (inv != null)
            {
                List<IMyInventoryItem> items = inv.GetItems();
                itemsList.AddRange(items);
            }
        }

        for (int i = 0; i < itemsList.Count; i++)
        {
            LCD.debugWrite(itemsList[i].Content.SubtypeName.ToString(), true);
            LCD.debugWrite(itemsList[i].Amount.ToString(), true);
            invstr = itemsList[i].Content.SubtypeName;
            if (itemType == itemsList[i].Content.SubtypeName)
            {
                numItems += double.Parse(itemsList[i].Amount.ToString());
            }
        }

        return numItems;
    }

    public void changeNamedSoundState(string name, bool state)
    {
        if (!soundStates.ContainsKey(name))
        {
            soundStates[name] = !state;
        }
        if(state != soundStates[name])
        {
            List<IMyTerminalBlock> soundBlockList = new List<IMyTerminalBlock>();
            _GridTerminalSystem.SearchBlocksOfName(name, soundBlockList);
            LCD.debugWrite(soundBlockList.Count.ToString(), true);
            foreach (IMyTerminalBlock soundBlock in soundBlockList)
            {
                ((IMySoundBlock)soundBlock).ApplyAction(state ? "PlaySound" : "StopSound");
            }
            soundStates[name] = state;
        }
    }
}

public class display
{
    List<IMyTerminalBlock> debugPanels = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> displayPanels = new List<IMyTerminalBlock>();

    public display()
    {
        if (debugOn)
        {
            _GridTerminalSystem.SearchBlocksOfName(debugIdentifier, debugPanels);
        }
        _GridTerminalSystem.SearchBlocksOfName(LCDIdentifier, displayPanels);
    }

    public void debugWrite(string write, bool append)
    {
        for (int i = 0; i < debugPanels.Count; i++)
        {
            IMyTextPanel panel = (IMyTextPanel)debugPanels[i];
            panel.WritePublicText(write + "\r\n", append);
            panel.ShowPublicTextOnScreen();
            panel.UpdateVisual();
        }
    }

    public void executeInstructions()
    {
        for (int i = 0; i < displayPanels.Count; i++)
        {
            LCD.debugWrite("Executing Panel " + i, true);
            IMyTextPanel panel = (IMyTextPanel)displayPanels[i];
            string customData = panel.CustomData;
            executor thisDisplay = new executor();
            panel.WritePublicText(thisDisplay.executeInstruction(customData), false);

            panel.SetValue("FontColor", thisDisplay.fontColor);
            panel.SetValue("FontSize", thisDisplay.fontSize);
            panel.ShowPublicTextOnScreen();
            panel.UpdateVisual();
            thisDisplay = null;
        }
    }

    public string renderPercentageBar(double percentage, int xLength, bool invertBar = false)
    {
        string barStart = "[ ";
        string barEnd = " ]";
        string barFill = "|";
        string barEmpty = "'";

        string output = barStart;

        double barFillNum = ((percentage) * (xLength - barStart.Length - barEnd.Length)) / barFill.Length;
        for (int i = barStart.Length + barEnd.Length - 2; i < xLength - barEmpty.Length - 1; i++)
        {
            if (((i < barFillNum + barStart.Length + barEnd.Length - 2) && (invertBar == false)) || ((i > -(barFillNum + barStart.Length + barEnd.Length - 2 - xLength)) && (invertBar == true)))
            {
                output += barFill;
            }
            else
            {
                output += barEmpty;
                i += (barEmpty.Length - 1);
            }
        }

        return output + barEnd;
    }
}

static class helper
{
    public static Dictionary<string, double> powerConversion = new Dictionary<string, double>
    {
        {"W", 1.0},
        {"k", 1000.0},
        {"M", 1000000.0},
        {"G", 1000000000.0}
    };

    public static string returnFormattedPower(double power)
    {
        if (power > powerConversion["G"])
        {
            return (power / powerConversion["G"]).ToString() + " GW";
        }
        else if (power > powerConversion["M"])
        {
            return (power / powerConversion["M"]).ToString() + " MW";
        }
        else if (power > powerConversion["k"])
        {
            return (power / powerConversion["k"]).ToString() + " kW";
        }
        else
        {
            return power.ToString() + " W";
        }
    }

    public static string roundNumber(double number)
    {
        if (number < 0.0)
        {
            return number.ToString("0.000");
        }
        else if (number < 100.0)
        {
            return number.ToString("00.00");
        }
        else
        {
            return number.ToString("000.0");
        }
    }

    public static double secondsToDisplace(double displacement, double velocity = 0, double acceleration = 0)
    {
        double seconds = 0;
        if (acceleration != 0)
        {
            seconds = (Math.Sqrt(2.0 * acceleration * displacement + Math.Pow(velocity, 2.0)) - velocity) / acceleration;
        }
        else if (velocity != 0)
        {
            seconds = displacement / velocity;
        }
        else
        {
            seconds = displacement == 0 ? 0 : long.MaxValue;
        }

        return seconds;
    }

    public static double RadianToDegree(double angle)
    {
        return angle * (180.0 / Math.PI);
    }
}