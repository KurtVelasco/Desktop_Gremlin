using Desktop_Gremlin;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace Mambo
{
    internal class SpriteManagerComp
    {
        public static int PlayAnimation(string sheetName, string actionType, int currentFrame, int frameCount, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
        {
            BitmapImage sheet = SpriteManagerComp.Get(sheetName, actionType);
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
            if (frameCount <= 0)
            {
                Gremlin.ErrorClose($"Error Animation: {sheetName} action: {actionType} has invalid frame count", "Animation Error", true);
            }
            return (currentFrame + 1) % frameCount;
        }
        public static int PlayEffect(string sheetName, string actionType, int currentFrame, int frameCount, System.Windows.Controls.Image targetImage, bool PlayOnce = false)
        {
            BitmapImage sheet = SpriteManagerComp.Get(sheetName, actionType);
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
            if (frameCount <= 0)
            {
                Gremlin.ErrorClose($"Error Animation: {sheetName} action: {actionType} has invalid frame count", "Animation Error", true);
            }
            return (currentFrame + 1) % frameCount;
        }
        public static BitmapImage Get(string animationName, string actionType)
        {
            BitmapImage sheet = null;
            string fileName = GetFileName(animationName);
            if (fileName == null)
            {
                Gremlin.ErrorClose("Error Animation: " + animationName + " is missing", "Animation Missing", false);
                return null;
            }
            sheet = LoadSprite(Settings.CompanionChar, fileName, actionType);
            return sheet;
        }
        private static string GetFileName(string animationName)
        {
            switch (animationName.ToLower())
            {
                case "idle":
                    return "idle.png";
                case "idle2":
                    return "idle2.png";
                case "intro":
                    return "intro.png";
                case "runleft":
                    return "runLeft.png";
                case "runright":
                    return "runRight.png";
                case "runup":
                    return "runUp.png";
                case "rundown":
                    return "runDown.png";
                case "outro":
                    return "outro.png";
                case "grab":
                    return "grab.png";
                case "runidle":
                    return "runIdle.png";
                case "click":
                    return "click.png";
                case "hover":
                    return "hover.png";
                case "sleep":
                    return "sleep.png";
                case "fireleft":
                    return "fireLeft.png";
                case "fireright":
                    return "fireRight.png";
                case "reload":
                    return "reload.png";
                case "pat":
                    return "pat.png";
                case "upleft":
                    return "upLeft.png";
                case "upright":
                    return "upRight.png";
                case "downleft":
                    return "downLeft.png";
                case "downright":
                    return "downRight.png";
                case "walkleft":
                    return "walkLeft.png";
                case "walkright":
                    return "walkRight.png";
                case "walkdown":
                    return "walkDown.png";
                case "walkup":
                    return "walkUp.png";
                case "emote1":
                    return "emote1.png";
                case "emote2":
                    return "emote2.png";
                case "emote3":
                    return "emote3.png";
                case "emote4":
                    return "emote4.png";
                case "jumpscare":
                    return "jumpScare.png";
                case "poof":
                    return "poof.png";
                default:
                    return null;
            }
        }
        private static BitmapImage LoadSprite(string filefolder, string fileName, string action, string rootFolder = "Gremlins")
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "SpriteSheet", rootFolder, filefolder, action, fileName);
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
}
