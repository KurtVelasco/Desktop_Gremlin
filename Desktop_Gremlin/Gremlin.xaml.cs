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
            ConfigManager.LoadMasterConfig();
            ConfigManager.LoadConfigChar();
            SetupTrayIcon();
            InitializeAnimations();
            PlaySound("intro.wav");
            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromSeconds(120);
            _idleTimer.Tick += IdleTimer_Tick; ;
            _idleTimer.Start();
        }
        private int PlayAnimation(BitmapImage sheet, int currentFrame, int frameCount, int frameWidth, int frameHeight, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
        {
            if (sheet == null)
            {
                //NormalError("Animation sheet missing or failed to load.", "Animation Error");
                return currentFrame;
            }
            int x = (currentFrame % Settings.SpriteColumn) * frameWidth;
            int y = (currentFrame / Settings.SpriteColumn) * frameHeight;

            if (x + frameWidth > sheet.PixelWidth || y + frameHeight > sheet.PixelHeight)
            {
                return currentFrame;
            }

            targetImage.Source = new CroppedBitmap(sheet, new Int32Rect(x, y, frameWidth, frameHeight));


            return (currentFrame + 1) % frameCount;
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
       

        private void PlayAnimationIdle()
        {
            System.Windows.MessageBox.Show("IDIOT", "Hikari");
        }

        //Will eventually update this verbose piece of shit code
        //For now, it works.
        private void InitializeAnimations()
        {
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {
                if(AnimationStates.IsSleeping && !AnimationStates.IsIntro)
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
                if (AnimationStates.IsIntro)
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
                        AnimationStates.IsIntro = false;
                    }
                }
                if (AnimationStates.IsDragging)
                {
                    if (CurrentFrames.Grab == 0)
                    {
                        PlaySound("grab.wav");
                    }
                    CurrentFrames.Grab = PlayAnimation(
                        SpriteManager.Get("grab"),
                        CurrentFrames.Grab,
                        FrameCounts.Grab,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
      
                    AnimationStates.IsIntro = false;
                    AnimationStates.IsClick = false;
                    AnimationStates.IsSleeping = false;
                }
                if (AnimationStates.IsPat)
                {
                    if (CurrentFrames.Pat == 0)
                    {
                        PlaySound("emote.wav");
                    }
                    CurrentFrames.Pat = PlayAnimation(
                        SpriteManager.Get("pat"),
                        CurrentFrames.Pat,
                        FrameCounts.Pat,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    AnimationStates.IsIntro = false;
                    AnimationStates.IsClick = false;
                    AnimationStates.IsSleeping = false;
                    if (CurrentFrames.Pat == 0)
                    {
                        AnimationStates.IsPat = false;   
                    }
                }

                if (AnimationStates.IsHover && !AnimationStates.IsDragging && !AnimationStates.IsSleeping)
                {
                    CurrentFrames.Hover = PlayAnimation(
                        SpriteManager.Get("hover"),
                        CurrentFrames.Hover,
                        FrameCounts.Hover,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    AnimationStates.IsPat = false;
                }
                if (Settings.Ammo <= 0)
                {
                    if (CurrentFrames.Reload == 0)
                    {
                        PlaySound("reload.wav");
                    }
                    AnimationStates.IsFiring_Left = false;
                    AnimationStates.IsFiring_Right = false;
                    CurrentFrames.Reload = PlayAnimation(
                        SpriteManager.Get("reload"),
                        CurrentFrames.Reload,
                        FrameCounts.Reload,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    if (CurrentFrames.Reload == 0)
                    {
                        Settings.Ammo = 5;
                    }
                }
                if (AnimationStates.IsClick)
                {
                    CurrentFrames.Click = PlayAnimation(
                        SpriteManager.Get("click"),
                        CurrentFrames.Click,
                        FrameCounts.Click,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);

                    AnimationStates.IsIntro = false;
                    AnimationStates.IsSleeping = false;
                    if (CurrentFrames.Click == 0)
                    {
                        AnimationStates.IsClick = false;
                    }
                }
                if (!AnimationStates.IsSleeping && !AnimationStates.IsIntro && !AnimationStates.IsDragging 
                && !AnimationStates.IsWalkIdle && !AnimationStates.IsClick && !AnimationStates.IsHover && !AnimationStates.IsFiring_Left &&!AnimationStates.IsFiring_Right
                && Settings.Ammo > 0 && !AnimationStates.IsPat)
                {
                    CurrentFrames.Idle = PlayAnimation(
                        SpriteManager.Get("idle"),
                        CurrentFrames.Idle,
                        FrameCounts.Idle,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                }
                if (AnimationStates.IsFiring_Left & Settings.Ammo > 0)
                {
                    if (CurrentFrames.LeftFire == 0)
                    {
                        PlaySound("fire.wav");
                    }
                    CurrentFrames.LeftFire = PlayAnimation(
                        SpriteManager.Get("fireL"),
                        CurrentFrames.LeftFire,
                        FrameCounts.LeftFire,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    if (CurrentFrames.LeftFire == 0)
                    {
                        AnimationStates.IsFiring_Left = false;
                        Settings.Ammo = Settings.Ammo - 1;
                    }    
                }
                if (AnimationStates.IsFiring_Right && Settings.Ammo > 0)
                {
                    if (CurrentFrames.RightFire == 0)
                    {
                        PlaySound("fire.wav");
                    }
                    CurrentFrames.RightFire = PlayAnimation(
                        SpriteManager.Get("fireR"),
                        CurrentFrames.RightFire,
                        FrameCounts.RightFire,
                        Settings.FrameWidth,
                        Settings.FrameHeight,
                        SpriteImage);
                    if (CurrentFrames.RightFire == 0)
                    {
                        AnimationStates.IsFiring_Right = false;
                        Settings.Ammo = Settings.Ammo - 1;
                    }
                }

                if (MouseSettings.FollowCursor &&!AnimationStates.IsPat && !AnimationStates.IsDragging && !AnimationStates.IsClick && !AnimationStates.IsSleeping &&!AnimationStates.IsFiring_Left
                    &&!AnimationStates.IsFiring_Right && Settings.Ammo > 0 )
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
                            // Right
                            CurrentFrames.WalkRight = PlayAnimation(
                                SpriteManager.Get("right"),
                                CurrentFrames.WalkRight,
                                FrameCounts.Right,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 22.5 && angle < 67.5)
                        {
                            // Down-Right
                            CurrentFrames.DownRight = PlayAnimation(
                                SpriteManager.Get("downRight"),
                                CurrentFrames.DownRight,
                                FrameCounts.DownRight,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 67.5 && angle < 112.5)
                        {
                            // Down
                            CurrentFrames.WalkDown = PlayAnimation(
                                SpriteManager.Get("forward"),
                                CurrentFrames.WalkDown,
                                FrameCounts.Down,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 112.5 && angle < 157.5)
                        {
                            // Down-Left
                            CurrentFrames.DownLeft = PlayAnimation(
                                SpriteManager.Get("downLeft"),
                                CurrentFrames.DownLeft,
                                FrameCounts.DownLeft,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 157.5 && angle < 202.5)
                        {
                            CurrentFrames.WalkLeft = PlayAnimation(
                                SpriteManager.Get("left"),
                                CurrentFrames.WalkLeft,
                                FrameCounts.Left,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 202.5 && angle < 247.5)
                        {
                            CurrentFrames.UpLeft = PlayAnimation(
                                SpriteManager.Get("upLeft"),
                                CurrentFrames.UpLeft,
                                FrameCounts.UpLeft,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 247.5 && angle < 292.5)
                        {
                            CurrentFrames.WalkUp = PlayAnimation(
                                SpriteManager.Get("backward"),
                                CurrentFrames.WalkUp,
                                FrameCounts.Up,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
                        }
                        else if (angle >= 292.5 && angle < 337.5)
                        {
                            CurrentFrames.UpRight = PlayAnimation(
                                SpriteManager.Get("upRight"),
                                CurrentFrames.UpRight,
                                FrameCounts.UpRight,
                                Settings.FrameWidth,
                                Settings.FrameHeight,
                                SpriteImage);
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
                if (Settings.LastPlayed.TryGetValue(fileName, out DateTime lastTime))
                {
                    if ((DateTime.Now - lastTime).TotalSeconds < delaySeconds)
                        return;
                }
            }

            try
            {
                SoundPlayer sp = new SoundPlayer(path);
                sp.Play();
                Settings.LastPlayed[fileName] = DateTime.Now;
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
            AnimationStates.IsClick = !AnimationStates.IsClick;
            if (AnimationStates.IsClick)
            {
                PlaySound("mambo.wav");
            }
        }

        private void SpriteImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!AnimationStates.IsIntro && !AnimationStates.IsWalking)
            {
                AnimationStates.IsHover = true;
            }
            if(!AnimationStates.IsIntro && !AnimationStates.IsWalking &&!AnimationStates.IsSleeping &&!AnimationStates.IsClick && !MouseSettings.FollowCursor)
            {
                PlaySound("hover.wav",3);
            }
        }

        private void SpriteImage_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!AnimationStates.IsIntro)
            {
                AnimationStates.IsHover = false;
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
            AnimationStates.IsDragging = true;
            DragMove();
            AnimationStates.IsDragging = false;
            MouseSettings.FollowCursor = !MouseSettings.FollowCursor;
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
            AnimationStates.IsSleeping = false;  
        }
        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            if (!AnimationStates.IsSleeping)
            {
                GC.Collect();   
                AnimationStates.IsSleeping = true;
            }
        }
        private void LeftHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            AnimationStates.IsFiring_Left = true;

        }
        private void RightHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            AnimationStates.IsFiring_Right = true;
 
        }
        private void TopHotspot_Click(object sender, MouseButtonEventArgs e)
        {
            ResetIdleTimer();
            CurrentFrames.Pat = 0;

            AnimationStates.IsPat = true;
            PlayAnimationIdle();
        }

    }



}
