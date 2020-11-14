using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using FinchAPI;
using System.Diagnostics;
using System.Linq;

namespace Project_FinchControl
{

    // **************************************************
    //
    // Title: Finch Control - Menu Starter
    // Description: Starter solution with the helper methods,
    //              opening and closing screens, and the menu
    // Application Type: Console
    // Author: Velis, John
    // Author: Thayer, Shaun
    // Dated Created: 1/22/2020
    // Last Modified: 11/13/2020
    //
    // **************************************************
    public enum Command
    {
        NONE,
        MOVEBACKWARD,
        MOVEFORWARD,
        STOPMOTOR,
        WAIT,
        TURNRIGHT,
        TURNLEFT,
        LEDON,
        LEDOFF,
        GETTEMPERATURE,
        DONE,
    }
    class Program
    {
        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SetTheme();

            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        /// <summary>
        /// setup the console theme
        /// </summary>
        static void SetTheme()
        {
            Console.WindowWidth = 120;
            Console.WindowHeight = 25;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
        }

        #region MAIN MENU
        /// <summary>
        /// *****************************************************************
        /// *                     Main Menu                                 *
        /// *****************************************************************
        /// </summary>
        static void DisplayMenuScreen()
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool quitApplication = false;
            string menuChoice;

            Finch myFinch = new Finch();

            do
            {
                DisplayScreenHeader("Main Menu");
                Console.WriteLine("\t\t_________________________\n");
                //
                // get user menu choice
                //
                Console.WriteLine("\t\ta) Connect Finch Robot");
                Console.WriteLine("\t\tb) Talent Show");
                Console.WriteLine("\t\tc) Data Recorder");
                Console.WriteLine("\t\td) Alarm System");
                Console.WriteLine("\t\te) User Programming");
                Console.WriteLine("\t\tf) Disconnect Finch Robot");
                Console.WriteLine("\t\tq) Quit");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        DisplayConnectFinchRobot(myFinch);
                        break;

                    case "b":
                        DisplayTalentShowMenuScreen(myFinch);
                        break;

                    case "c":
                        DisplayDataRecorderMenuScreen(myFinch);
                        break;

                    case "d":
                        DisplayAlarmSystemMenuScreen(myFinch);
                        break;

                    case "e":
                        DisplayUserProgramingMenu(myFinch);
                        break;

                    case "f":
                        DisplayDisconnectFinchRobot(myFinch);
                        break;

                    case "q":
                        DisplayDisconnectFinchRobot(myFinch);
                        quitApplication = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);
        }
        #region USER PROGRAMING

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myFinch"></param>
        private static void DisplayUserProgramingMenu(Finch myFinch)
        {
            Console.WriteLine("User Programing Menu");

            bool quitMenu = false;
            string menuChoice;

            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters;
            commandParameters = (0, 0, 0);

            List<Command> commands = new List<Command>();

            Console.WriteLine("\tA: Set Command Parameters");
            Console.WriteLine("\tB: Add Commands");
            Console.WriteLine("\tC: View Commands");
            Console.WriteLine("\tD: Execute Commands");
            Console.WriteLine("\tQ: Quit Menu");

            menuChoice = Console.ReadLine();
            switch (menuChoice)
            {
                case "a":
                    commandParameters = DisplaySetCommandParameters();
                    break;

                case "b":
                    DisplayGetCommands(commands);
                    break;

                case "c":
                    DisplayViewCommands(commands);
                    break;

                case "D":
                    DisplayExecuteCommands(myFinch, commands, commandParameters);
                    break;

                case "F":
                    quitMenu = true;
                    break;

                default:
                    break;
            }
        }

        private static void DisplayExecuteCommands(Finch myFinch, List<Command> commands,
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters)
        {
            // to do list: validate user input

            DisplayScreenHeader("Execute commands");

            Console.WriteLine("The Finch robot is to Execute the commands");
            DisplayContinuePrompt();

            foreach (Command command in commands)
            {
                switch (command)
                {
                    case Command.NONE:
                        Console.WriteLine("Invalid Command");
                        break;
                    case Command.MOVEBACKWARD:
                        myFinch.setMotors(commandParameters.motorSpeed, commandParameters.motorSpeed);
                        break;
                    case Command.MOVEFORWARD:
                        myFinch.setMotors(-commandParameters.motorSpeed, -commandParameters.motorSpeed);
                        break;
                    case Command.STOPMOTOR:
                        myFinch.setMotors(0, 0);
                        break;
                    case Command.WAIT:
                        myFinch.wait((int)commandParameters.waitSeconds * 1000);
                        break;
                    case Command.TURNRIGHT:
                        myFinch.setMotors(commandParameters.motorSpeed, -commandParameters.motorSpeed);
                        break;
                    case Command.TURNLEFT:
                        myFinch.setMotors(-commandParameters.motorSpeed, commandParameters.motorSpeed);
                        break;
                    case Command.LEDON:
                        myFinch.setLED(commandParameters.ledBrightness, commandParameters.ledBrightness, commandParameters.ledBrightness);
                        break;
                    case Command.LEDOFF:
                        myFinch.setLED(0, 0, 0);
                        break;
                    case Command.GETTEMPERATURE:
                        double temperature = myFinch.getTemperature();
                        Console.WriteLine(temperature);
                        break;
                    case Command.DONE:

                        break;
                    default:
                        break;
                }
            }

            DisplayMenuPrompt("To User Programing");
        }

