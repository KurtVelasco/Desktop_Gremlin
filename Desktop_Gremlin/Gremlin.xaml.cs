using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Media;
namespace Desktop_Gremlin
{

    public partial class Gremlin : Window
    {


        //To those reading this, I'm sorry for this messy code, or not//
        //In the future I'm planning to seperate major code snippets into diffrent class files//
        //Instead of barfing evrything in 1 file//
        //Thanks and have a Mamboful day//
        private NotifyIcon TRAY_ICON;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        public static class Settings
        {
            public static int SpriteColumn { get; set; } = 10;
            public static int FrameRate { get; set; } = 31;
            public static string StartingChar { get; set; } = "Haru";
            public static double FollowRadius { get; set; } = 150.0;
            public static int FrameWidth { get; set; } = 200;
            public static int FrameHeight { get; set; } = 200;

            public static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
        }
        public class AnimationFrameCounts
        {
            public int Intro { get; set; } = 0;
            public int Idle { get; set; } = 0;
            public int Left { get; set; } = 0;
            public int Right { get; set; } = 0;
            public int Up { get; set; } = 0;
            public int Down { get; set; } = 0;
            public int Outro { get; set; } = 0;
            public int Grab { get; set; } = 0;
            public int WalkIdle { get; set; } = 0;
            public int Click { get; set; } = 0;
            public int Dance { get; set; } = 0;
            public int Hover { get; set; } = 0;
            public int Sleep { get; set; } = 0; 
        }
        
        public class AnimationFrame
        {
            public int Intro { get; set; } = 0;
            public int Idle { get; set; } = 0;
            public int Outro { get; set; } = 0;
            public int WalkDown { get; set; } = 0;
            public int WalkUp { get; set; } = 0;
            public int WalkRight { get; set; } = 0;
            public int WalkLeft { get; set; } = 0;
            public int Grab { get; set; } = 0;
            public int WalkIdle { get; set; } = 0;
            public int Click { get; set; } = 0;
            public int Dance { get; set; } = 0;
            public int Hover { get; set; } = 0;
            public int Sleep { get; set; } = 0; 
        }
        public class State
        {
            //Some of the States are unused, I'll used them in the future
            //If I need more percision when it comes to state managment
            public bool IsIntro { get; set; } = true;
            public bool ForwardAnimation { get; set; } = true;
            public bool IsRandom { get; set; } = false;
            public bool IsHover { get; set; } = false;
            public bool IsIdle { get; set; } = true;
            public bool IsWalking { get; set; } = false;
            public bool IsDragging { get; set; } = false;
            public bool IsWalkIdle { get; set; } = false;
            public bool IsClick { get; set; } = false;
            public bool IsSleeping { get; set; } = false;   
        }
        public class MouseSettings
        {
            public bool FollowCursor { get; set; } = false;
            public System.Drawing.Point LastMousePosition { get; set; }
            public double FollowSpeed { get; set; } = 5.0;
            public double MouseX { get; set; }
            public double MouseY { get; set; }
            public double Speed { get; set; } = 10.0;

        }
        public MouseSettings Mouse = new MouseSettings();
        public AnimationFrame CurrentFrames = new AnimationFrame();
        public State States = new State();
        public AnimationFrameCounts FrameCounts = new AnimationFrameCounts();

        private DispatcherTimer _closeTimer;
        private DispatcherTimer _masterTimer;
        private DispatcherTimer _idleTimer;
        public struct POINT
        {
            public int X;
            public int Y;
        }

 
        public Gremlin()
        {

            this.ShowInTaskbar = false;
            InitializeComponent();
            SpriteImage.Source = new CroppedBitmap();
            LoadMasterConfig();
            LoadConfigChar();
            SetupTrayIcon();
            InitializeAnimations();
            PlaySound("intro.wav");
            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromSeconds(120);
            _idleTimer.Tick += IdleTimer_Tick; ;
            _idleTimer.Start();
        }
   
        //TODO: Put this on a seperate class
        public static class SpriteManager
        {
            private static readonly Dictionary<string, BitmapImage> Cache = new Dictionary<string, BitmapImage>();
            private static readonly HashSet<string> AlwaysCached = new HashSet<string>()
            {
                "idle", "left", "right", "forward", "backward", "widle"
            };

            private static string _currentExtra = null;

