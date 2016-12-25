// Avionics Firmware Suite
// Orbital Dynamics
// Modded Version

public static string LCDIdentifier = "[LCD]";
public static string debugIdentifier = "[DEBUG]";
public static string controllerIdentifier = "[MAIN CONTROL]";
public static bool debugOn = true;

public static bool firstRun = true;
public static navigation navigationService;
public static double dLastRun;

void Main()
{
    dLastRun = Runtime.TimeSinceLastRun.TotalSeconds;
    grid.initialize(GridTerminalSystem);
    grid.LCD.executeInstructions();

}

public static class grid
{
    public static IMyGridTerminalSystem _GridTerminalSystem;
    public static display LCD;
    public static state ship;
    public static void initialize(IMyGridTerminalSystem GridTerminalSystem)
    {
        _GridTerminalSystem = GridTerminalSystem;
        LCD = new display();
        ship = new state();
        grid.LCD.debugWrite("DEBUG START", false);
        if (firstRun == true)
        {
            navigationService = new navigation();
            firstRun = false;
        }
        else
        {
            navigationService.refresh();
        }
    }
}

public class executor
{
    public int xLength = 80;
    public int yLength = 50;
    public bool invertBar = false;
    public string prefix = "";
    public string br = "\n";
    public string brDelimiter = "<br>";
    public string instructionSet = "";
    public char instructionDelimiter = ';';
    public char argumentDelimiter = ':';
    public char subArgumentDelimiter = ',';
    public string output = "";

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
                    grid.LCD.debugWrite(operation[0], true);
                    this.prefix = operation[1];
                    break;

                case "nl":
                    output += br + prefix;
                    break;

                case "invertBar":
                    grid.LCD.debugWrite(operation[0], true);
                    this.invertBar = Convert.ToBoolean(operation[1]);
                    break;

                case "viewportSize":
                    {
                        grid.LCD.debugWrite(operation[0], true);
                        string[] args = operation[1].Split(subArgumentDelimiter);
                        this.xLength = int.Parse(args[0]);
                        this.yLength = int.Parse(args[1]);
                        grid.LCD.debugWrite(this.xLength.ToString() + "," + this.yLength.ToString(), true);
                    }
                    break;

                case "echo":
                    {
                        grid.LCD.debugWrite(operation[0], true);
                        for (int j = 1; j < operation.Length; j++)
                        {
                            output += operation[j];
                        }
                    }
                    break;

                case "totalPowerUsed":
                    {
                        grid.LCD.debugWrite(operation[0], true);
                        float powerUsed = grid.ship.powerUsed();
                        float percentage = powerUsed / grid.ship.powerAvailable();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        for (int j = 0; j < args.Length; j++)
                        {
                            switch (args[j])
                            {
                                case "displayPercentageBar":
                                    output += grid.LCD.renderPercentageBar(percentage, xLength, invertBar);
                                    break;

                                case "displayPercentage":
                                    output += formatNumberPercentage(percentage * 100);
                                    break;

                                case "displayPowerUsed":
                                    output += grid.ship.returnFormattedPower(powerUsed);
                                    break;

                                default:
                                    output += "UNKNOWN OPTION: " + args[j];
                                    break;
                            }
                        }
                    }
                    break;

                case "totalPowerAvailable":
                    grid.LCD.debugWrite(operation[0], true);
                    output += grid.ship.returnFormattedPower(grid.ship.powerAvailable());
                    break;

                case "totalBatteryStored":
                    {
                        grid.LCD.debugWrite(operation[0], true);
                        float batteryStored = grid.ship.batteryPowerStored();
                        float percentage = batteryStored / grid.ship.batteryPowerMax();

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        for (int j = 0; j < args.Length; j++)
                        {
                            switch (args[j])
                            {
                                case "displayPercentageBar":
                                    output += grid.LCD.renderPercentageBar(percentage, xLength, invertBar);
                                    break;

                                case "displayPercentage":
                                    output += formatNumberPercentage(percentage * 100);
                                    break;

                                case "displayBatteryStored":
                                    output += grid.ship.returnFormattedPower(batteryStored) + "h";
                                    break;

                                default:
                                    output += "UNKNOWN OPTION: " + args[j];
                                    break;
                            }
                        }
                    }
                    break;

                case "totalBatteryMax":
                    grid.LCD.debugWrite(operation[0], true);
                    output += grid.ship.returnFormattedPower(grid.ship.batteryPowerMax()) + "h";
                    break;

                case "getShipInv":
                    {
                        grid.LCD.debugWrite(operation[0], true);

                        string[] args = operation[1].Split(subArgumentDelimiter);
                        for (int j = 0; j < args.Length; j++)
                        {
                            float numItems = grid.ship.getShipInv(args[j++]);

                            switch (args[j])
                            {
                                case "displayPercentageBar":
                                    output += grid.LCD.renderPercentageBar(numItems / float.Parse(args[++j]), xLength, invertBar);
                                    break;

                                case "displayPercentage":
                                    output += formatNumberPercentage(numItems / float.Parse(args[++j]) * 100);
                                    break;

                                case "displayNumItems":
                                    output += numItems;
                                    break;

                                default:
                                    output += "UNKNOWN OPTION: " + args[j];
                                    break;
                            }
                        }
                    }
                    break;