        private static void DisplayViewCommands(List<Command> commands)
        {
            DisplayScreenHeader("User Programing");

            foreach (Command command in commands)
            {
                Console.WriteLine("\t\t" + command);
            }
        }

        private static object DisplayGetCommands(List<Command> commands)
        {
            DisplayScreenHeader("Enter Commands");

            Command command;
            bool validResponse;
            bool isDoneAddingCommands = false;
            string userResponse;

            foreach (Command commandName in Enum.GetValues(typeof(Command)))
            {
                if (commandName.ToString() != "NONE")
                {
                    Console.WriteLine("\t\t" + commandName);
                }
            }

            do
            {

                validResponse = true;

                Console.WriteLine($"Enter Command");
                userResponse = Console.ReadLine().ToUpper();

                if (userResponse != "done")
                {
                    if (!Enum.TryParse(Console.ReadLine(), out command))
                    {
                        Console.WriteLine("\n\tPlease enter a proper command: ");
                        Console.ReadLine();
                        DisplayContinuePrompt();
                        validResponse = false;
                    }
                    else
                    {
                        commands.Add(command);
                    }
                }
                else
                {
                    isDoneAddingCommands = true;
                }
            } while (!validResponse || !isDoneAddingCommands);

            DisplayMenuPrompt("To User Programing Menu");

            return commands;
        }

        private static (int motorSpeed, int ledBrightness, double waitSeconds) DisplaySetCommandParameters()
        {
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters;

            commandParameters = (0, 0, 0);

            DisplayScreenHeader("Command Parameters");

            Console.Write("Enter Motor Speed:");
            int.TryParse(Console.ReadLine(), out commandParameters.motorSpeed);

            Console.Write("Enter Led Brightness:");
            int.TryParse(Console.ReadLine(), out commandParameters.ledBrightness);

            Console.Write("Enter Wait in Seconds:");
            double.TryParse(Console.ReadLine(), out commandParameters.waitSeconds);


            return commandParameters;
        }

        #endregion

        #region ALARM SYSTEM
        /// <summary>
        /// *****************************************************************
        /// *                     Alarm System Menu                          *
        /// *****************************************************************
        /// </summary>
        
