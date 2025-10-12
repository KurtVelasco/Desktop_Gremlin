public static class Settings
{
    public static int SpriteColumn { get; set; } = 0;
    public static int FrameRate { get; set; } = 0;
    public static string StartingChar { get; set; } = "";
    public static double FollowRadius { get; set; } = 0;
    public static int FrameWidth { get; set; } = 0;
    public static int FrameHeight { get; set; } = 0;
    public static int MinInterval { get; set; } = 0;
    public static int MaxInterval { get; set; } = 0;
    public static int MoveDistance { get; set; } = 0;
    public static bool AllowRandomness { get; set; } = false;
    public static bool AllowGravity { get; set; } = false;
    public static int SleepTime { get; set; } = 0;
    public static int Ammo { get; set; } = 0;
    public static int CurrentAmmo { get; set; } = 0;
    public static bool FootStepSounds { get; set; } = false;    
    public static string CurrentIdle { get; set; } = "idle";
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