                case "getShipInvByConName":
                    {
                        grid.LCD.debugWrite(operation[0], true);
                        string[] args = operation[1].Split(subArgumentDelimiter);
                        for (int j = 0; j < args.Length; j++)
                        {
                            float numItems = grid.ship.getShipInvbyName(args[j++], args[j++]);

                            switch (args[j])
                            {
                                case "displayPercentageBar":
                                    output += grid.LCD.renderPercentageBar(numItems / float.Parse(args[++j]), xLength, invertBar);
                                    break;

                                case "displayPercentage":
                                    output += formatNumberPercentage(numItems / float.Parse(args[++j]) * 100);
                                    break;

                                case "displayNumItems":
                                    output += numItems.ToString();
                                    break;

                                default:
                                    output += "UNKNOWN OPTION: " + args[j];
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    grid.LCD.debugWrite("Unknown Command: " + operation[0], true);
                    break;
            }
        }
        return output;
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
        grid._GridTerminalSystem.SearchBlocksOfName(controllerIdentifier, controller);
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
            currentPosition = new position(mainControl);
        }
    }
}

public class position
{
    public double timeSinceLast;
    public Vector3D currentPosition;
    public double velocityMagnitude;
    public Vector3D linearVelocity;
    public Vector3D angularVelocity;
    public Vector3D gravityVector;
    public double gravityMagnitude;
    public Vector3D forwardVector;
    public Vector3D downVector;
    public double altitude;

    public position(IMyShipController reference)
    {
        MyShipVelocities shipVelocity = reference.GetShipVelocities();
        MyShipMass shipMass = reference.CalculateShipMass();

        timeSinceLast = dLastRun;
        currentPosition = reference.GetPosition();
        velocityMagnitude = reference.GetShipSpeed();
        linearVelocity = shipVelocity.LinearVelocity;
        angularVelocity = shipVelocity.AngularVelocity;
        gravityVector = reference.GetNaturalGravity();
        gravityMagnitude = gravityVector.Length();
        forwardVector = reference.WorldMatrix.Forward;
        downVector = reference.WorldMatrix.Down;
        if (gravityMagnitude > 0)
        {
            reference.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude);

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

    public state()
    {
        //Instantiate Variables
        grid._GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors);
        grid._GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);
        grid._GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
        grid._GridTerminalSystem.GetBlocksOfType<IMySmallMissileLauncherReload>(reloadableRockets);
        grid._GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(gatlings);

        powerConversion.Add("W", 1);
        powerConversion.Add("k", 1000);
        powerConversion.Add("M", 1000000);
        powerConversion.Add("G", 1000000000);
    }

    public float powerUsed()
    {
        float totalOutput = 0;
        for (int i = 0; i < reactors.Count; i++)
        {
            if (reactors[i].DetailedInfo.Contains("Current Output"))
            {
                var iString = reactors[i].DetailedInfo.Substring(reactors[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].DetailedInfo.Contains("Current Output"))
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Current Output") + 16);
                totalOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
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
                availableOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        for (int i = 0; i < batteries.Count; i++)
        {
            if ((batteries[i].DetailedInfo.Contains("Current Output")) && batteries[i].IsWorking)
            {
                var iString = batteries[i].DetailedInfo.Substring(batteries[i].DetailedInfo.IndexOf("Max Output") + 12);
                availableOutput += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
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
                maxPower += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
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
                storedPower += float.Parse(iString.Substring(0, iString.IndexOf(" "))) * (grid.ship.powerConversion.ContainsKey(iString.Substring(iString.IndexOf(" ") + 1, 1)) ? grid.ship.powerConversion[iString.Substring(iString.IndexOf(" ") + 1, 1)] : 1);
            }
        }
        return storedPower;
    }

    public float getShipInv(string itemType)
    {
        List<IMyTerminalBlock> shipInv = new List<IMyTerminalBlock>();
        List<IMyInventoryItem> itemsList = new List<IMyInventoryItem>();
        grid._GridTerminalSystem.GetBlocks(shipInv);
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
            //grid.LCD.debugWrite(itemsList[i].Content.SubtypeName, true);
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
        grid._GridTerminalSystem.SearchBlocksOfName(invName, shipInv);
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
            grid.LCD.debugWrite(itemsList[i].Content.SubtypeName.ToString(), true);
            grid.LCD.debugWrite(itemsList[i].Amount.ToString(), true);
            invstr = itemsList[i].Content.SubtypeName;
            if (itemType == itemsList[i].Content.SubtypeName)
            {
                numItems += float.Parse(itemsList[i].Amount.ToString());
            }
        }

        return numItems;
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
}

public class display
{
    List<IMyTerminalBlock> debugPanels = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> displayPanels = new List<IMyTerminalBlock>();

    public display()
    {
        if (debugOn)
        {
            grid._GridTerminalSystem.SearchBlocksOfName(debugIdentifier, debugPanels);
        }
        grid._GridTerminalSystem.SearchBlocksOfName(LCDIdentifier, displayPanels);
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
            grid.LCD.debugWrite("Executing Panel " + i, true);
            IMyTextPanel panel = (IMyTextPanel)displayPanels[i];
            string customData = panel.CustomData;
            executor thisDisplay = new executor();
            panel.WritePublicText(thisDisplay.executeInstruction(customData), false);

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
            if (((i < barFillNum + barStart.Length + barEnd.Length - 2) && (invertBar == false)) || ((i > Math.Abs(barFillNum + barStart.Length + barEnd.Length - 2 - xLength)) && (invertBar == true)))
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