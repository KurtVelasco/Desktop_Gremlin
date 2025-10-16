using System.Windows.Media.Imaging;
using System.Windows;
namespace Desktop_Gremlin
{
    public class AnimationPlayer
    {
        public static int PlayAnimation(string sheetName, int currentFrame, int frameCount, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
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
    }
}
