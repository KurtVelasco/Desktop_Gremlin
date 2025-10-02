using System;
using System.IO;

namespace Desktop_Gremlin
{
    public static class ConfigManager
    {
        public static void LoadMasterConfig()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (!File.Exists(path))
            {
                //FatalError("Cannot find the config file for the main directory", "Missing Config File");
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
                    case "SPRITE_SPEED":
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
                    case "SPRITE_COLUMN":
                        {
                            if (int.TryParse(value, out int intValue))
                            {
                                Settings.SpriteColumn = intValue;
                            }
                            break;
                        }
                    case "FRAME_HEIGHT":
                        {
                            if (int.TryParse(value, out int intValue))
                            {
                                Settings.FrameHeight = intValue;
                            }
                            break;
                        }
                    case "FRAME_WIDTH":
                        {
                            if (int.TryParse(value, out int intValue))
                            {
                                Settings.FrameWidth = intValue;
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
                    case "FRAME_RATE":
                        Settings.FrameRate = intValue;
                        break;
                    case "INTRO_FRAME_COUNT":
                        FrameCounts.Intro = intValue;
                        break;
                    case "IDLE_FRAME_COUNT":
                        FrameCounts.Idle = intValue;
                        break;
                    case "UP_FRAME_COUNT":
                        FrameCounts.Up = intValue;
                        break;
                    case "DOWN_FRAME_COUNT":
                        FrameCounts.Down = intValue;
                        break;
                    case "LEFT_FRAME_COUNT":
                        FrameCounts.Left = intValue;
                        break;
                    case "RIGHT_FRAME_COUNT":
                        FrameCounts.Right = intValue;
                        break;
                    case "OUTRO_FRAME_COUNT":
                        FrameCounts.Outro = intValue;
                        break;
                    case "GRAB_FRAME_COUNT":
                        FrameCounts.Grab = intValue;
                        break;
                    case "WALK_IDLE_FRAME_COUNT":
                        FrameCounts.WalkIdle = intValue;
                        break;
                    case "CLICK_FRAME_COUNT":
                        FrameCounts.Click = intValue;
                        break;
                    case "HOVER_FRAME_COUNT":
                        FrameCounts.Hover = intValue;
                        break;
                    case "SLEEP_FRAME_COUNT":
                        FrameCounts.Sleep = intValue;
                        break;
                    case "FIRE_L_COUNT":
                        FrameCounts.LeftFire = intValue;
                        break;
                    case "FIRE_R_COUNT":
                        FrameCounts.RightFire = intValue;
                        break;
                    case "RELOAD_COUNT":
                        FrameCounts.Reload = intValue;
                        break;
                    case "PAT_COUNT":
                        FrameCounts.Pat = intValue;
                        break;
                }
            }
        }
    }
}