        static void DisplayAlarmSystemMenuScreen(Finch myfinch)
        {            
            bool quitAlarmSystemMenu = false;
            string menuChoice;

            string[] sensorsToMonitor = null;

            int[] LightSenorsMinMaxThresholdValues = new int[4];            
            int[] TemperatureMinMaxThresholdValues = new int[2];
            LightSenorsMinMaxThresholdValues[0] = 0;
            LightSenorsMinMaxThresholdValues[1] = 0;
            LightSenorsMinMaxThresholdValues[2] = 0;
            LightSenorsMinMaxThresholdValues[3] = 0;
            TemperatureMinMaxThresholdValues[0] = 0;
            TemperatureMinMaxThresholdValues[1] = 0;
            int timeToMonitor = 0;            

            do
            {
                DisplayScreenHeader         ("Alarm System Menu");
                Console.WriteLine("\t\t_______________________________\n");
                Console.WriteLine("\t\ta> Sensors to Monitor");
                Console.WriteLine("\t\tb> Light Sensor Threshold");
                Console.WriteLine("\t\tc> Temperature Sensor Threshold");
                Console.WriteLine("\t\td> Time to Monitor");
                Console.WriteLine("\t\te> Set Alarm System");
                Console.WriteLine("\t\tq> Quit Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                
                Console.CursorVisible = true;

                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        sensorsToMonitor = AlarmSystemSensorsToMonitor();
                        break;

                    case "b":
                        LightSenorsMinMaxThresholdValues = AlarmSystemLightSensorsThreshold(myfinch);
                        break;

                    case "c":
                         TemperatureMinMaxThresholdValues = AlarmSystemTemperatureThreshold(myfinch);
                        break;
                        
                    case "d":
                        timeToMonitor = AlarmSystemTimeToMonitor();
                        break;
                    case "e":
                        if (sensorsToMonitor == null || (TemperatureMinMaxThresholdValues == null && sensorsToMonitor[1] == "yes")
                            || (LightSenorsMinMaxThresholdValues == null && sensorsToMonitor[0] != "no") || timeToMonitor == 0)
                        {

                            Console.WriteLine("\n\tPlease enter all required values.");

                            DisplayContinuePrompt();
                        }
                        else
                        {
                            AlarmSystemSetAlarms(myfinch, TemperatureMinMaxThresholdValues, sensorsToMonitor, LightSenorsMinMaxThresholdValues, timeToMonitor);
                        }
                        break;

                    case "q":
                        quitAlarmSystemMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitAlarmSystemMenu);

        }
        /// <summary>
        /// Alarm System Set Alarms \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <param name="TemperatureMinMaxThresholdValues"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="LightSenorsMinMaxThresholdValues"></param>
        /// <param name="timeToMonitor"></param>
        static void AlarmSystemSetAlarms(Finch myfinch, int[] TemperatureMinMaxThresholdValues, string[] sensorsToMonitor, int[] LightSenorsMinMaxThresholdValues, int timeToMonitor)
        {                   
            int seconds = 1;            

            DisplayScreenHeader("Set the Alarm System");

            Console.CursorVisible = false;

            Console.WriteLine($"\n\n\tSensors to Monitor: " +
                $"\n\n\tLight Sensor: {sensorsToMonitor[0]}" +
                $"\n\n\tTemperature Sensor: {sensorsToMonitor[1]}" +
                $"\n\n\tObstacle Sensor: {sensorsToMonitor[2]}");
            Console.WriteLine("\n\tCurrent Thresholds");
            Console.WriteLine($"\n\tMin/Max Threshhold Values for Light Sensors:" +
                $"\n\tLeft Minimum: {LightSenorsMinMaxThresholdValues[0]} Left Maximum: {LightSenorsMinMaxThresholdValues[1]}" +
                $"\n\tRight Minimum: {LightSenorsMinMaxThresholdValues[2]} Right Maximum: {LightSenorsMinMaxThresholdValues[3]}");
            Console.WriteLine($"\n\tMin/Max Threshhold Values for Temp Sensors: Minimum: {TemperatureMinMaxThresholdValues[0]} Maximum: {TemperatureMinMaxThresholdValues[1]}");
            Console.WriteLine($"\n\tTime for Alarm system to Monitor in seconds: {timeToMonitor / 1000}");
            Console.WriteLine("\n\t");

            Console.WriteLine("$\n\tPress any key to set the alarm");
            Console.CursorVisible = false;
            Console.ReadKey();
            Console.CursorVisible = true;

            bool thresholdExceeded;

            Console.Clear();

            do
            {
                thresholdExceeded = false;

                // Get Current Senors Reading \\

                thresholdExceeded = AlarmSystemLightSensorThresholdExceeded(myfinch, sensorsToMonitor, LightSenorsMinMaxThresholdValues) ||
                    AlarmSystemTemperaturesThreshHoldExceeded(TemperatureMinMaxThresholdValues, sensorsToMonitor, myfinch) ||
                    AlarmSystemObjectDetected(sensorsToMonitor, myfinch);

                if (thresholdExceeded == false)
                {
                    seconds++;
                    Console.SetCursorPosition(10, 10);
                    Console.WriteLine($"\t\tTime: {seconds}");
                    myfinch.setLED(0, 255, 0);
                    myfinch.wait(1000);
                    myfinch.setLED(0, 0, 0);
                    GetCurrentSenorsReading(myfinch);
                    seconds++;

                }
                else
                {
                    thresholdExceeded = true;
                }
            } while (thresholdExceeded == false && seconds < timeToMonitor);

            if (thresholdExceeded == true)
            {
                Console.Clear();

                Console.WriteLine("\n\tThreshold Exceeded!\n");
                myfinch.noteOn(1200);
                myfinch.setLED(255, 0, 0);

                Console.WriteLine($"\n\tMin/Max Threshhold Values for Light Sensors:" +
                $"\n\tLeft Minimum: {LightSenorsMinMaxThresholdValues[0]} Left Maximum: {LightSenorsMinMaxThresholdValues[1]}" +
                $"\n\tRight Minimum: {LightSenorsMinMaxThresholdValues[2]} Right Maximum: {LightSenorsMinMaxThresholdValues[3]}");
                Console.WriteLine($"\n\tMin/Max Threshhold Values for Temp Sensors: Minimum: {TemperatureMinMaxThresholdValues[0]} Maximum: {TemperatureMinMaxThresholdValues[1]}");
                Console.WriteLine($"\n\tTime for Alarm system to Monitor in seconds: {timeToMonitor}");

                GetCurrentSenorsReading(myfinch);

                DisplayContinuePrompt();
                myfinch.noteOff();
                myfinch.setLED(0,0,0);

            }
            else
            {
                Console.WriteLine("\n\tAlarm timed out before Threshold was Exceeded");
            }


            DisplayMenuPrompt("Alarm system");
        }

        /// <summary>
        /// GetCurrentSenorsReading \\\
        /// </summary>
        /// <param name="myfinch"></param>
        static void GetCurrentSenorsReading(Finch myfinch)
        {
            Console.CursorVisible = false;
            bool[] currentobstacleReading = myfinch.getObstacleSensors();


            Console.WriteLine($"\n\tCurrent Light Sensor Reading: Left: {myfinch.getLeftLightSensor()} Right: {myfinch.getRightLightSensor()}");
            Console.WriteLine($"\n\tCurrent Temperature Sensor Reading: {myfinch.getTemperature()} Celcius");
            Console.WriteLine($"\n\tCurrent object Sensor Reading: Left {currentobstacleReading[0]} Right {currentobstacleReading[1]}");
        }

        /// <summary>
        /// Alarm System Object Detected \\\
        /// </summary>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static bool AlarmSystemObjectDetected(string[] sensorsToMonitor, Finch myfinch)
        {            
            bool[] getObstacleReadingArray = new bool[2];
            bool thresholdExceeded = false;                        
            
            getObstacleReadingArray[0] = myfinch.isObstacleLeftSide();
            getObstacleReadingArray[1] = myfinch.isObstacleRightSide();
            
            switch (sensorsToMonitor[2])
            {
                case "yes":
                    if (getObstacleReadingArray[0] == true || getObstacleReadingArray[1] == true)
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;

        }

        /// <summary>
        /// Alarm System Temperatures Thresh Hold Exceeded \\\
        /// </summary>
        /// <param name="TemperatureMinMaxThresholdValues"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static bool AlarmSystemTemperaturesThreshHoldExceeded(int[] TemperatureMinMaxThresholdValues, string[] sensorsToMonitor, Finch myfinch)
        {
            double currentTemperatureReading = myfinch.getTemperature();

            bool thresholdExceeded = false;

            switch (sensorsToMonitor[1])
            {
                case "yes":
                    if (currentTemperatureReading < TemperatureMinMaxThresholdValues[0]  || currentTemperatureReading > TemperatureMinMaxThresholdValues[1])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;

        }

        /// <summary>
        /// Alarm System Light sensor Threshold Exceeded \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <param name="sensorsToMonitor"></param>
        /// <param name="LightSenorsMinMaxThresholdValues"></param>
        /// <returns></returns>
        static bool AlarmSystemLightSensorThresholdExceeded(Finch myfinch, string[] sensorsToMonitor, int[] LightSenorsMinMaxThresholdValues)
        {
            int currentLeftLightSensorValue = myfinch.getLeftLightSensor();
            int currentRightLightSensorValue = myfinch.getRightLightSensor();

            bool thresholdExceeded;

            // int[] LightSenorsMinMaxThresholdValues
            // ******** Array Index ********
            // [0] = minThresholdValuesLeft 
            // [1] = maxThresholdValuesLeft 
            // [2] = minThresholdValuesRight
            // [3] = maxThresholdValuesRight

            switch (sensorsToMonitor[0])
            {
                case "left":
                    if (currentLeftLightSensorValue < LightSenorsMinMaxThresholdValues[0] || currentLeftLightSensorValue > LightSenorsMinMaxThresholdValues[1])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;
                case "right":
                    if (currentRightLightSensorValue < LightSenorsMinMaxThresholdValues[2] || currentRightLightSensorValue > LightSenorsMinMaxThresholdValues[3])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;

                case "both":
                    if (currentLeftLightSensorValue < LightSenorsMinMaxThresholdValues[0] || currentRightLightSensorValue < LightSenorsMinMaxThresholdValues[2] || currentLeftLightSensorValue > LightSenorsMinMaxThresholdValues[1] || currentRightLightSensorValue > LightSenorsMinMaxThresholdValues[3])
                    {
                        thresholdExceeded = true;
                    }
                    else
                    {
                        thresholdExceeded = false;
                    }
                    break;
                case "no":
                    thresholdExceeded = false;
                    break;

                default:
                    thresholdExceeded = false;
                    break;
            }

            return thresholdExceeded;
        }

        /// <summary>
        /// Alarm System Time To Monitor \\\
        /// </summary>
        /// <returns></returns>
        static int AlarmSystemTimeToMonitor()
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool validResponse;

            DisplayScreenHeader("Time to Monitor");

            int timeToMonitor;
            do
            {
                validResponse = true;

                Console.Write("\n\tLength of time for Alarm To Monitor in Seconds: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out timeToMonitor))
                {
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm system");

                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            

            return timeToMonitor;

        }

        /// <summary>
        /// Alarm System Light Sensors Threshold \\\
        /// </summary>
        /// <param name="myfinch"></param>
        /// <returns></returns>
        static int[] AlarmSystemLightSensorsThreshold(Finch myfinch)
        {
            Console.Clear();
            Console.CursorVisible = true;

            int[] LightSenorsMinMaxThresholdValues = new int[4];

            // Array Index
            // [0] = minThresholdValuesLeft 
            // [1] = maxThresholdValuesLeft 
            // [2] = minThresholdValuesRight
            // [3] = maxThresholdValuesRight

            Console.WriteLine($"\n\tCurrent values for both left and right light sensor:" +
                $"\n\tLeft: {myfinch.getLeftLightSensor()} Right: {myfinch.getRightLightSensor()}");
            bool validResponse;

            do
            {
                validResponse = true;

                Console.Write($"\n\tEnter Minimum Threshold for [Left] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int minThresholdValuesLeft))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[0] = minThresholdValuesLeft;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.Write($"\n\tEnter Maximum Threshold for [Left] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int maxThresholdValuesLeft))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[1] = maxThresholdValuesLeft;
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.Write($"\n\tEnter Minimum Threshold for [Right] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int minThresholdValuesRight))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[2] = minThresholdValuesRight;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.Write($"\n\tEnter Minimum Threshold for [Right] light sensors: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int maxThresholdValuesRight))
                {
                    Console.CursorVisible = false;
                    LightSenorsMinMaxThresholdValues[3] = maxThresholdValuesRight;
                    DisplayMenuPrompt("Alarm system");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            return LightSenorsMinMaxThresholdValues;
        }

        /// <summary>
        /// AlarmSystem Temperature Threshold \\\
        /// </summary>
        /// <returns></returns>
        static int[] AlarmSystemTemperatureThreshold(Finch myfinch)
        {
            Console.Clear();
            Console.CursorVisible = true;

            int[] TemperatureMinMaxThresholdValues = new int[2];
            bool validResponse;

            Console.WriteLine($"\n\tCurrent Temperature Value:" +
                $"{myfinch.getTemperature()} Celcius.");
            do
            {
                validResponse = true;

                Console.Write($"\n\tPlease Enter Mininum Temperature Threshold: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out TemperatureMinMaxThresholdValues[0]))
                {
                    Console.CursorVisible = false;                    
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.Write($"\n\tPlease Enter Maximum Temperature Threshold: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out TemperatureMinMaxThresholdValues[1]))
                {
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm system");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a valid numerical value");
                }
            } while (!validResponse);

            return TemperatureMinMaxThresholdValues;
        }
                
        /// <summary>
        /// Alarm system sensors to monitor \\\
        /// </summary>
        /// <returns>The system sensors to monitor.</returns>
        static string[] AlarmSystemSensorsToMonitor()
        {
            Console.Clear();
            Console.CursorVisible = true;

            bool validResponse;
            string[] sensorsToMonitor = new string[3];

            Console.WriteLine("\n\tSensors to Monitor\n");

            do
            {
                validResponse = true;              
                                
                Console.Write("\n\tDo you want to Monitor the Light Sensors? [left, right, both, no]: ");
                string lightSensorsToMonitor = Console.ReadLine().ToLower();

                if (lightSensorsToMonitor == "left" || lightSensorsToMonitor == "right" || lightSensorsToMonitor == "both" || lightSensorsToMonitor == "no")
                {
                    sensorsToMonitor[0] = lightSensorsToMonitor;
                }                
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");
                }
            } while (!validResponse);

            do
            {
                validResponse = true;
                
                Console.Write("\n\tDo you want to Monitor the Temperature? [yes, no]: ");
                string temperaturesToMonitor = Console.ReadLine().ToLower();

                if (temperaturesToMonitor == "yes" || temperaturesToMonitor == "no")
                {
                    sensorsToMonitor[1] = temperaturesToMonitor;
                }               
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");                    
                }
            } while (!validResponse);

            do
            {
                validResponse = true;

                Console.Write("\n\tDo you want to Monitor the Obstacle sensor? [yes, no]: ");
                string obstacleToMonitor = Console.ReadLine().ToLower();

                if (obstacleToMonitor == "yes" || obstacleToMonitor == "no")
                {
                    sensorsToMonitor[2] = obstacleToMonitor;
                    Console.CursorVisible = false;
                    DisplayMenuPrompt("Alarm System");
                }
                else
                {
                    validResponse = false;
                    Console.CursorVisible = false;
                    Console.WriteLine("\n\tPlease enter a correct response");                    
                }
            } while (!validResponse);
            
            return sensorsToMonitor;
        }
        #endregion

        #region DATA RECORDING
        /// <summary>
        /// *****************************************************************
        /// *                     Data Recording Menu                       *
        /// *****************************************************************
        /// </summary>

        static void DisplayDataRecorderMenuScreen(Finch myFinch)
        {
            Console.CursorVisible = true;

            bool quitDataRecorderMenu = false;

            List<double[]> RowListOfArrays;

            int numberOfDataPoints = 0;
            int frequencyOfData = 0;
            int numberOfRows = 0;

            string menuChoice;

            double[][] temperaturesPerRow = null;
            
            do
            {
                DisplayScreenHeader("Data Sensor Grid Menu");
                Console.WriteLine("\t\t_________________________\n");
                //
                // get user menu choice
                //
                Console.WriteLine("\t\ta) Get the number of Data Points for each row of your Data Grid");
                Console.WriteLine("\t\tb) Get the timing between Data Points");
                Console.WriteLine("\t\tc) Get the number of Rows for your grid");
                Console.WriteLine("\t\td) Fixed Sized Sensor Grid Array");
                Console.WriteLine("\t\te) Non Fixed Sized Sensor Grid Array");
                Console.WriteLine("\t\tq) Main Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();
                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        numberOfDataPoints = GetTheNumberOfDataPoints();
                        break;

                    case "b":
                        frequencyOfData = FrequencyOfDataPoints();
                        break;

                    case "c":
                        numberOfRows = NumberOfRows();
                        break;

                    case "d":
                        if (numberOfDataPoints == 0 || frequencyOfData == 0)
                        {
                            Console.WriteLine("Please add values to the Menu options (a) and (b)");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                        StaticGridArrayDataSet(numberOfDataPoints, frequencyOfData, myFinch);
                        }
                        break;
                    
                    case "e":
                        if (numberOfDataPoints == 0 || frequencyOfData == 0 || numberOfRows == 0)
                        {
                            Console.WriteLine("\n\tPlease add values to the Menu options (a), (b), and (c)");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                            NonStaticSensorGridArray(numberOfRows, numberOfDataPoints, frequencyOfData, temperaturesPerRow, myFinch);
                        }
                        break;

                    case "q":
                        quitDataRecorderMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }                          

            } while (!quitDataRecorderMenu);
        }
        
        /// <summary>
        /// Get The Static Data Set \\\
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static void StaticGridArrayDataSet(int numberOfDataPoints, int frequencyOfData, Finch myFinch)
        {
            List<double[]> RowListOfArrays = new List<double[]>();

            Console.WriteLine("\n\tTime to build your Data Grid" +
                              "\n\t********* Example ********" +
                              "\n\t|-------|-------|-------|");
            Console.WriteLine  ("\t| Row 1 | Row 2 | Row 3 |" +
                              "\n\t|-------|-------|-------|" +
                              "\n\t| Temp1 | Temp1 | Temp1 |" +
                              "\n\t| Temp2 | Temp2 | Temp2 |" +
                              "\n\t| Temp3 | Temp3 | Temp3 |" +
                              "\n\t|-------|-------|-------|");

            Console.WriteLine($"\n\tThe Finch will make a Sensor Reading then move forward for {frequencyOfData}sec");
            Console.WriteLine("\n\tEach time the Finch makes a reading it will light up and make a 1 sec beep");
            Console.WriteLine($"\n\tNumber of Rows: {3}");
            Console.WriteLine($"\n\tNuber of Data Points: {numberOfDataPoints} per row.");
            Console.WriteLine($"\n\tTime between Data Points: {frequencyOfData}");                      

            StaticGridArrayGetDataSet(numberOfDataPoints, frequencyOfData, myFinch);

            DisplayMenuPrompt("Data Recording");                                               
        }

        /// <summary>
        /// Stati Grid Array Get Data Set
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static double[][] StaticGridArrayGetDataSet(int numberOfDataPoints, int frequencyOfData, Finch myFinch)
        {
            double temperatureReading;
                       
            double[][] temperaturesPerRow = new double[3][];
            double[] temperatureReadingArray1 = new double[numberOfDataPoints];
            double[] temperatureReadingArray2 = new double[numberOfDataPoints];
            double[] temperatureReadingArray3 = new double[numberOfDataPoints];

           
            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {1}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray1[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[0] = temperatureReadingArray1;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);

                Console.WriteLine("\n\tTo began next reading");

                DisplayContinuePrompt();
            }

            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {2}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray2[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[1] = temperatureReadingArray2;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);

                Console.WriteLine("\n\tTo began next reading");

                DisplayContinuePrompt();
            }

            Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {3}.");

            DisplayContinuePrompt();

            for (int a = 0; a < numberOfDataPoints; a++)
            {
                int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                temperatureReading = myFinch.getTemperature();
                temperatureReadingArray3[a] = temperatureReading;
                Console.WriteLine($"\n\tTemperature Reading { a + 1}: {temperatureReading}");
                temperaturesPerRow[2] = temperatureReadingArray3;
                myFinch.setLED(255, 255, 255);
                myFinch.noteOn(1000);
                myFinch.wait(1000);
                myFinch.setLED(0, 0, 0);
                myFinch.noteOff();
                myFinch.setMotors(200, 200);
                myFinch.wait(frequencyOfdataPointsinMilSeconds);
                myFinch.setMotors(0, 0);

                Console.WriteLine("\n\tTo began next reading");

                DisplayContinuePrompt();
            }

            Console.WriteLine("\n\tDone Collecting Data");

            Console.WriteLine("\n\tFor per Row Print out");

            DisplayContinuePrompt();

            Console.WriteLine("\n\tRow 1");
            foreach (double temp in temperatureReadingArray1)
            {
                Console.WriteLine($"\n\tTemperature - {temp} ");
            }
            Console.WriteLine("\n\tRow 2");
            foreach (double temp in temperatureReadingArray2)
            {
                Console.WriteLine($"\n\tTemperature - {temp} ");
            }
            Console.WriteLine("\n\tRow 3");
            foreach (double temp in temperatureReadingArray3)
            {
                Console.WriteLine($"\n\tTemperature - {temp} ");
            }

            DisplayMenuPrompt("Data Recording");

            return temperaturesPerRow;
        }
        /// <summary>
        /// FrequencyOfDataPoints
        /// </summary>
        /// <returns></returns>
        static int FrequencyOfDataPoints()
        {
            int frequencyOfData;
            bool validResponse;

            do
            {
                Console.WriteLine("\n\tFrequency of Data points");
                Console.Write("\n\tTime between Data Points in Seconds: ");
                string userResponse = Console.ReadLine();

                validResponse = true;

                if (int.TryParse(userResponse, out frequencyOfData))
                {

                    DisplayMenuPrompt("Data Recording Menu");

                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number: ");
                }
            } while (!validResponse);
            
            return frequencyOfData;
        }

        /// <summary>
        /// GetTheNumberOfDataPoints
        /// </summary>
        static int GetTheNumberOfDataPoints()
        {

            int numberOfDataPoints;
            bool validResponse;

            do
            {
                Console.WriteLine("\n\tGet the number of Data Points");
                Console.Write("\n\tNumber of Data Points needed?: ");
                string userResponse = Console.ReadLine();

                validResponse = true;

                if (int.TryParse(userResponse, out numberOfDataPoints))
                {

                    DisplayMenuPrompt("Data Recording");

                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number: ");

                }
            } while (!validResponse);          

            return numberOfDataPoints;
        }

        /// <summary>
        /// Sensors the grid array.
        /// </summary>
        /// <returns>The grid array.</returns>
        /// <param name="numberOfDataPoints">Number of data points.</param>
        /// <param name="frequencyOfData">Frequence of data points.</param>
        /// <param name="numberOfRows">Frequence of data points.</param>
        /// <param name="myFinch">My finch.</param>
        static List<double[]> NonStaticSensorGridArray(int numberOfRows, int numberOfDataPoints, int frequencyOfData, double[][] temperaturesPerRow, Finch myFinch)
        {
            List<double[]> RowListOfArrays = new List<double[]>();

            Console.WriteLine("\n\tTime to build your Data Grid" +
                            "\n\n\t********* Example ********" +
                            "\n\n\t|-------|-------|-------|");
            Console.WriteLine  ("\t| Row 1 | Row 2 | Row 3 |" +
                              "\n\t|-------|-------|-------|" +
                              "\n\t| Temp1 | Temp1 | Temp1 |" +
                              "\n\t| Temp2 | Temp2 | Temp2 |" +
                              "\n\t| Temp3 | Temp3 | Temp3 |" +
                              "\n\t|-------|-------|-------|");

            Console.WriteLine($"\n\tThe Finch will make a Sensor Reading then move forward for {frequencyOfData}sec");
            Console.WriteLine("\n\tEach time the Finch makes a reading it will light up and make a 1 sec beep");
            Console.WriteLine($"\n\tNumber of Rows: {numberOfRows}, if you were using a none static grid array");
            Console.WriteLine($"\n\tNuber of Data Points: {numberOfDataPoints} per row.");
            Console.WriteLine($"\n\tTime between Data Points: {frequencyOfData}");

            DisplayContinuePrompt();

            NonStaticGridArrayGetDataSet(numberOfRows, numberOfDataPoints, frequencyOfData, myFinch);

            DisplayMenuPrompt("Data Set");

            return RowListOfArrays;
        }

        /// <summary>
        /// Static Grid Array Get Data Set \\\
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="frequencyOfData"></param>
        /// <param name="myFinch"></param>
        /// <returns></returns>
        static double[][] NonStaticGridArrayGetDataSet(int numberOfRows, int numberOfDataPoints, int frequencyOfData, Finch myFinch)
        {
            double temperatureReading;
            int tempRow;

            double[][] temperaturesPerRow = new double[numberOfRows][];
            double[] temperatureReadingArray = new double[numberOfDataPoints];

            for (int i = 0; i < numberOfRows; i++)
            {
                Console.WriteLine($"\n\tPlease place the finch bot at the start of Row {i + 1}.");

                DisplayContinuePrompt();

                for (int a = 0; a < numberOfDataPoints; a++)
                {
                    int frequencyOfdataPointsinMilSeconds = frequencyOfData * 1000;
                    temperatureReading = myFinch.getTemperature();
                    temperatureReadingArray[a] = temperatureReading;                    
                    Console.WriteLine($"Temperature Reading { a + 1}: {temperatureReading}");
                    myFinch.setLED(255, 255, 255);
                    myFinch.noteOn(1000);
                    myFinch.wait(1000);
                    myFinch.setLED(0, 0, 0);
                    myFinch.noteOff();
                    myFinch.setMotors(200, 200);
                    myFinch.wait(frequencyOfdataPointsinMilSeconds);
                    myFinch.setMotors(0, 0);
                                        
                    Console.WriteLine("\n\tTo began next reading");

                    DisplayContinuePrompt();
                }

                temperaturesPerRow[i] = temperatureReadingArray;                
            }
            
            for (tempRow = 0; tempRow < temperaturesPerRow.Length; tempRow++)
            {
                Console.Write($"\n\tRow - {tempRow + 1}", temperaturesPerRow[tempRow]);

                foreach (double temp in temperatureReadingArray)
                {
                    Console.WriteLine($"\n\tTemperature - {temp} ");
                }
            }

            DisplayMenuPrompt("Data Recording");

            return temperaturesPerRow;
        }

        /// <summary>
        /// Number Of Rows in Grid Array \\\
        /// </summary>
        /// <returns></returns>
        static int NumberOfRows()
        {
            int numberOfRows;
            bool validResponse;

            do
            {
                validResponse = true;

                Console.Write("\n\tPlease enter how many rows you need for the Data Grid: ");
                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out numberOfRows))
                {
                    DisplayMenuPrompt("Data Recording");

                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number: ");
                }
            } while (!validResponse);
            
            return numberOfRows;
        }
        #endregion

        #region TALENT SHOW

        /// <summary>
        /// *****************************************************************
        /// *                     Talent Show Menu                          *
        /// *****************************************************************
        /// </summary>
        static void DisplayTalentShowMenuScreen(Finch myFinch)
        {
            Console.CursorVisible = true;

            bool quitTalentShowMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Talent Show Menu");
                Console.WriteLine("\t\t_________________________\n");

                //
                // get user menu choice
                //
                Console.WriteLine("\t\ta) Light and Sound");
                Console.WriteLine("\t\tb) Lets Dance");
                Console.WriteLine("\t\tc) ");
                Console.WriteLine("\t\td) ");
                Console.WriteLine("\t\tq) Main Menu");
                Console.Write("\n\t\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        DisplayLightAndSound(myFinch);
                        break;

                    case "b":
                        DisplayingItsDanceMoves(myFinch);
                        break;

                    case "c":

                        break;

                    case "d":

                        break;

                    case "q":
                        quitTalentShowMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitTalentShowMenu);
        }
        /// <summary>
        /// *****************************************************************
        /// *                   Talent Show > Dance Moves                   *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        static void DisplayingItsDanceMoves(Finch myFinch)
        {
            Console.WriteLine("\n\tLets play Music then dance to it!");
            Console.Write("\n\tHow many dance moves do you want to see?:");
            string usersResponse = Console.ReadLine();
            Console.WriteLine($"\n\tYou choose {usersResponse} dance moves.");
            bool validResponse;

            do
            {
                validResponse = true;

                if (!int.TryParse(usersResponse, out int DanceMoves))
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number:");
                    usersResponse = Console.ReadLine();
                }

                else if (DanceMoves > 0)
                {
                    for (int i = 0; i < DanceMoves; i++)
                    {
                        myFinch.setLED(255, 0, 0);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 0, 255);
                        myFinch.noteOn(1567);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 255, 0);
                        myFinch.noteOn(1175);
                        myFinch.wait(1000);
                        myFinch.setLED(0, 0, 0);
                    }

                    myFinch.noteOn(2049);
                    myFinch.wait(1000);
                    myFinch.noteOff();

                    Console.WriteLine("\n\tKnow lets Dance!");

                    for (int i = 0; i < DanceMoves; i++)
                    {
                        myFinch.setLED(255, 0, 0);
                        myFinch.setMotors(125, -125);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.noteOn(1567);
                        myFinch.wait(500);
                        myFinch.noteOn(1175);
                        myFinch.wait(500);
                        myFinch.setLED(0, 255, 0);
                        myFinch.setMotors(-125, 125);
                        myFinch.noteOn(1049);
                        myFinch.wait(1000);
                        myFinch.noteOn(1567);
                        myFinch.wait(500);
                        myFinch.noteOn(1175);
                        myFinch.wait(500);
                        myFinch.noteOff();
                    }
                    myFinch.setMotors(50, 50);
                    myFinch.noteOn(2049);
                    myFinch.wait(1000);
                    myFinch.noteOn(3135);
                    myFinch.wait(500);
                    myFinch.noteOn(2349);
                    myFinch.wait(500);
                    myFinch.setMotors(0, 0);
                    myFinch.noteOff();
                }
                else
                {
                    validResponse = false;

                    Console.Write("\n\tPlease enter a valid number:");
                    usersResponse = Console.ReadLine();
                }

            } while (!validResponse);

            Console.CursorVisible = false;
            DisplayMenuPrompt("Talent Show");

        }
        /// <summary>
        /// *****************************************************************
        /// *               Talent Show > Light and Sound                   *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        static void DisplayLightAndSound(Finch myFinch)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Light and Sound");

            Console.WriteLine("\tThe Finch robot will now show off\n\tits glowing talent!");
            DisplayContinuePrompt();

            for (int lightSoundLevel = 0; lightSoundLevel < 255; lightSoundLevel++)
            {
                myFinch.setLED(lightSoundLevel, lightSoundLevel, lightSoundLevel);
                myFinch.noteOn(lightSoundLevel * 100);
            }

            DisplayMenuPrompt("Talent Show");
        }

        #endregion

        #region FINCH ROBOT MANAGEMENT

        /// <summary>
        /// *****************************************************************
        /// *               Disconnect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        static void DisplayDisconnectFinchRobot(Finch myFinch)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("\n\tAbout to disconnect from the Finch robot.");
            DisplayContinuePrompt();

            myFinch.disConnect();

            Console.WriteLine("\n\tThe Finch robot is now disconnect.");

            DisplayMenuPrompt("Main Menu");
        }

        /// <summary>
        /// *****************************************************************
        /// *                  Connect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="myFinch">finch robot object</param>
        /// <returns>notify if the robot is connected</returns>
        static bool DisplayConnectFinchRobot(Finch myFinch)
        {
            Console.CursorVisible = false;

            bool robotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine("\tAbout to connect to Finch robot.\n\n\tPlease be sure the USB cable is connected\n\n\tto the robot and computer now.");
            DisplayContinuePrompt();

            robotConnected = myFinch.connect();

            myFinch.noteOn(1265);
            myFinch.wait(250);
            myFinch.noteOff();
            myFinch.setLED(255, 0, 0);
            myFinch.wait(250);
            myFinch.setLED(0, 0, 255);
            myFinch.wait(250);
            myFinch.setLED(0, 255, 0);
            myFinch.wait(250);
            myFinch.setLED(0, 0, 0);

            // TODO test connection and provide user feedback - text, lights, sounds

            Console.CursorVisible = false;
            DisplayMenuPrompt("Main Menu");

            //
            // reset finch robot
            //

            myFinch.setLED(0, 0, 0);
            myFinch.noteOff();

            return robotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// *****************************************************************
        /// *                     Welcome Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Closing Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display menu prompt
        /// </summary>
        static void DisplayMenuPrompt(string menuName)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPress any key to return to the {menuName} Menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>

        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}
#endregion