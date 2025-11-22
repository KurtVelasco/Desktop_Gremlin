using Desktop_Gremlin;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

public static class ConfigManager
{
    public static void LoadMasterConfig()
    {
        string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        if (!File.Exists(path))
        {
            Gremlin.ErrorClose("Cannot find the Main config.txt", "Missing config.txt", true);
            return;
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
                case "COMPANION_CHAR":
                    {
                        Settings.CompanionChar = value;
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
                            Settings.RandomMaxInterval = intValue;
                        }
                    }
                    break;
                case "MIN_INTERVAL":
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Settings.RandomMinInterval = intValue;
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

                case "ALLOW_COLOR_HOTSPOT":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.AllowColoredHotSpot = Value;
                        }
                    }
                    break;
                case "SHOW_TASKBAR":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.ShowTaskBar = Value;
                        }
                    }
                    break;
                case "SPRITE_SCALE":
                    {
                        if (double.TryParse(value, out double Value))
                        {
                            Settings.SpriteSize = Value;
                        }
                    }
                    break;
                case "FORCE_FAKE_TRANSPARENT":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.FakeTransparent = Value;
                        }
                    }
                    break;
                case "ALLOW_ERROR_MESSAGES":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.AllowErrorMessages = Value;
                        }
                    }
                    break;
                case "MAX_ACCELERATION":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.MaxItemAcceleration = Value;
                        }
                    }
                    break;
                case "FOLLOW_ACCELERATION":
                    {
                        if (double.TryParse(value, out double Value))
                        {
                            Settings.ItemAcceleration = Value;
                        }
                    }
                    break;
                case "CURRENT_ACCELERATION":
                    {
                        if (double.TryParse(value, out double Value))
                        {
                            Settings.ItemAcceleration = Value;
                        }
                    }
                    break;
                case "MAX_EATING_SIZE":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.FoodItemGetSize = Value;
                        }
                    }
                    break;
                case "ITEM_WIDTH":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.ItemWidth = Value;
                        }
                    }
                    break;
                case "ITEM_HEIGHT":
                    {
                        if (int.TryParse(value, out int Value))
                        {
                            Settings.ItemHeight = Value;
                        }
                    }
                    break;
                case "COMPANIONS_SCALE":
                    {
                        if (double.TryParse(value, out double Value))
                        {
                            Settings.CompanionScale = Value;
                        }
                    }
                    break;
                case "ENABLE_MIN_RESIZE":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.EnableMinSize = Value;
                        }
                    }
                    break;
                case "FORCE_CENTER":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.ForceCenter = Value;
                        }
                    }
                    break;
                case "ENABLE_MANUAL_RESIZE":
                    {
                        if (bool.TryParse(value, out bool Value))
                        {
                            Settings.ManualReize = Value;
                        }
                    }
                    break;
                case "VOLUME_LEVEL":
                    {
                        if (double.TryParse(value, out double Value))
                        {
                            Settings.VolumeLevel = Value;
                        }
                    }
                    break;
            }

        }
    }
    
    public static FrameCounts LoadConfigChar(string character)
    {
        var result = new FrameCounts();
        string path = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SpriteSheet", "Gremlins", character, "config.txt");

        if (!File.Exists(path))
        {
            Gremlin.ErrorClose("Cannot find the SpriteSheet config.txt", "Missing config.txt", true);
            return result;
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
                case "INTRO": result.Intro = intValue; break;
                case "IDLE": result.Idle = intValue; break;
                case "IDLE2": result.Idle2 = intValue; break;
                case "RUNUP": result.Up = intValue; break;
                case "RUNDOWN": result.Down = intValue; break;
                case "RUNLEFT": result.Left = intValue; break;
                case "RUNRIGHT": result.Right = intValue; break;
                case "UPLEFT": result.UpLeft = intValue; break;
                case "UPRIGHT": result.UpRight = intValue; break;
                case "DOWNLEFT": result.DownLeft = intValue; break;
                case "DOWNRIGHT": result.DownRight = intValue; break;
                case "OUTRO": result.Outro = intValue; break;
                case "GRAB": result.Grab = intValue; break;
                case "RUNIDLE": result.RunIdle = intValue; break;
                case "CLICK": result.Click = intValue; break;
                case "HOVER": result.Hover = intValue; break;
                case "SLEEP": result.Sleep = intValue; break;
                case "FIREL": result.LeftFire = intValue; break;
                case "FIRER": result.RightFire = intValue; break;
                case "RELOAD": result.Reload = intValue; break;
                case "PAT": result.Pat = intValue; break;
                case "WALKLEFT": result.WalkL = intValue; break;
                case "WALKRIGHT": result.WalkR = intValue; break;
                case "WALKUP": result.WalkUp = intValue; break;
                case "WALKDOWN": result.WalkDown = intValue; break;
                case "EMOTE1": result.Emote1 = intValue; break;
                case "EMOTE2": result.Emote2 = intValue; break;
                case "EMOTE3": result.Emote3 = intValue; break;
                case "EMOTE4": result.Emote4 = intValue; break;
                case "JUMPSCARE": result.JumpScare = intValue; break;
                case "POOF": result.Poof = intValue; break;
                case "WIDTH": Settings.FrameWidth = intValue; break;
                case "HEIGHT": Settings.FrameHeight = intValue; break;
                case "COLUMN": Settings.SpriteColumn = intValue; break;
                case "WIDTHJS": Settings.FrameWidthJs = intValue; break;
                case "HEIGHTJS": Settings.FrameHeightJs = intValue; break;
            }
        }
        return result;
    }


    public static void ApplyXamlSettings(Gremlin window)
    {
        if (window == null)
        {
            return;
        }

        bool useColors = Settings.AllowColoredHotSpot;
        bool showTaskBar = Settings.ShowTaskBar;
        double scale = Settings.SpriteSize;

        ApplySettings(window, Settings.AllowColoredHotSpot, Settings.ShowTaskBar,Settings.SpriteSize,Settings.FakeTransparent,
            Settings.ManualReize,Settings.ForceCenter,Settings.EnableMinSize
            );
    }
    private static void ApplySettings(Gremlin window, bool useColors, bool showTaskBar, double scale, 
        bool useFakeTransparent, bool useManualReize, bool forCenter, bool enableMinResize)
    {
        Border LeftHotspot = window.LeftHotspot;
        Border LeftDownHotspot = window.LeftDownHotspot;
        Border RightHotspot = window.RightHotspot;
        Border RightDownHotspot = window.RightDownHotspot;
        Border TopHotspot = window.TopHotspot;
        System.Windows.Controls.Image SpriteImage = window.SpriteImage;

        if (useColors)
        {
            LeftHotspot.Background = new SolidColorBrush(Colors.Red);
            LeftDownHotspot.Background = new SolidColorBrush(Colors.Yellow);
            RightHotspot.Background = new SolidColorBrush(Colors.Blue);
            RightDownHotspot.Background = new SolidColorBrush(Colors.Orange);
            TopHotspot.Background = new SolidColorBrush(Colors.Purple);
        }
        else
        {
            var noColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#01000000"));
            LeftHotspot.Background = noColor;
            LeftDownHotspot.Background = noColor;
            RightHotspot.Background = noColor;
            RightDownHotspot.Background = noColor;
            TopHotspot.Background = noColor;
        }

        window.ShowInTaskbar = showTaskBar;

        if (useFakeTransparent)
        {
            window.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#01000000"));
        }
        if (useManualReize)
        {
            window.SizeToContent = SizeToContent.Manual;    
        }
        if(forCenter)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        double baseLeftW = LeftHotspot.Width, baseLeftH = LeftHotspot.Height;
        double baseLeftDownW = LeftDownHotspot.Width, baseLeftDownH = LeftDownHotspot.Height;
        double baseRightW = RightHotspot.Width, baseRightH = RightHotspot.Height;
        double baseRightDownW = RightDownHotspot.Width, baseRightDownH = RightDownHotspot.Height;
        double baseTopW = TopHotspot.Width, baseTopH = TopHotspot.Height;

        double originalWidth = SpriteImage.Width;
        double originalHeight = SpriteImage.Height;

        double newWidth = originalWidth * scale;
        double newHeight = originalHeight * scale;
        window.Width = window.Height * scale;
        window.Height = window.Height * scale;  

        SpriteImage.Width = newWidth;
        SpriteImage.Height = newHeight;
        if(enableMinResize)
        {
            window.MinWidth = window.Width;
            window.MinHeight = window.Height;
        }
       double leftHotspotOffsetX = LeftHotspot.Margin.Left - SpriteImage.Margin.Left;
        double leftHotspotOffsetY = LeftHotspot.Margin.Top - SpriteImage.Margin.Top;

        double leftDownOffsetX = LeftDownHotspot.Margin.Left - SpriteImage.Margin.Left;
        double leftDownOffsetY = LeftDownHotspot.Margin.Top - SpriteImage.Margin.Top;

        double rightOffsetX = RightHotspot.Margin.Left - SpriteImage.Margin.Left;
        double rightOffsetY = RightHotspot.Margin.Top - SpriteImage.Margin.Top;

        double rightDownOffsetX = RightDownHotspot.Margin.Left - SpriteImage.Margin.Left;
        double rightDownOffsetY = RightDownHotspot.Margin.Top - SpriteImage.Margin.Top;

        double topOffsetX = TopHotspot.Margin.Left - SpriteImage.Margin.Left;
        double topOffsetY = TopHotspot.Margin.Top - SpriteImage.Margin.Top;

        double centerX = (window.Width - newWidth) / 2;
        double centerY = (window.Height - newHeight) / 2;

        SpriteImage.Margin = new Thickness(centerX, centerY, 0, 0);

        double scaleX = newWidth / originalWidth;
        double scaleY = newHeight / originalHeight;
        ScaleHotspot(LeftHotspot, leftHotspotOffsetX, leftHotspotOffsetY, scaleX, scaleY, centerX, centerY, baseLeftW, baseLeftH);
        ScaleHotspot(LeftDownHotspot, leftDownOffsetX, leftDownOffsetY, scaleX, scaleY, centerX, centerY, baseLeftDownW, baseLeftDownH);
        ScaleHotspot(RightHotspot, rightOffsetX, rightOffsetY, scaleX, scaleY, centerX, centerY, baseRightW, baseRightH);
        ScaleHotspot(RightDownHotspot, rightDownOffsetX, rightDownOffsetY, scaleX, scaleY, centerX, centerY, baseRightDownW, baseRightDownH);
        ScaleHotspot(TopHotspot, topOffsetX, topOffsetY, scaleX, scaleY, centerX, centerY, baseTopW, baseTopH);
    }

    private static void ScaleHotspot(Border hotspot, double offsetX, double offsetY, double scaleX,
    double scaleY, double centerX, double centerY, double baseWidth, double baseHeight)
    {
        hotspot.Width = baseWidth * scaleX;
        hotspot.Height = baseHeight * scaleY;
        hotspot.Margin = new Thickness(centerX + offsetX * scaleX, centerY + offsetY * scaleY, 0, 0);
        
    }
    public class AppConfig
    {
        private readonly Window _window;
        private NotifyIcon _trayIcon;
        public AnimationStates _states;    
        public AppConfig(Window window, AnimationStates states)
        {
            _window = window;
            _states = states;
            SetupTrayIcon();
        }   
        public void SetupTrayIcon()
        {
            _trayIcon = new NotifyIcon();

            if (File.Exists("SpriteSheet/System/ico.ico"))
            {
                _trayIcon.Icon = new Icon("SpriteSheet/System/ico.ico");
            }
            else
            {
                _trayIcon.Icon = SystemIcons.Application;
            }

            _trayIcon.Visible = true;
            _trayIcon.Text = "Gremlin";

            var menu = new ContextMenuStrip();
            menu.Items.Add("Stylish Close", null, (s, e) => CloseApp());
            menu.Items.Add("Force Close", null, (s, e) => ForceClose());
            menu.Items.Add("Restart", null, (s, e) => RestartApp());
            _trayIcon.ContextMenuStrip = menu;
        }

        public void CloseApp()
        {
           _states.PlayOutro();  
            MediaManager.PlaySound("outro.wav", Settings.StartingChar); 
        }
        private void ForceClose()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void RestartApp()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(exePath);
            System.Windows.Application.Current.Shutdown();
        }
    }

}

