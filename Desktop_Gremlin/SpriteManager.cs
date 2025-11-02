using System;
using System.Windows.Media.Imaging;
using System.IO;

namespace Desktop_Gremlin
{
    public static class SpriteManager
    {
        public static BitmapImage Get(string animationName)
        {
            BitmapImage sheet = null;
            animationName = animationName.ToLower();

            string fileName = GetFileName(animationName);
            if (fileName == null)
            {
                return null;
            }
            sheet = LoadSprite(Settings.StartingChar, fileName);
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
                case "firel":
                    return "fireL.png";
                case "firer":
                    return "fireR.png";
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
                case "walkl":
                    return "walkL.png";
                case "walkr":
                    return "walkR.png";
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
                default:
                    return null;
            }
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
}
