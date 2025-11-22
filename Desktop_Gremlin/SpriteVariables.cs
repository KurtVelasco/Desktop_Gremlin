public class FrameCounts
{
    public int Intro { get; set; } = 0;
    public int Idle2 { get; set; } = 0;
    public int Idle { get; set; } = 0;
    public int Left { get; set; } = 0;
    public int Right { get; set; } = 0;
    public int Up { get; set; } = 0;
    public int Down { get; set; } = 0;
    public int UpLeft { get; set; } = 0;
    public int UpRight { get; set; } = 0;
    public int DownLeft { get; set; } = 0;
    public int DownRight { get; set; } = 0;
    public int Outro { get; set; } = 0;
    public int Grab { get; set; } = 0;
    public int RunIdle { get; set; } = 0;
    public int Click { get; set; } = 0;
    public int Dance { get; set; } = 0;
    public int Hover { get; set; } = 0;
    public int Sleep { get; set; } = 0;
    public int LeftFire { get; set; } = 0;
    public int RightFire { get; set; } = 0;
    public int Reload { get; set; } = 0;
    public int Pat { get; set; } = 0;
    public int WalkL { get; set; } = 0;
    public int WalkR { get; set; } = 0;
    public int WalkDown { get; set; } = 0;
    public int WalkUp { get; set; } = 0;
    public int Emote1 { get; set; } = 0;
    public int Emote2 { get; set; } = 0;
    public int Emote3 { get; set; } = 0;
    public int Emote4 { get; set; } = 0;
    public int JumpScare { get; set; } = 0;

    public int Poof { get; set; } = 0;
}

public class CurrentFrames
{
    public int LeftFire { get; set; } = 0;
    public int RightFire { get; set; } = 0;
    public int Intro { get; set; } = 0;
    public int Idle { get; set; } = 0;
    public int Idle2 { get; set; } = 0;  
    public int Outro { get; set; } = 0;
    public int Down { get; set; } = 0;
    public int Up { get; set; } = 0;
    public int Right { get; set; } = 0;
    public int Left { get; set; } = 0;
    public int UpLeft { get; set; } = 0;
    public int UpRight { get; set; } = 0;
    public int DownLeft { get; set; } = 0;
    public int DownRight { get; set; } = 0;
    public int Grab { get; set; } = 0;
    public int WalkIdle { get; set; } = 0;
    public int Click { get; set; } = 0;
    public int Dance { get; set; } = 0;
    public int Hover { get; set; } = 0;
    public int Sleep { get; set; } = 0;
    public int Reload { get; set; } = 0;
    public int Pat { get; set; } = 0;
    public int WalkLeft { get; set; } = 0;  
    public int WalkRight { get; set; } = 0;
    public int WalkUp { get; set; } = 0; 
    public int WalkDown { get; set;} = 0;
    public int Emote1 { get; set; } = 0;
    public int Emote2 { get; set; } = 0;
    public int Emote3 { get; set; } = 0;
    public int Emote4 { get; set; } = 0;
    public int JumpScare { get; set; } = 0;
    public int Poof { get; set; } = 0;
}
public static class Settings
{
    public static int SpriteColumn { get; set; } = 0;
    public static int FrameRate { get; set; } = 0;
    public static string StartingChar { get; set; } = "";
    public static string CompanionChar { get; set; } = "";
    public static double FollowRadius { get; set; } = 0;
    public static int FrameWidth { get; set; } = 0;
    public static int FrameHeight { get; set; } = 0;
    public static int FrameWidthJs { get; set; } = 0;
    public static int FrameHeightJs { get; set; } = 0;
    public static int RandomMinInterval { get; set; } = 0;
    public static int RandomMaxInterval { get; set; } = 0;
    public static int MoveDistance { get; set; } = 0;
    public static bool AllowRandomness { get; set; } = false;
    public static bool AllowGravity { get; set; } = false;
    public static int SleepTime { get; set; } = 0;
    public static int Ammo { get; set; } = 0;
    public static int CurrentAmmo { get; set; } = 0;
    public static int CurrendIdle { get; set; } = 0;
    public static bool FootStepSounds { get; set; } = false;
    public static bool AllowColoredHotSpot { get; set; } = false;
    public static bool ShowTaskBar { get; set; } = false;
    public static double SpriteSize { get; set; } = 1.0;
    public static bool FakeTransparent { get; set; } = false;   
    public static double ItemAcceleration { get; set;} = 1.0;
    public static int MaxItemAcceleration { get; set; } = 0;
    public static double CurrentItemAcceleration { get; set; } = 0;
    public static int FoodItemGetSize { get; set; } = 0;
    public static int ItemWidth { get; set; } = 0;
    public static int ItemHeight { get; set; } = 0;
    public static bool AllowErrorMessages { get; set; } = false;  
    public static double CompanionScale { get; set; } = 0;
    public static bool ManualReize { get; set; } = false;
    public static bool ForceCenter { get; set; } = false;
    public static bool EnableMinSize { get; set; } = false;
    public static double VolumeLevel { get; set; } = 1.0;   
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



