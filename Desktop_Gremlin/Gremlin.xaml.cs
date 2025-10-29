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
        private int _clickCount = 0;    

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
                    CurrentFrames.Idle = PlayAnimation("idle", CurrentFrames.Idle,
                    FrameCounts.Idle, SpriteImage);                                             
                }             
                if (AnimationStates.GetState("Click"))
                {
                    CurrentFrames.Click = PlayAnimation("click",
                    CurrentFrames.Click,
                    FrameCounts.Click,SpriteImage);

                    if (CurrentFrames.Click == 0)
                    {
                        MediaManager.PlaySound("emote1.wav");
                        AnimationStates.UnlockState();
                        AnimationStates.ResetAllExceptIdle();
                    }
                }
                if (AnimationStates.GetState("Emote1"))
                {
                    AnimationStates.UnlockState();
                    AnimationStates.LockState();
                    CurrentFrames.Emote1 = PlayAnimation("emote1",
                    CurrentFrames.Emote1,
                    FrameCounts.Emote1, SpriteImage);

                    if (CurrentFrames.Emote1 == 0)
                    {
                        _clickCount = 0;
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
            if(_clickCount < 3)
            {
                _clickCount++;
                AnimationStates.SetState("Click");
                CurrentFrames.Idle = 0;
            }
            else
            {
                AnimationStates.UnlockState();
                AnimationStates.SetState("Emote1");
                MediaManager.PlaySound("mambo.wav");
                _clickCount = 0;
                CurrentFrames.Idle = 0;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            DragMove();           
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (_isClosed)
                return;

            _isClosed = true;

            CleanUpResources();
        }
        private void CleanUpResources()
        {
            try
            {
                _masterTimer?.Stop();
                _idleTimer?.Stop();
                _activeRandomMoveTimer?.Stop();

                _walkLoopPlayer?.Stop();
                _walkLoopPlayer?.Close();

                if (TRAY_ICON != null)
                {
                    TRAY_ICON.Visible = false;
                    TRAY_ICON.Dispose();
                    TRAY_ICON = null;
                }
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Cleanup failed: {ex.Message}");
            }
        }



    }
}

