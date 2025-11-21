using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Desktop_Gremlin
{
    /// <summary>
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window
    {
        private DispatcherTimer _jumpTimer;
        public FullScreen()
        {
            InitializeComponent();
            InitializeAnimation();  

        }
        private int PlayAnimation(string sheetName,string action, int currentFrame, int frameCount, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
        {
            BitmapImage sheet = SpriteManager.Get(sheetName,action);

            if (sheet == null)
            {
                return currentFrame;
            }
            int x = (currentFrame % Settings.SpriteColumn) * Settings.FrameWidthJs;
            int y = (currentFrame / Settings.SpriteColumn) * Settings.FrameHeightJs;

            if (x + Settings.FrameWidthJs > sheet.PixelWidth || y + Settings.FrameHeightJs > sheet.PixelHeight)
            {
                return currentFrame;
            }

            targetImage.Source = new CroppedBitmap(sheet, new Int32Rect(x, y, Settings.FrameWidthJs, Settings.FrameHeightJs));

            return (currentFrame + 1) % frameCount;
        }
        public void InitializeAnimation()
        {
            //_jumpTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate) };
            //_jumpTimer.Tick += (s, e) =>
            //{
            //   CurrentFrames.JumpScare = PlayAnimation("jumpscare","Action", CurrentFrames.JumpScare, 
            //       FrameCounts.JumpScare, 
            //       SpriteImage);
            //    if (CurrentFrames.JumpScare <= 0)
            //    {
            //        _jumpTimer.Stop();
            //        this.Close();
            //    }
            //};  
            //_jumpTimer.Start();
        }





    }
}
