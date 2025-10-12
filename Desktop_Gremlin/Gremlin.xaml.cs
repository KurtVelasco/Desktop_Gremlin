using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
namespace Desktop_Gremlin
{

    public partial class Gremlin : Window
    {
        //To those reading this, I'm sorry for this messy code, or not//
        //In the future I'm planning to seperate major code snippets into diffrent class files//
        //Instead of barfing evrything in 1 file//
        //Thanks and have a Mamboful day//

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);  
    
      
        private DateTime _nextRandomActionTime = DateTime.Now.AddSeconds(10);
        private Random _rng = new Random();
        private bool _wasIdleLastFrame = false;
        public static DispatcherTimer _masterTimer;
        private DispatcherTimer _idleTimer;
        private DispatcherTimer _activeRandomMoveTimer;
        private MediaPlayer _walkLoopPlayer;
        private bool _isWalkingSoundPlaying = false;
        private TrayManager _trayManager;
        public struct POINT
        {
            public int X;
            public int Y;
        }
        public Gremlin()
        {
            InitializeComponent();

            SpriteImage.Source = new CroppedBitmap();
            ConfigManager.LoadMasterConfig();
            ConfigManager.LoadConfigChar();

            _trayManager = new TrayManager(this);
            _trayManager.SetupTrayIcon();

            InitializeAnimations();
            InitializeTimers();
            MediaManager.PlaySound("intro.wav");
        }
        public void InitializeTimers()
        {

            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromSeconds(Settings.SleepTime);
            _idleTimer.Tick += IdleTimer_Tick;
            _idleTimer.Start();

            _walkLoopPlayer = new MediaPlayer();
            _walkLoopPlayer.Open(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "Aoba", "steps.wav")));
            _walkLoopPlayer.Volume = 1; 
            _walkLoopPlayer.MediaEnded += (s, e) =>
            {
                _walkLoopPlayer.Position = TimeSpan.Zero;
                _walkLoopPlayer.Play(); 
            };
        }
       
        private void InitializeAnimations()
        {
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {
                if (AnimationStates.GetState("Intro"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Intro = AnimationPlayer.PlayAnimation("intro",CurrentFrames.Intro,FrameCounts.Intro,SpriteImage);

                    if (CurrentFrames.Intro == 0)
                    {
                        AnimationStates.ResetAllExceptIdle();   
                        AnimationStates.UnlockState();
                    }
                }
                if (AnimationStates.GetState("Dragging"))
                {
                    CurrentFrames.Grab = AnimationPlayer.PlayAnimation("grab",CurrentFrames.Grab,
                        FrameCounts.Grab,SpriteImage);
                }
                if (AnimationStates.GetState("Emote1"))
                {
                    AnimationStates.LockState(); 
                    CurrentFrames.Emote1 = AnimationPlayer.PlayAnimation("emote1", CurrentFrames.Emote1, FrameCounts.Emote1, SpriteImage);
                    if (CurrentFrames.Emote1 == 0)
                    { 
                      AnimationStates.UnlockState(); 
                      AnimationStates.ResetAllExceptIdle(); 
                    }
                }
                if (AnimationStates.GetState("Emote2"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Emote2 = AnimationPlayer.PlayAnimation("emote2", CurrentFrames.Emote2, FrameCounts.Emote2, SpriteImage);
                    if (CurrentFrames.Emote2 == 0)
                    {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Emote3"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Emote3 = AnimationPlayer.PlayAnimation("emote3", CurrentFrames.Emote3, FrameCounts.Emote3, SpriteImage);
                    if (CurrentFrames.Emote3 == 0)
                    {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Emote4"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Emote4 = AnimationPlayer.PlayAnimation("emote4", CurrentFrames.Emote4, FrameCounts.Emote4, SpriteImage);
                    if (CurrentFrames.Emote4 == 0)
                    {
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Reload"))
                {
                    if(CurrentFrames.Reload == 0)
                    {
                        MediaManager.PlaySound("reload.wav");
                    };
                    CurrentFrames.Reload = AnimationPlayer.PlayAnimation("reload", CurrentFrames.Reload,
                        FrameCounts.Reload, SpriteImage);
                    if (CurrentFrames.Reload == 0)
                    {
                        Settings.CurrentAmmo = Settings.Ammo;
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Idle"))
                {
                    CurrentFrames.Idle = AnimationPlayer.PlayAnimation("idle", CurrentFrames.Idle,
                        FrameCounts.Idle, SpriteImage);
                }
                if (AnimationStates.GetState("Click"))
                {
                    AnimationStates.UnlockState();
                    AnimationStates.LockState();
                    CurrentFrames.Click = AnimationPlayer.PlayAnimation("click",
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
                    CurrentFrames.Hover = AnimationPlayer.PlayAnimation(
                        "hover",
                        CurrentFrames.Hover,
                        FrameCounts.Hover,
                        SpriteImage);

                }
                if (AnimationStates.GetState("Sleeping"))
                {
                    CurrentFrames.Sleep = AnimationPlayer.PlayAnimation(
                       "sleep",
                       CurrentFrames.Sleep,
                       FrameCounts.Sleep,
                       SpriteImage);
                }
                if (AnimationStates.GetState("Pat"))
                {
                    AnimationStates.LockState();
                    CurrentFrames.Pat = AnimationPlayer.PlayAnimation(
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
                            CurrentFrames.Right = AnimationPlayer.PlayAnimation(
                                "right",
                                CurrentFrames.Right,
                                FrameCounts.Right,
                                SpriteImage);
                        }
                        else if (angle >= 22.5 && angle < 67.5)
                        {
                            CurrentFrames.DownRight = AnimationPlayer.PlayAnimation(
                                "downRight",
                                CurrentFrames.DownRight,
                                FrameCounts.DownRight,
                                SpriteImage);
                        }
                        else if (angle >= 67.5 && angle < 112.5)
                        {
                            CurrentFrames.Down = AnimationPlayer.PlayAnimation(
                                "forward",
                                CurrentFrames.Down,
                                FrameCounts.Down,
                                SpriteImage);
                        }
                        else if (angle >= 112.5 && angle < 157.5)
                        {
                            CurrentFrames.DownLeft = AnimationPlayer.PlayAnimation(
                                "downLeft",
                                CurrentFrames.DownLeft,
                                FrameCounts.DownLeft,
                                SpriteImage);
                        }
                        else if (angle >= 157.5 && angle < 202.5)
                        {
                            CurrentFrames.Left = AnimationPlayer.PlayAnimation(
                                "left",
                                CurrentFrames.Left,
                                FrameCounts.Left,
                                SpriteImage);
                        }
                        else if (angle >= 202.5 && angle < 247.5)
                        {
                            CurrentFrames.UpLeft = AnimationPlayer.PlayAnimation(
                                "upLeft",
                                CurrentFrames.UpLeft,
                                FrameCounts.UpLeft,
                                SpriteImage);
                        }
                        else if (angle >= 247.5 && angle < 292.5)
                        {
                            CurrentFrames.Up = AnimationPlayer.PlayAnimation(
                                "backward",
                                CurrentFrames.Up,
                                FrameCounts.Up,
                                SpriteImage);
                        }
                        else if (angle >= 292.5 && angle < 337.5)
                        {
                            CurrentFrames.UpRight = AnimationPlayer.PlayAnimation(
                                "upRight",
                                CurrentFrames.UpRight,
                                FrameCounts.UpRight,
                                SpriteImage);
                        }                       
                        if (!_isWalkingSoundPlaying &&Settings.FootStepSounds)
                        {
                            _walkLoopPlayer.Play();
                            _isWalkingSoundPlaying = true;
                        }
                                              
                    }
                    else
                    {
                        CurrentFrames.WalkIdle = AnimationPlayer.PlayAnimation(
                            "wIdle",
                            CurrentFrames.WalkIdle,
                            FrameCounts.WalkIdle,
                            SpriteImage);

                        if (_isWalkingSoundPlaying && Settings.FootStepSounds) 
                        {
                            _walkLoopPlayer.Stop();
                            _isWalkingSoundPlaying = false;
                        }
                        
                    }
                 

                }
                bool isIdleNow = AnimationStates.IsCompletelyIdle();

                if (Settings.AllowRandomness)
                {
                    if (isIdleNow && !_wasIdleLastFrame)
                    {
                        int interval = _rng.Next(Settings.MinInterval, Settings.MaxInterval);
                        _nextRandomActionTime = DateTime.Now.AddSeconds(interval);
                    }
                    if (isIdleNow && DateTime.Now >= _nextRandomActionTime)
                    {
                        AnimationStates.SetState("Random");

                        int action = _rng.Next(0, 7);
                        switch (action)
                        {
                            case 0:
                                RandomMove();
                                break;
                            case 1:
                                AnimationStates.SetState("Reload");
                                AnimationStates.LockState();
                                break;
                            case 2:
                                CurrentFrames.Pat = 0;
                                AnimationStates.UnlockState();
                                AnimationStates.SetState("Pat");
                                MediaManager.PlaySound("pat.wav");
                                break;
                            case 3:
                                CurrentFrames.Click = 0;
                                AnimationStates.UnlockState();
                                AnimationStates.SetState("Click");
                                MediaManager.PlaySound("mambo.wav");
                                break;
                            case 4:
                                ResetIdleTimer();
                                CurrentFrames.Emote1 = 0;
                                AnimationStates.UnlockState();
                                if (Settings.CurrentAmmo <= 0)
                                {
                                    AnimationStates.SetState("Reload");
                                    AnimationStates.LockState();
                                }
                                else
                                {
                                    AnimationStates.SetState("Emote1");
                                    MediaManager.PlaySound("emote2.wav");
                                    Settings.CurrentAmmo--;
                                }
                                break;
                            case 5:
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
                                    Settings.CurrentAmmo--;
                                }
                                break;
                            case 6:
                                RandomMove();
                                break;
                        }
                  
                        int intervalAfterAction = _rng.Next(Settings.MinInterval, Settings.MaxInterval);
                        _nextRandomActionTime = DateTime.Now.AddSeconds(intervalAfterAction);
                    }
                }
                _wasIdleLastFrame = isIdleNow;
            };      
            _masterTimer.Start();     
        }
        private void RandomMove()
        {
            _activeRandomMoveTimer?.Stop();

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

                if (moveCount >= step || !AnimationStates.GetState("Random"))
                {
                    moveTimer.Stop();
                    _activeRandomMoveTimer = null;
                    AnimationStates.SetState("Idle");
                    return;
                }
                this.Left += dx;
                this.Top += dy;
                moveCount++;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (dx > 0)
                    {
                        CurrentFrames.WalkR = AnimationPlayer.PlayAnimation("walkR", CurrentFrames.WalkR,
                            FrameCounts.WalkR, SpriteImage);
                    }
                    else
                    {
                        CurrentFrames.WalkL = AnimationPlayer.PlayAnimation("walkL", CurrentFrames.WalkL,
                            FrameCounts.WalkL, SpriteImage);
                    }
                }
                else
                {
                    if (dy > 0)
                    {
                        CurrentFrames.WalkDown =    AnimationPlayer.PlayAnimation("walkDown", CurrentFrames.WalkDown,
                            FrameCounts.WalkDown, SpriteImage);
                    }
                    else
                    {
                        CurrentFrames.WalkUp = AnimationPlayer.PlayAnimation("walkUp", CurrentFrames.WalkUp,
                            FrameCounts.WalkUp,SpriteImage);
                    }
                }
           
            };
            moveTimer.Start();
        }
        private void SpriteImage_RightClick(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Click = 0;
            AnimationStates.UnlockState();
            AnimationStates.SetState("Click");
            MediaManager.PlaySound("mambo.wav");
        }

        private void SpriteImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimationStates.SetState("Hover");
            MediaManager.PlaySound("hover.wav", 5);           
        }
        private void SpriteImage_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            AnimationStates.SetState("Idle");
            CurrentFrames.Hover = 0;
            
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            AnimationStates.UnlockState();
            AnimationStates.SetState("Dragging");
            MediaManager.PlaySound("grab.wav");
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
                MediaManager.PlaySound("run.wav");
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
            MediaManager.PlaySound("pat.wav");
        }
        private void LeftHotspot_Click(object sender, MouseButtonEventArgs e)
        {
          
            ResetIdleTimer();
            CurrentFrames.Emote1 = 0;
            AnimationStates.UnlockState();
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
        private void RightHotspot_Click(object sender, MouseButtonEventArgs e)
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
    }
}
