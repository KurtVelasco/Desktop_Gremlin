using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
    
        public static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
        private DateTime _nextRandomActionTime = DateTime.Now.AddSeconds(10);

        private Random _rng = new Random();


        private MediaPlayer _walkLoopPlayer;

        private bool _isWalkingSoundPlaying = false;
        private bool _wasIdleLastFrame = false;

        

        private DispatcherTimer _masterTimer;
        private DispatcherTimer _idleTimer;
        private DispatcherTimer _activeRandomMoveTimer;

        private bool _isClosed = false;
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
            InitializeAnimations();
            InitializeTimers();
            SetupTrayIcon();    
            MediaManager.PlaySound("intro.wav");
        }
        public void InitializeTimers()
        {
            _idleTimer = new DispatcherTimer();
            _idleTimer.Interval = TimeSpan.FromSeconds(Settings.SleepTime);
            _idleTimer.Start();


            _walkLoopPlayer = new MediaPlayer();
            _walkLoopPlayer.Open(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", Settings.StartingChar, "steps.wav")));
            _walkLoopPlayer.Volume = 1; 
            _walkLoopPlayer.MediaEnded += (s, e) =>
            {
                _walkLoopPlayer.Position = TimeSpan.Zero;
                _walkLoopPlayer.Play(); 
            };
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
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {        
                if (AnimationStates.GetState("Idle"))
                {
                    switch (Settings.CurrendIdle)
                    {
                        case 0:
                            CurrentFrames.Idle = PlayAnimation("idle", CurrentFrames.Idle,
                        FrameCounts.Idle, SpriteImage);
                            break;
                        case 1:
                            CurrentFrames.Idle2 = PlayAnimation("idle2", CurrentFrames.Idle2,
                        FrameCounts.Idle2, SpriteImage);
                            break;
                    }
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
            };      
            _masterTimer.Start();     
        }
        
        private void SpriteImage_RightClick(object sender, MouseButtonEventArgs e)
        {
            CurrentFrames.Click = 0;
            AnimationStates.UnlockState();
            AnimationStates.SetState("Click");
            MediaManager.PlaySound("mambo.wav");
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            DragMove();           
        }
           
      
        private readonly Window _window;
        private NotifyIcon _trayIcon;
        private DispatcherTimer _closeTimer;
        public void SetupTrayIcon()
        {
            _trayIcon = new NotifyIcon();

            if (File.Exists("ico.ico"))
            {
                _trayIcon.Icon = new Icon("ico.ico");
            }
            else
            {
                _trayIcon.Icon = SystemIcons.Application;
            }

            _trayIcon.Visible = true;
            _trayIcon.Text = "Gremlin";

            var menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Reappear", null, (s, e) => ResetApp());
            _trayIcon.ContextMenuStrip = menu;
        }

        private void ResetApp()
        {
            CurrentFrames.Outro = 0;
            _isClosed = false;  
            AnimationStates.SetState("Outro");
            MediaManager.PlaySound("outro.wav");    

        }

    }
}

