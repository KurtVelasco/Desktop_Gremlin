using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Gremlin
{
    public static class Settings
    {
        public static int SpriteColumn { get; set; } = 10;
        public static int FrameRate { get; set; } = 31;
        public static string StartingChar { get; set; } = "Haru";
        public static double FollowRadius { get; set; } = 150.0;
        public static int FrameWidth { get; set; } = 200;
        public static int FrameHeight { get; set; } = 200;
        public static int Ammo { get; set; } = 5;

        public static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
    }
    public static class MouseSettings
    {
        public static bool FollowCursor { get; set; } = false;
        public static System.Drawing.Point LastMousePosition { get; set; }
        public static double FollowSpeed { get; set; } = 5.0;
        public static double MouseX { get; set; }
        public static double MouseY { get; set; }
        public static double Speed { get; set; } = 10.0;
    }
}