            public static BitmapImage Get(string animationName)
            {
                animationName = animationName.ToLower();

                if (Cache.TryGetValue(animationName, out var sheet))
                {
                    return sheet;
                }

                string fileName = GetFileName(animationName);
                if (fileName == null)
                {
                    NormalError("Unknown animation requested: " + animationName, "Sprite Error");
                    return null;
                }
                sheet = LoadSprite(Settings.StartingChar, fileName);
                if (sheet != null)
                {
                    Cache[animationName] = sheet;
                    if (!AlwaysCached.Contains(animationName))
                    {
                        if (_currentExtra != null && _currentExtra != animationName)
                        {
                            Cache.Remove(_currentExtra);
                        }
                        _currentExtra = animationName;
                    }
                }

                return sheet;
            }
            private static string GetFileName(string animationName)
            {
                switch (animationName.ToLower())
                {
                    case "idle": 
                        return "idle.png";
                    case "intro": 
                        return "intro.png";
                    case "left": 
                        return "left.png";
                    case "right": 
                        return "right.png";
                    case "forward": 
                        return "forward.png";
                    case "backward": 
                        return "backward.png";
                    case "outro": 
                        return "outro.png";
                    case "grab": 
                        return "grab.png";
                    case "widle": 
                        return "wIdle.png";
                    case "click": 
                        return "click.png";
                    case "hover": 
                        return "hover.png";
                    case "sleep":
                        return "sleep.png";
                    default: 
                        return null;
                }
            }
            public static void PruneCache()
            {
                var keysToRemove = new List<string>();

                foreach (var key in Cache.Keys)
                {
                    if (!AlwaysCached.Contains(key.ToLower()))
                    {
                        keysToRemove.Add(key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    Cache.Remove(key);
                }
            }
            public static void ClearCache()
            {
                foreach (var img in Cache.Values)
                {
                    img.StreamSource?.Dispose();
                }
                Cache.Clear();
                _currentExtra = null;
            }
            private static BitmapImage LoadSprite(string filefolder, string fileName, string rootFolder = "Gremlins")
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "SpriteSheet", rootFolder, filefolder, fileName);

                if (!File.Exists(path))
                    return null;

                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(path);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static void FatalError(string message, string title = "Error")
        {
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            System.Windows.Application.Current.Shutdown();
        }
        public static void NormalError(string message, string title = "Error")
        {
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private int PlayAnimation(BitmapImage sheet, int currentFrame, int frameCount, int frameWidth, int frameHeight, System.Windows.Controls.Image targetImage, bool reverse = false)
        {
            if (sheet == null)
            {
                NormalError("Animation sheet missing or failed to load.", "Animation Error");
                return currentFrame;
            }

            int x = (currentFrame % Settings.SpriteColumn) * frameWidth;
            int y = (currentFrame / Settings.SpriteColumn) * frameHeight;

            if (x + frameWidth > sheet.PixelWidth || y + frameHeight > sheet.PixelHeight)
            {
                return currentFrame;
            }

            targetImage.Source = new CroppedBitmap(sheet, new Int32Rect(x, y, frameWidth, frameHeight));

            if (!reverse)
            {
                return (currentFrame + 1) % frameCount;
            }
            else
            {
                if (States.ForwardAnimation)
                {
                    currentFrame++;

                    if (currentFrame >= frameCount - 1)
                    {
                        States.ForwardAnimation = false;
                    } 
                }
                else
                {
                    currentFrame--;
                    if (currentFrame <= 0)
                    {
                        States.ForwardAnimation = true;
                    } 
                }
                return currentFrame;
            }
        }
        private void InitializeAnimations()
        {
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {
                if(States.IsSleeping && !States.IsIntro)
                {
                    CurrentFrames.Sleep =
                    PlayAnimation(
                    SpriteManager.Get("sleep"),
                    CurrentFrames.Sleep,
                    FrameCounts.Sleep,
                    Settings.FrameWidth,
                    Settings.FrameHeight,   
                    SpriteImage);
                }   
                if (States.IsIntro)
                {
                    CurrentFrames.Intro = PlayAnimation(
                        SpriteManager.Get("intro"),
                        CurrentFrames.Intro,
                        FrameCounts.Intro,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);

                    if (CurrentFrames.Intro == 0)
                    {
                        States.IsIntro = false;
                    }
                }
                if (States.IsDragging)
                {
                    CurrentFrames.Grab = PlayAnimation(
                        SpriteManager.Get("grab"),
                        CurrentFrames.Grab,
                        FrameCounts.Grab,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    States.IsIntro = false;
                    States.IsClick = false;
                    States.IsSleeping = false;
                }

                if (States.IsHover && !States.IsDragging && !States.IsSleeping)
                {
                    CurrentFrames.Hover = PlayAnimation(
                        SpriteManager.Get("hover"),
                        CurrentFrames.Hover,
                        FrameCounts.Hover,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                }

                if (States.IsClick)
                {
                    CurrentFrames.Click = PlayAnimation(
                        SpriteManager.Get("click"),
                        CurrentFrames.Click,
                        FrameCounts.Click,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);

                    States.IsIntro = false;
                    States.IsSleeping = false;
                    if (CurrentFrames.Click == 0)
                    {
                        States.IsClick = false;
                    }
                }
                if (!States.IsSleeping && !States.IsIntro && !States.IsDragging && !States.IsWalkIdle && !States.IsClick && !States.IsHover)
                {
                    CurrentFrames.Idle = PlayAnimation(
                        SpriteManager.Get("idle"),
                        CurrentFrames.Idle,
                        FrameCounts.Idle,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                }

                if (Mouse.FollowCursor && !States.IsDragging && !States.IsClick && !States.IsSleeping)
                {
                    POINT cursorPos;
                    GetCursorPos(out cursorPos);
                    var cursorScreen = new System.Windows.Point(cursorPos.X, cursorPos.Y);

                    double halfW = SpriteImage.ActualWidth > 0 ? SpriteImage.ActualWidth / 2.0 : Settings.FrameWidth / 2.0;
                    double halfH = SpriteImage.ActualHeight > 0 ? SpriteImage.ActualHeight / 2.0 : Settings.FrameHeight / 2.0;
                    var spriteCenterScreen = SpriteImage.PointToScreen(new System.Windows.Point(halfW, halfH));


                    //This is prob the most mind boggling code that I can find
                    //The prev iteration is the sprite would be offsetted to thr right because
                    //wpf screen use top-left instead of pure coorindates yada yada  ///
                    var source = PresentationSource.FromVisual(this);
                    System.Windows.Media.Matrix transformFromDevice = System.Windows.Media.Matrix.Identity;
                    if (source?.CompositionTarget != null)
                    {
                        transformFromDevice = source.CompositionTarget.TransformFromDevice;
                    }

                    var spriteCenterWpf = transformFromDevice.Transform(spriteCenterScreen);
                    var cursorWpf = transformFromDevice.Transform(cursorScreen);

                    double dx = cursorWpf.X - spriteCenterWpf.X;
                    double dy = cursorWpf.Y - spriteCenterWpf.Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance > Settings.FollowRadius)
                    {
                        double step = Math.Min(Mouse.Speed, distance - Settings.FollowRadius);
                        double nx = dx / distance;
                        double ny = dy / distance;
                        double moveX = nx * step;
                        double moveY = ny * step;

                        this.Left += moveX;
                        this.Top += moveY;

                        if (Math.Abs(moveX) > Math.Abs(moveY))
                        {
                            if (moveX < 0)
                            {
                                CurrentFrames.WalkLeft = PlayAnimation(
                                     SpriteManager.Get("left"),
                                    CurrentFrames.WalkLeft,
                                    FrameCounts.Left,
                                    Settings.FrameWidth,
                                    Settings.FrameHeight,
                                    SpriteImage);
                            }
                            else
                            {
                                CurrentFrames.WalkRight = PlayAnimation(
                                     SpriteManager.Get("right"),
                                    CurrentFrames.WalkRight,
                                    FrameCounts.Right,
                                    Settings.FrameWidth,
                                    Settings.FrameHeight,
                                    SpriteImage);
                            }
                        }
                        else
                        {
                            if (moveY > 0)
                            {
                                CurrentFrames.WalkDown = PlayAnimation(
                                     SpriteManager.Get("forward"),
                                    CurrentFrames.WalkDown,
                                    FrameCounts.Down,
                                    Settings.FrameWidth,
                                    Settings.FrameHeight,
                                    SpriteImage);
                            }
                            else
                            {
                                CurrentFrames.WalkUp = PlayAnimation(
                                     SpriteManager.Get("backward"),
                                    CurrentFrames.WalkUp,
                                    FrameCounts.Up,
                                    Settings.FrameWidth,
                                    Settings.FrameHeight,
                                    SpriteImage);
                            }
                        }
                    }
                    else
                    {
                        CurrentFrames.WalkIdle = PlayAnimation(
                            SpriteManager.Get("wIdle"),
                            CurrentFrames.WalkIdle,
                            FrameCounts.WalkIdle,
                            Settings.FrameWidth,
                            Settings.FrameHeight,
                            SpriteImage);
                    }
                }
            };

            _masterTimer.Start();
        }
      

        private void ResetApp()
        {
            TRAY_ICON.Visible = false;
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(exePath);
            System.Windows.Application.Current.Shutdown();
        }
        private void CloseApp()
        {
            _masterTimer.Stop();

            _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _closeTimer.Tick += (s, e) =>
            {
                try
                {
                    CurrentFrames.Outro = PlayAnimation(
                        SpriteManager.Get("outro"),
                        CurrentFrames.Outro,
                        FrameCounts.Outro,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);

                    if (CurrentFrames.Outro == 0)
                    {
                        TRAY_ICON.Visible = false;
                        TRAY_ICON?.Dispose();
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    
                    TRAY_ICON.Visible = false;
                    TRAY_ICON?.Dispose();               
                    System.Windows.Application.Current.Shutdown();
                }
            };

            _closeTimer.Start();

        }

        private void SetupTrayIcon()
        {
            TRAY_ICON = new NotifyIcon();
           
            if (File.Exists("icon.ico"))
            {
                TRAY_ICON.Icon = new Icon("icon.ico");
            }
            else
            {
                TRAY_ICON.Icon = SystemIcons.Application;
            }        
 
            TRAY_ICON.Visible = true;
            TRAY_ICON.Text = "Gremlin";

            var menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Reappear", null, (s, e) => ResetApp());
            menu.Items.Add("Close", null, (s, e) => CloseApp());
            //List<string> options = new List<string>
            //{
            //    "Mambo",
            //    "Haru",
            //};
            //var submenu = new ToolStripMenuItem("Options");
            //foreach (var option in options)
            //{
            //    var item = new ToolStripMenuItem(option);
            //    item.Click += (s, e) =>
            //    {
            //        Settings.StartingChar = option;
            //        SpriteManager.ClearCache();
            //        LoadConfigChar();
            //        _masterTimer.Stop();
            //        InitializeAnimations();
            //    };
            //    submenu.DropDownItems.Add(item);
            //}

            //menu.Items.Add(new ToolStripSeparator());
            //menu.Items.Add(submenu);

            TRAY_ICON.ContextMenuStrip = menu;
        }


        private void PlaySound(string fileName, double delaySeconds = 0)
        {
            string path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Sounds", Settings.StartingChar, fileName);

            if (!File.Exists(path))
            {
                return;
            }
            if (delaySeconds > 0)
            {
                if (Settings.LastPlayed.TryGetValue(fileName, out DateTime lastTime))
                {
                    if ((DateTime.Now - lastTime).TotalSeconds < delaySeconds)
                    {
                        return;
                    }
                }
            }
            try
            {
                using (SoundPlayer sp = new SoundPlayer(path))
                {
                    sp.Play();
                }
                Settings.LastPlayed[fileName] = DateTime.Now;
            }
            catch (Exception)
            {
            }
        }

        private void LoadMasterConfig()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (!File.Exists(path))
            {
                FatalError("Cannot find the config file for the main directory", "Missing Config File");
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
        private void LoadConfigChar()
        {
            string path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "SpriteSheet", "Gremlins", Settings.StartingChar, "config.txt");

            if (!File.Exists(path))
            {
                FatalError("Cannot find character name from config/folder. Please check config file if filename matches",
                    "Missing Character File");
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
                }
            }
        }
        private void SpriteImage_RightClick(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Click = 0;
            States.IsClick = !States.IsClick;
            if (States.IsClick)
            {
                PlaySound("mambo.wav");
            }
        }

        private void SpriteImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!States.IsIntro && !States.IsWalking)
            {
                States.IsHover = true;
            }
            if(!States.IsIntro && !States.IsWalking &&!States.IsSleeping &&!States.IsClick && !Mouse.FollowCursor)
            {
                PlaySound("hover.wav",3);
            }
        }

        private void SpriteImage_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!States.IsIntro)
            {
                States.IsHover = false;
                CurrentFrames.Hover = 0;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            TRAY_ICON?.Dispose();
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            States.IsDragging = true;
            DragMove();
            States.IsDragging = false;
            Mouse.FollowCursor = !Mouse.FollowCursor;
            if (Mouse.FollowCursor)
            {
                PlaySound("run.wav");
            }
        }
        private void ResetIdleTimer()
        {
            _idleTimer.Stop();
            _idleTimer.Start();
            States.IsSleeping = false;  
        }
        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            if (!States.IsSleeping)
            {
                States.IsSleeping = true;
            }
        }
    }



}
