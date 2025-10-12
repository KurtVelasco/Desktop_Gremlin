using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;

namespace Desktop_Gremlin
{
    public static class SpriteManager
    {
        private static readonly Dictionary<string, BitmapImage> Cache = new Dictionary<string, BitmapImage>();
        private static readonly HashSet<string> AlwaysCached = new HashSet<string>()
        {
            //"idle", "left", "right", "forward", "backward", "widle"
        };

        private static string _currentExtra = null;

        public static BitmapImage Get(string animationName)
        {
            animationName = animationName.ToLower();

            if (Cache.TryGetValue(animationName, out var sheet))
            {
                return sheet;
            }

            string fileName = GetFileName(animationName);
            if (fileName == null)
            {
                //NormalError("Unknown animation requested: " + animationName, "Sprite Error");
                return null;
            }

            sheet = LoadSprite(Settings.StartingChar, fileName);
            //if (sheet != null)
            //{
            //    Cache[animationName] = sheet;
            //    if (!AlwaysCached.Contains(animationName))
            //    {
            //        if (_currentExtra != null && _currentExtra != animationName)
            //        {
            //            Cache.Remove(_currentExtra);
            //        }
            //        _currentExtra = animationName;
            //    }
            //}
            return sheet;

        }
        private static string GetFileName(string animationName)
        {
            switch (animationName.ToLower())
            {
                case "idle":
                    return "idle.png";
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
                default:
                    return null;
            }
        }
        public static void PruneCache()
        {
            var keysToRemove = new List<string>();

            foreach (var key in Cache.Keys)
            {
                if (!AlwaysCached.Contains(key.ToLower()))
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                Cache.Remove(key);
            }
        }
        public static void ClearCache()
        {
            foreach (var img in Cache.Values)
            {
                img.StreamSource?.Dispose();
            }
            Cache.Clear();
            _currentExtra = null;
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
