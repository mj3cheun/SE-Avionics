// Avionics Firmware Suite
// Orbital Dynamics
// Modded Version

public static string LCDIdentifier = "[LCD]";
public static string debugIdentifier = "[DEBUG]";
public static string controllerIdentifier = "[MAIN]";
public static string warningSoundIdentifier = "[WARNING]";
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
                                output += LCD.renderPercentageBar((float)navigationService.currentPosition.velocityMagnitude / float.Parse(args[1]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage((float)navigationService.currentPosition.velocityMagnitude / float.Parse(args[1]));
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
                                output += LCD.renderPercentageBar((float)navigationService.currentPosition.altitude / float.Parse(args[1]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage((float)navigationService.currentPosition.altitude / float.Parse(args[1]));
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
                        float powerUsed = ship.powerUsed();
                        float percentage = powerUsed / ship.powerAvailable();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(percentage, xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage(percentage * 100);
                                break;

                            case "displayPowerUsed":
                                output += ship.returnFormattedPower(powerUsed);
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "totalPowerAvailable":
                    LCD.debugWrite(operation[0], true);
                    output += ship.returnFormattedPower(ship.powerAvailable());
                    break;

                case "totalBatteryStored":
                    {
                        LCD.debugWrite(operation[0], true);
                        float batteryStored = ship.batteryPowerStored();
                        float percentage = batteryStored / ship.batteryPowerMax();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        switch (args[0].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(percentage, xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage(percentage * 100);
                                break;

                            case "displayBatteryStored":
                                output += ship.returnFormattedPower(batteryStored) + "h";
                                break;

                            default:
                                output += "UNKNOWN OPTION: " + args[0];
                                break;
                        }
                    }
                    break;

                case "totalBatteryMax":
                    LCD.debugWrite(operation[0], true);
                    output += ship.returnFormattedPower(ship.batteryPowerMax()) + "h";
                    break;

                case "getShipInv":
                    {
                        LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        float numItems = ship.getShipInv(args[0]);

                        switch (args[1].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(numItems / float.Parse(args[2]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage(numItems / float.Parse(args[2]) * 100);
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
                        float numItems = ship.getShipInvbyName(args[0], args[1]);

                        switch (args[2].Trim())
                        {
                            case "displayPercentageBar":
                                output += LCD.renderPercentageBar(numItems / float.Parse(args[3]), xLength, invertBar);
                                break;

                            case "displayPercentage":
                                output += formatNumberPercentage(numItems / float.Parse(args[3]) * 100);
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

                        if((ship.secondsToDisplace(navigationService.currentPosition.altitude, navigationService.currentPosition.sinkRate, navigationService.currentPosition.sinkRateAcceleration) < secondsToImpactWarn) && (navigationService.currentPosition.sinkRate > speedThreshold))
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

                default:
                    LCD.debugWrite("Unknown Command: " + operation[0], true);
                    break;
            }
        }
        return output != "" ? output : " ";
    }

    string formatNumberPercentage(float number)
    {
        output = (number < 10000) ? (String.Format("{0,-" + ((output.IndexOf('.') >= 3) ? "6" : "7") + "}", (String.Format("{0:###0.0}%", number)))) : "+9999%";
        output = (output.IndexOf('.') == 1) ? (output + " ") : (output);
        return (output.IndexOf('1') == -1) || (output.IndexOf('.') > 3) ? (output) : (output + " ");
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
    public readonly double gravityMagnitude;
    public readonly Vector3D forwardVector;
    public readonly Vector3D downVector;

    public readonly double altitude;
    public readonly double sinkRate;
    public readonly double sinkRateAcceleration;

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
        downVector = reference.WorldMatrix.Down;

        if (gravityMagnitude > 0)
        {
            reference.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude);
            sinkRate = linearVelocity.Dot(Vector3D.Normalize(gravityVector));
            sinkRateAcceleration = prevPosition != null ? ((sinkRate - prevPosition.sinkRate) / timeSinceLast) : 0;
        }
        else
        {
            altitude = -1;
            sinkRate = 0;
            sinkRateAcceleration = 0;
        }
    }
}

public class state
{
    List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> reactors = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> reloadableRockets = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> gatlings = new List<IMyTerminalBlock>();
    public Dictionary<string, float> powerConversion = new Dictionary<string, float>();
    public Dictionary<string, bool> soundStates = new Dictionary<string, bool>();

    public state()
    {
        //Instantiate Variables
        powerConversion.Add("W", 1);
        powerConversion.Add("k", 1000);
        powerConversion.Add("M", 1000000);
        powerConversion.Add("G", 1000000000);
    }

    public void refresh()
    {
        _GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors);
        _GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);
        _GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
        _GridTerminalSystem.GetBlocksOfType<IMySmallMissileLauncherReload>(reloadableRockets);
        _GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(gatlings);
    }

    public float powerUsed()
    {
        float totalOutput = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if (reactors[i].DetailedInfo.Contains("Current Output"))
            {
                var iString = reactors[i].DetailedInfo.Substring(reactors[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("Current Output"))
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return totalOutput;
    }

    public float powerAvailable()
    {
        float availableOutput = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if ((reactors[i].DetailedInfo.Contains("Current Output")) && reactors[i].IsWorking)
            {
                var iString = reactors[i].DetailedInfo.Substring(reactors[i].DetailedInfo.IndexOf("Max Output") + 12);
                availableOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if ((batteries[i].DetailedInfo.Contains("Current Output")) && batteries[i].IsWorking)
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Max Output") + 12);
                availableOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return availableOutput;
    }

    public float batteryPowerMax()
    {
        float maxPower = 0;
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("Max Stored Power"))
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Max Stored Power") + 18);
                maxPower += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return maxPower;
    }

    public float batteryPowerStored()
    {
        float storedPower = 0;
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("\nStored power"))
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Stored power") + 14);
                storedPower += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return storedPower;
    }

    public float getShipInv(string itemType)
    {
        List<IMyTerminalBlock> shipInv = new List<IMyTerminalBlock>();
        List<IMyInventoryItem> itemsList = new List<IMyInventoryItem>();
        _GridTerminalSystem.GetBlocks(shipInv);
        float numItems = 0;
        string invstr = "";

        for (int i = 0; i < shipInv.Count; i++)
        {
            var inv = shipInv[i].GetInventory(0);
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
                numItems += float.Parse(itemsList[i].Amount.ToString());
            }
        }

        return numItems;
    }

    public float getShipInvbyName(string invName, string itemType)
    {
        List<IMyTerminalBlock> shipInv = new List<IMyTerminalBlock>();
        List<IMyInventoryItem> itemsList = new List<IMyInventoryItem>();
        _GridTerminalSystem.SearchBlocksOfName(invName, shipInv);
        float numItems = 0;
        string invstr = "";

        for (int i = 0; i < shipInv.Count; i++)
        {
            var inv = shipInv[i].GetInventory(0);
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
                numItems += float.Parse(itemsList[i].Amount.ToString());
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

    public string returnFormattedPower(float power)
    {
        if (power > this.powerConversion["G"])
        {
            return (power / this.powerConversion["G"]).ToString() + " GW";
        }
        else if (power > this.powerConversion["M"])
        {
            return (power / this.powerConversion["M"]).ToString() + " MW";
        }
        else if (power > this.powerConversion["k"])
        {
            return (power / this.powerConversion["k"]).ToString() + " kW";
        }
        else
        {
            return power.ToString() + " W";
        }
    }

    public double secondsToDisplace(double displacement, double velocity = 0, double acceleration = 0)
    {
        double seconds = 0;
        if(acceleration != 0)
        {
            seconds = (Math.Sqrt(2.0 * acceleration * displacement + Math.Pow(velocity, 2.0)) - velocity) / acceleration;
        }
        else if(velocity != 0)
        {
            seconds = displacement / velocity;
        }
        else
        {
            seconds = displacement == 0 ? 0 : long.MaxValue;
        }

        return seconds;
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

    public string renderPercentageBar(float percentage, int xLength, bool invertBar = false)
    {
        string barStart = "[ ";
        string barEnd = " ]";
        string barFill = "|";
        string barEmpty = "'";

        string output = barStart;

        float barFillNum = ((percentage) * (xLength - barStart.Length - barEnd.Length)) / barFill.Length;
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