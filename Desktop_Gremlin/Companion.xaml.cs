using Desktop_Gremlin;
using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
namespace Mambo
{
    /// <summary>
    /// Interaction logic for Companion.xaml
    /// </summary>
    public partial class Companion : Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        public Window MainGremlin { get; set; }
        public struct POINT
        {
            public int X;
            public int Y;
        }
        private DispatcherTimer _masterTimer;
        private DispatcherTimer _effectTimer;
        private AnimationStates GremlinState = new AnimationStates();
        private CurrentFrames CurrentFrames = new CurrentFrames();  
        private FrameCounts FrameCounts = new FrameCounts();
        private const int MoveDelayMs = 500;
        private DateTime _lastStillTime = DateTime.Now;
        private bool _isMoving = false;
        public Companion()
        {
            InitializeComponent();
            SpriteImage.Source = new CroppedBitmap();
            FrameCounts = ConfigManager.LoadConfigChar(Settings.CompanionChar);
            GremlinState.LockState();
            InitializeAnimations();
            this.Width = this.Width * Settings.CompanionScale;
            this.Height = this.Height * Settings.CompanionScale;
            SpriteImage.Width = SpriteImage.Width * Settings.CompanionScale;
            SpriteImage.Height = SpriteImage.Height * Settings.CompanionScale;
            IntroEffect.Width = IntroEffect.Width * Settings.CompanionScale;
            IntroEffect.Height = IntroEffect.Height * Settings.CompanionScale;
        }
        private int PlayAnimationIfActive(string stateName, string folder, int currentFrame, int frameCount, bool resetOnEnd)
        {
            if (!GremlinState.GetState(stateName))
            {
                return currentFrame;
            };
            currentFrame = SpriteManagerComp.PlayAnimation(stateName, folder, currentFrame, frameCount, SpriteImage);

            if (resetOnEnd && currentFrame == 0)
            {
                GremlinState.UnlockState();
                GremlinState.ResetAllExceptIdle();
            }
            return currentFrame;
        }
        private int OverlayEffect(string stateName, string folder, int currentFrame, int frameCount, bool resetOnEnd)
        {
            currentFrame = SpriteManagerComp.PlayEffect(stateName, folder, currentFrame, frameCount, IntroEffect);
            if (resetOnEnd && currentFrame == 0)
            {
                IntroEffect.Source = null;
                _effectTimer.Stop();    
            }
            return currentFrame;    
        }
        private void InitializeAnimations()
        {
            _masterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _masterTimer.Tick += (s, e) =>
            {              
                CurrentFrames.Idle = PlayAnimationIfActive("Idle", "Actions", CurrentFrames.Idle, FrameCounts.Idle, false);
                CurrentFrames.Grab = PlayAnimationIfActive("Grab", "Actions", CurrentFrames.Grab, FrameCounts.Grab, false);
                CurrentFrames.Intro = PlayAnimationIfActive("Intro", "Actions", CurrentFrames.Intro, FrameCounts.Intro, true);
                if (!GremlinState.GetState("Grab"))
                {
                    FollowMainGremlin();
                }
            };
            _masterTimer.Start();

            _effectTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            _effectTimer.Tick += (s, e) =>
            {
                CurrentFrames.Poof = OverlayEffect("Poof", "Effects", CurrentFrames.Poof, FrameCounts.Poof, true);               
            };
            _effectTimer.Start();
        }
        private void FollowMainGremlin()
        {
            if (MainGremlin == null)
            {
                return;
            }
            double halfW = SpriteImage.ActualWidth > 0 ? SpriteImage.ActualWidth / 2.0 : Settings.FrameWidth / 2.0;
            double halfH = SpriteImage.ActualHeight > 0 ? SpriteImage.ActualHeight / 2.0 : Settings.FrameHeight / 2.0;

            var compCenter = new Point(this.Left + halfW, this.Top + halfH);

            double mainHalfW = MainGremlin.Width / 2.0;
            double mainHalfH = MainGremlin.Height / 2.0;
            var mainCenter = new Point(MainGremlin.Left + mainHalfW, MainGremlin.Top + mainHalfH);

            double dx = mainCenter.X - compCenter.X;
            double dy = mainCenter.Y - compCenter.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            bool shouldMove = distance - SpriteImage.Width / 2 > Settings.FollowRadius;

            if (!shouldMove)
            {
                _isMoving = false;
                _lastStillTime = DateTime.Now;
                CurrentFrames.WalkIdle = SpriteManagerComp.PlayAnimation("runIdle","Actions",CurrentFrames.WalkIdle,FrameCounts.RunIdle,SpriteImage);
                return;
            }
            if (!_isMoving)
            {
                double msStill = (DateTime.Now - _lastStillTime).TotalMilliseconds;

                if (msStill < MoveDelayMs)
                {
                    return;
                }

                _isMoving = true;
            }
            double nx = dx / distance;
            double ny = dy / distance;
            double step = Math.Min(MouseSettings.Speed, distance - Settings.FollowRadius);

            this.Left += nx * step;
            this.Top += ny * step;

            UpdateDirectionAnimation(nx, ny);
        }
        private void UpdateDirectionAnimation(double nx, double ny)
        {
            double angle = Math.Atan2(ny, nx) * 180.0 / Math.PI;
            if (angle < 0) angle += 360;

            if (angle >= 337.5 || angle < 22.5)
            {
                CurrentFrames.Right = SpriteManagerComp.PlayAnimation("runRight", "Run", CurrentFrames.Right, FrameCounts.Right, SpriteImage);
            }            
            else if (angle >= 22.5 && angle < 67.5)
            {
                CurrentFrames.DownRight = SpriteManagerComp.PlayAnimation("downRight", "Run", CurrentFrames.DownRight, FrameCounts.DownRight, SpriteImage);
            }              
            else if (angle >= 67.5 && angle < 112.5)
            {
                CurrentFrames.Down = SpriteManagerComp.PlayAnimation("runDown", "Run", CurrentFrames.Down, FrameCounts.Down, SpriteImage);
            }                
            else if (angle >= 112.5 && angle < 157.5)
            {
                CurrentFrames.DownLeft = SpriteManagerComp.PlayAnimation("downLeft", "Run", CurrentFrames.DownLeft, FrameCounts.DownLeft, SpriteImage);
            }                
            else if (angle >= 157.5 && angle < 202.5)
            {
                CurrentFrames.Left = SpriteManagerComp.PlayAnimation("runLeft", "Run", CurrentFrames.Left, FrameCounts.Left, SpriteImage);
            }    
            else if (angle >= 202.5 && angle < 247.5)
            {
                CurrentFrames.UpLeft = SpriteManagerComp.PlayAnimation("upLeft", "Run", CurrentFrames.UpLeft, FrameCounts.UpLeft, SpriteImage);
            }
            else if (angle >= 247.5 && angle < 292.5)
            {
                CurrentFrames.Up = SpriteManagerComp.PlayAnimation("runUp", "Run", CurrentFrames.Up, FrameCounts.Up, SpriteImage);
            }               
            else if (angle >= 292.5 && angle < 337.5)
            {
                CurrentFrames.UpRight = SpriteManagerComp.PlayAnimation("upRight", "Run", CurrentFrames.UpRight, FrameCounts.UpRight, SpriteImage);
            }
                
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GremlinState.UnlockState();
            GremlinState.SetState("Grab");
            MediaManager.PlaySound("grab.wav");
            DragMove();
            GremlinState.SetState("Idle");
        }

    }
}
