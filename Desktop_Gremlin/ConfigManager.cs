using System;
using System.IO;

public static class ConfigManager
{
    public static void LoadMasterConfig()
    {
        string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        if (!File.Exists(path))
        {
             
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
            {
                continue;
            }

            var parts = line.Split('=');
            if (parts.Length != 2)
            {
                continue;
            }

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key.ToUpper())
            {
                case "START_CHAR":
                    {
                        Settings.StartingChar = value;
                        break;
                    }
                case "SPRITE_FRAMERATE":
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Settings.FrameRate = intValue;
                        }
                        break;
                    }
                case "FOLLOW_RADIUS":
                    {
                        if (double.TryParse(value, out double intValue))
                        {
                            Settings.FollowRadius = intValue;
                        }
                        break;
                    }         
                case "MAX_INTERVAL":
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Settings.MaxInterval = intValue;
                        }
                    }
                    break;
                case "MIN_INTERVAL":
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Settings.MinInterval = intValue;
                        }
                    }
                    break;
                case "RANDOM_MOVE_DISTANCE":
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Settings.MoveDistance = intValue;
                        }
                    }
                    break;
                case "ALLOW_RANDOM_ACTIONS":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.AllowRandomness = Value;
                        }               
                    }
                    break;
             case "ALLOW_GRAVITY":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.AllowGravity = Value;
                        }
                    }
                    break;
                case "SLEEP_TIME":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.SleepTime = Value;
                        }
                    }
                    break;
                case "ALLOW_FOOTSTEP_SOUNDS":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.FootStepSounds = Value;
                        }
                    }
                    break;
                case "AMMO":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.Ammo = Value;
                        }
                    }
                    break;
                }
            }
              
        }
    

    public static void LoadConfigChar()
    {
        string path = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SpriteSheet", "Gremlins", Settings.StartingChar, "config.txt");

        if (!File.Exists(path))
        {
            //FatalError("Cannot find character name from config/folder. Please check config file if filename matches",
            //    "Missing Character File");
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
            {
                continue;
            }

            var parts = line.Split('=');
            if (parts.Length != 2)
            {
                continue;
            }

            string key = parts[0].Trim();
            string value = parts[1].Trim();
            if (!int.TryParse(value, out int intValue))
            {
                continue;
            }
            switch (key.ToUpper())
            {
                case "INTRO":
                    FrameCounts.Intro = intValue;
                    break;
                case "IDLE":
                    FrameCounts.Idle = intValue;
                    break;
                case "UP":
                    FrameCounts.Up = intValue;
                    break;
                case "DOWN":
                    FrameCounts.Down = intValue;
                    break;
                case "LEFT":
                    FrameCounts.Left = intValue;
                    break;
                case "RIGHT":
                    FrameCounts.Right = intValue;
                    break;
                case "OUTRO":
                    FrameCounts.Outro = intValue;
                    break;
                case "GRAB":
                    FrameCounts.Grab = intValue;
                    break;
                case "WALK_IDLE":
                    FrameCounts.WalkIdle = intValue;
                    break;
                case "CLICK":
                    FrameCounts.Click = intValue;
                    break;
                case "HOVER":
                    FrameCounts.Hover = intValue;
                    break;
                case "SLEEP":
                    FrameCounts.Sleep = intValue;
                    break;
                case "FIRE_L":
                    FrameCounts.LeftFire = intValue;
                    break;
                case "FIRE_R":
                    FrameCounts.RightFire = intValue;
                    break;
                case "RELOAD":
                    FrameCounts.Reload = intValue;
                    break;
                case "PAT":
                    FrameCounts.Pat = intValue;
                    break;
                case "UPLEFT":
                    FrameCounts.UpLeft = intValue;
                    break;
                case "UPRIGHT":
                    FrameCounts.UpRight = intValue;
                    break;
                case "DOWNLEFT":
                    FrameCounts.DownLeft = intValue;
                    break;
                case "DOWNRIGHT":
                    FrameCounts.DownRight = intValue;
                    break;
                case "WALK_L":
                    FrameCounts.WalkL = intValue;
                    break;
                case "WALK_R":
                    FrameCounts.WalkR = intValue;
                    break;
                case "WALK_U":
                    FrameCounts.WalkUp = intValue;
                    break;
                case "WALK_D":
                    FrameCounts.WalkDown = intValue;
                    break;
                case "EMOTE1":
                    FrameCounts.Emote1 = intValue;
                    break;
                case "EMOTE2":
                    FrameCounts.Emote2 = intValue;
                    break;
                case "EMOTE3":
                    FrameCounts.Emote3 = intValue;
                    break;
                case "EMOTE4":
                    FrameCounts.Emote4 = intValue;
                    break;
                case "WIDTH":
                    Settings.FrameWidth = intValue;
                    break;
                case "HEIGHT":
                    Settings.FrameHeight = intValue;
                    break;
                case "COLUMN":
                    Settings.SpriteColumn = intValue;
                    break;
            }
        }
    }
}

