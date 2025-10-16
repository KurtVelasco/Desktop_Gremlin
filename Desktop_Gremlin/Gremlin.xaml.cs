using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Media;
using System.Collections.Generic;
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
<<<<<<< Updated upstream
    
        public static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
=======
>>>>>>> Stashed changes
        private DateTime _nextRandomActionTime = DateTime.Now.AddSeconds(10);

        private Random _rng = new Random();
<<<<<<< Updated upstream
=======

        private MediaPlayer _walkLoopPlayer;

        private bool _isWalkingSoundPlaying = false;
        private bool _wasIdleLastFrame = false;

>>>>>>> Stashed changes
        private DispatcherTimer _masterTimer;
        private DispatcherTimer _idleTimer;
        private DispatcherTimer _activeRandomMoveTimer;
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
            ConfigManager.LoadMasterConfig();
            ConfigManager.LoadConfigChar();
<<<<<<< Updated upstream
            SetupTrayIcon();
            InitializeAnimations();
            PlaySound("intro.wav");
=======
            InitializeAnimations();
            InitializeTimers();
            MediaManager.PlaySound("intro.wav");
        }
        public void InitializeTimers()
        {
>>>>>>> Stashed changes
            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromSeconds(Settings.SleepTime);
            _idleTimer.Tick += IdleTimer_Tick; ;
            _idleTimer.Start();
<<<<<<< Updated upstream
=======

            _walkLoopPlayer = new MediaPlayer();
            _walkLoopPlayer.Open(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", Settings.StartingChar, "steps.wav")));
            _walkLoopPlayer.Volume = 1; 
            _walkLoopPlayer.MediaEnded += (s, e) =>
            {
                _walkLoopPlayer.Position = TimeSpan.Zero;
                _walkLoopPlayer.Play(); 
            };
>>>>>>> Stashed changes
        }
        private int PlayAnimation(string sheetName, int currentFrame, int frameCount, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
        {
            BitmapImage sheet = SpriteManager.Get(sheetName);

            if (sheet == null)
            {
                return currentFrame;
            }
            int x = (currentFrame % Settings.SpriteColumn) * Settings.FrameWidth;
            int y = (currentFrame / Settings.SpriteColumn) * Settings.FrameHeight;

            if (x + Settings.FrameWidth > sheet.PixelWidth || y + Settings.FrameHeight > sheet.PixelHeight)
            {
                return currentFrame;
            }

            targetImage.Source = new CroppedBitmap(sheet, new Int32Rect(x, y, Settings.FrameWidth, Settings.FrameHeight));

            return (currentFrame + 1) % frameCount;
        }     
        private void InitializeAnimations()
        {
            _masterTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate)
            };
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {
                if (AnimationStates.GetState("Intro"))
                {
                    CurrentFrames.Intro = PlayAnimation("intro",CurrentFrames.Intro,FrameCounts.Intro,SpriteImage);

                    if (CurrentFrames.Intro == 0)
                    {
                        AnimationStates.ResetAllExceptIdle();   
                    }
                }
                if (AnimationStates.GetState("Dragging"))
                {
                    CurrentFrames.Grab = PlayAnimation("grab",CurrentFrames.Grab,
                        FrameCounts.Grab,SpriteImage);
                }
                if (AnimationStates.GetState("Emote1"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Emote1 = PlayAnimation("emote1", CurrentFrames.Emote1,
                        FrameCounts.Emote1, SpriteImage);
                }
                if (AnimationStates.GetState("Emote2"))
                {
                    CurrentFrames.Emote2 = PlayAnimation("emote2", CurrentFrames.Emote2,
                        FrameCounts.Emote2, SpriteImage);
                    if (CurrentFrames.Emote2 == 0)
                    {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Idle"))
                {
                    CurrentFrames.Idle = PlayAnimation("idle", CurrentFrames.Idle,
                        FrameCounts.Idle, SpriteImage);
                }
                if (AnimationStates.GetState("Idle"))
                {
                    CurrentFrames.Idle = PlayAnimation("idle",CurrentFrames.Idle,
                        FrameCounts.Idle,SpriteImage);
                }
                if (AnimationStates.GetState("Click"))
                {
                    AnimationStates.UnlockState();
                    AnimationStates.LockState();
                    CurrentFrames.Click = PlayAnimation("click",
                    CurrentFrames.Click,
                    FrameCounts.Click,SpriteImage);

                    if (CurrentFrames.Click == 0)
                    {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Hover"))
                {
                    CurrentFrames.Hover = PlayAnimation(
                        "hover",
                        CurrentFrames.Hover,
                        FrameCounts.Hover,
                        SpriteImage);

                }
                if (AnimationStates.GetState("Sleeping"))
                {
                    CurrentFrames.Sleep = PlayAnimation(
                       "sleep",
                       CurrentFrames.Sleep,
                       FrameCounts.Sleep,
                       SpriteImage);
                }
                if (AnimationStates.GetState("Pat"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Pat = PlayAnimation(
                      "pat",
                      CurrentFrames.Pat,
                      FrameCounts.Pat,
                      SpriteImage);
                    if (CurrentFrames.Pat == 0) {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (MouseSettings.FollowCursor && AnimationStates.GetState("Walking"))
                {
                    POINT cursorPos;
                    GetCursorPos(out cursorPos);
                    var cursorScreen = new System.Windows.Point(cursorPos.X, cursorPos.Y);

                    double halfW = SpriteImage.ActualWidth > 0 ? SpriteImage.ActualWidth / 2.0 : Settings.FrameWidth / 2.0;
                    double halfH = SpriteImage.ActualHeight > 0 ? SpriteImage.ActualHeight / 2.0 : Settings.FrameHeight / 2.0;
                    var spriteCenterScreen = SpriteImage.PointToScreen(new System.Windows.Point(halfW, halfH));


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
                        double step = Math.Min(MouseSettings.Speed, distance - Settings.FollowRadius);
                        double nx = dx / distance;
                        double ny = dy / distance;
                        double moveX = nx * step;
                        double moveY = ny * step;

                        this.Left += moveX;
                        this.Top += moveY;
                        double angle = Math.Atan2(moveY, moveX) * (180.0 / Math.PI);

                        if (angle < 0) angle += 360;


                        if (angle >= 337.5 || angle < 22.5)
                        {
                            CurrentFrames.Right = PlayAnimation(
                                "right",
                                CurrentFrames.Right,
                                FrameCounts.Right,
                                SpriteImage);
                        }
                        else if (angle >= 22.5 && angle < 67.5)
                        {
                            CurrentFrames.DownRight = PlayAnimation(
                                "downRight",
                                CurrentFrames.DownRight,
                                FrameCounts.DownRight,
                                SpriteImage);
                        }
                        else if (angle >= 67.5 && angle < 112.5)
                        {
                            CurrentFrames.Down = PlayAnimation(
                                "forward",
                                CurrentFrames.Down,
                                FrameCounts.Down,
                                SpriteImage);
                        }
                        else if (angle >= 112.5 && angle < 157.5)
                        {
                            CurrentFrames.DownLeft = PlayAnimation(
                                "downLeft",
                                CurrentFrames.DownLeft,
                                FrameCounts.DownLeft,
                                SpriteImage);
                        }
                        else if (angle >= 157.5 && angle < 202.5)
                        {
                            CurrentFrames.Left = PlayAnimation(
                                "left",
                                CurrentFrames.Left,
                                FrameCounts.Left,
                                SpriteImage);
                        }
                        else if (angle >= 202.5 && angle < 247.5)
                        {
                            CurrentFrames.UpLeft = PlayAnimation(
                                "upLeft",
                                CurrentFrames.UpLeft,
                                FrameCounts.UpLeft,
                                SpriteImage);
                        }
                        else if (angle >= 247.5 && angle < 292.5)
                        {
                            CurrentFrames.WalkUp = PlayAnimation(
                                "backward",
                                CurrentFrames.WalkUp,
                                FrameCounts.Up,
                                SpriteImage);
                        }
                        else if (angle >= 292.5 && angle < 337.5)
                        {
                            CurrentFrames.UpRight = PlayAnimation(
                                "upRight",
                                CurrentFrames.UpRight,
                                FrameCounts.UpRight,
                                SpriteImage);
                        }
                    }
                    else
                    {
                        CurrentFrames.WalkIdle = PlayAnimation(
                            "wIdle",
                            CurrentFrames.WalkIdle,
                            FrameCounts.WalkIdle,
                            SpriteImage);
                    }

                }
<<<<<<< Updated upstream
                if (Settings.AllowRandomness)
=======
                bool isIdleNow = AnimationStates.IsCompletelyIdle();

                if (Settings.AllowRandomness)   
>>>>>>> Stashed changes
                {
                    if (DateTime.Now >= _nextRandomActionTime)
                    {
                        PerformRandomAction();

                        int interval = _rng.Next(Settings.MinInterval, Settings.MaxInterval);
                        _nextRandomActionTime = DateTime.Now.AddSeconds(interval);
                    }
<<<<<<< Updated upstream
=======
                    if (isIdleNow && DateTime.Now >= _nextRandomActionTime)
                    {
                        AnimationStates.SetState("Random");

                        int action = _rng.Next(0, 3);
                        switch (action)
                        {
                            case 0:
                                RandomMove();
                                break;
                            case 1:
                                RandomMove();
                                break;
                            case 2:
                                CurrentFrames.Pat = 0;
                                AnimationStates.UnlockState();
                                AnimationStates.SetState("Pat");
                                MediaManager.PlaySound("pat.wav");
                                break;                      
                        }
                  
                        int intervalAfterAction = _rng.Next(Settings.MinInterval, Settings.MaxInterval);
                        _nextRandomActionTime = DateTime.Now.AddSeconds(intervalAfterAction);
                    }
>>>>>>> Stashed changes
                }
            };      
            _masterTimer.Start();     
        }
        private void PerformRandomAction()
        {
            if (AnimationStates.IsCompletelyIdle())
            {
                RandomMove();
            }

        }
        private void RandomMove()
        {
            _activeRandomMoveTimer?.Stop();

            AnimationStates.SetState("Random");

            double moveX = (_rng.NextDouble() - 0.5) * Settings.MoveDistance * 2;
            double moveY = (_rng.NextDouble() - 0.5) * Settings.MoveDistance * 2;

            double targetLeft = Math.Max(SystemParameters.WorkArea.Left,
            Math.Min(this.Left + moveX, SystemParameters.WorkArea.Right - SpriteImage.ActualWidth));
            double targetTop = Math.Max(SystemParameters.WorkArea.Top,
            Math.Min(this.Top + moveY, SystemParameters.WorkArea.Bottom - SpriteImage.ActualHeight));

            const double step = 120;
            double dx = (targetLeft - this.Left) / step;
            double dy = (targetTop - this.Top) / step;

            DispatcherTimer moveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            _activeRandomMoveTimer = moveTimer;

            int moveCount = 0;

            moveTimer.Tick += (s, e) =>
            {
           
                this.Left += dx;
                this.Top += dy;
                moveCount++;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (dx > 0)
                    {
                        CurrentFrames.WalkR = PlayAnimation("walkR", CurrentFrames.WalkR,
                            FrameCounts.WalkR, SpriteImage);
                    }
                    else
                    {
                        CurrentFrames.WalkL = PlayAnimation("walkL", CurrentFrames.WalkL,
                            FrameCounts.WalkL, SpriteImage);
                    }
                }
                else
                {
                    if (dy > 0)
                    {
                        CurrentFrames.WalkDown = PlayAnimation("walkDown", CurrentFrames.WalkDown,
                            FrameCounts.WalkDown, SpriteImage);
                    }
                    else
                    {
                        CurrentFrames.WalkUp = PlayAnimation("walkUp", CurrentFrames.WalkUp,
                            FrameCounts.WalkUp,SpriteImage);
                    }
                }

                if (moveCount >= step || !AnimationStates.GetState("Random") )
                {
                    moveTimer.Stop();
                    AnimationStates.SetState("Idle");
                    _activeRandomMoveTimer = null;
                }
            };
            moveTimer.Start();
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
            DispatcherTimer _closeTimer = new DispatcherTimer();
            _masterTimer?.Stop();
            _activeRandomMoveTimer?.Stop(); 
            _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _closeTimer.Tick += (s, e) =>
            {
                try
                {
                    CurrentFrames.Outro = PlayAnimation(
                        "outro",
                        CurrentFrames.Outro,
                        FrameCounts.Outro,
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
           
            if (File.Exists("ico.ico"))
            {
                TRAY_ICON.Icon = new Icon("ico.ico");
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
            TRAY_ICON.ContextMenuStrip = menu;
        }
        private void PlaySound(string fileName, double delaySeconds = 0)
        {
            string path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Sounds", Settings.StartingChar, fileName);

            if (!File.Exists(path))
                return;

            if (delaySeconds > 0)
            {
                if (LastPlayed.TryGetValue(fileName, out DateTime lastTime))
                {
                    if ((DateTime.Now - lastTime).TotalSeconds < delaySeconds)
                        return;
                }
            }

            try
            {
                SoundPlayer sp = new SoundPlayer(path);
                sp.Play();
                LastPlayed[fileName] = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sound error: " + ex.Message);
            }
        }
        private void SpriteImage_RightClick(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Click = 0;
            AnimationStates.UnlockState();
            AnimationStates.SetState("Click");
            PlaySound("mambo.wav");
                }

        private void SpriteImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            AnimationStates.SetState("Hover");
            if (AnimationStates.GetState("Hover"))
            {
                PlaySound("hover.wav", 5);
            }
            
        }
        private void SpriteImage_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            AnimationStates.SetState("Idle");
            CurrentFrames.Hover = 0;
            
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            TRAY_ICON?.Dispose();
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            AnimationStates.UnlockState();
            AnimationStates.SetState("Dragging");
            PlaySound("grab.wav");
            DragMove();
            AnimationStates.SetState("Idle");
            MouseSettings.FollowCursor = !MouseSettings.FollowCursor;
            if (MouseSettings.FollowCursor)
            {
                AnimationStates.SetState("Walking");
                AnimationStates.LockState();
            }
            CurrentFrames.Grab = 0;

            if (MouseSettings.FollowCursor)
            {
                PlaySound("run.wav");
            }
        }
        private void ResetIdleTimer()
        {
            _idleTimer.Stop();
            _idleTimer.Start();
        }
        private void IdleTimer_Tick(object sender, EventArgs e)
        {

            AnimationStates.SetState("Sleeping");
            AnimationStates.LockState();

        }
        private void TopHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Pat = 0;
            AnimationStates.UnlockState();
            AnimationStates.SetState("Pat");
            PlaySound("pat.wav");
        }
        private void LeftHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Emote1 = 0;
            AnimationStates.UnlockState();
<<<<<<< Updated upstream
            AnimationStates.SetState("Emote1");
            PlaySound("emote1.wav");
        }
        private void RightHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Emote2 = 0;
            AnimationStates.UnlockState();
            AnimationStates.SetState("Emote2");
            AnimationStates.LockState();
            PlaySound("emote2.wav");
        }
=======
            if (Settings.CurrentAmmo <= 0)
            {
                AnimationStates.SetState("Reload");
                AnimationStates.LockState();
 
            }
            else
            {
                AnimationStates.SetState("Emote1");
                MediaManager.PlaySound("emote2.wav");
            }
            Settings.CurrentAmmo--;
        }
        private void LeftDownHotspot_Click(object sender, MouseButtonEventArgs e)
        {

            ResetIdleTimer();
            CurrentFrames.Emote2 = 0;
            AnimationStates.UnlockState();
            if (Settings.CurrentAmmo <= 0)
            {
                AnimationStates.SetState("Reload");
                AnimationStates.LockState();

            }
            else
            {
                AnimationStates.SetState("Emote2");
                MediaManager.PlaySound("emote2.wav");
            }
            Settings.CurrentAmmo--;
        }
        private void RightHotspot_Click(object sender, MouseButtonEventArgs e)
        {

            ResetIdleTimer();
            CurrentFrames.Emote3 = 0;
            AnimationStates.UnlockState();
            if (Settings.CurrentAmmo <= 0)
            {

                AnimationStates.SetState("Reload");
                AnimationStates.LockState();

            }
            else
            {
                AnimationStates.SetState("Emote3");
                MediaManager.PlaySound("emote2.wav");
            }

            Settings.CurrentAmmo--;
        }
        private void RightDownHotspot_Click(object sender, MouseButtonEventArgs e)
        {

            ResetIdleTimer();
            CurrentFrames.Emote4 = 0;
            AnimationStates.UnlockState();
            if (Settings.CurrentAmmo <= 0)
            {
                AnimationStates.SetState("Reload");
                AnimationStates.LockState();

            }
            else
            {
                AnimationStates.SetState("Emote4");
                MediaManager.PlaySound("emote2.wav");
            }
            Settings.CurrentAmmo--;
        }
      
>>>>>>> Stashed changes
    }
}
