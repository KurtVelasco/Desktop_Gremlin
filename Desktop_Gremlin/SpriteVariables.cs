public static class FrameCounts
{
    public static int Intro { get; set; } = 0;
    public static int Idle2 { get; set; } = 0;
    public static int Idle { get; set; } = 0;
    public static int Left { get; set; } = 0;
    public static int Right { get; set; } = 0;
    public static int Up { get; set; } = 0;
    public static int Down { get; set; } = 0;
    public static int UpLeft { get; set; } = 0;
    public static int UpRight { get; set; } = 0;
    public static int DownLeft { get; set; } = 0;
    public static int DownRight { get; set; } = 0;
    public static int Outro { get; set; } = 0;
    public static int Grab { get; set; } = 0;
    public static int RunIdle { get; set; } = 0;
    public static int Click { get; set; } = 0;
    public static int Dance { get; set; } = 0;
    public static int Hover { get; set; } = 0;
    public static int Sleep { get; set; } = 0;
    public static int LeftFire { get; set; } = 0;
    public static int RightFire { get; set; } = 0;
    public static int Reload { get; set; } = 0;
    public static int Pat { get; set; } = 0;
    public static int WalkL { get; set; } = 0;
    public static int WalkR { get; set; } = 0;
    public static int WalkDown { get; set;} = 0;
    public static int WalkUp { get; set; } = 0;
    public static int Emote1 { get; set; } = 0;
    public static int Emote2 { get; set; } = 0;
    public static int Emote3 { get; set; } = 0;
    public static int Emote4 { get; set; } = 0;
    public static int JumpScare { get; set; } = 0;  

}
public static class CurrentFrames
{
    public static int LeftFire { get; set; } = 0;
    public static int RightFire { get; set; } = 0;
    public static int Intro { get; set; } = 0;
    public static int Idle { get; set; } = 0;
    public static int Idle2 { get; set; } = 0;  
    public static int Outro { get; set; } = 0;
    public static int Down { get; set; } = 0;
    public static int Up { get; set; } = 0;
    public static int Right { get; set; } = 0;
    public static int Left { get; set; } = 0;
    public static int UpLeft { get; set; } = 0;
    public static int UpRight { get; set; } = 0;
    public static int DownLeft { get; set; } = 0;
    public static int DownRight { get; set; } = 0;
    public static int Grab { get; set; } = 0;
    public static int WalkIdle { get; set; } = 0;
    public static int Click { get; set; } = 0;
    public static int Dance { get; set; } = 0;
    public static int Hover { get; set; } = 0;
    public static int Sleep { get; set; } = 0;
    public static int Reload { get; set; } = 0;
    public static int Pat { get; set; } = 0;
    public static int WalkLeft { get; set; } = 0;  
    public static int WalkRight { get; set; } = 0;
    public static int WalkUp { get; set; } = 0; 
    public static int WalkDown { get; set;} = 0;
    public static int Emote1 { get; set; } = 0;
    public static int Emote2 { get; set; } = 0;
    public static int Emote3 { get; set; } = 0;
    public static int Emote4 { get; set; } = 0;
    public static int JumpScare { get; set; } = 0;
}
public static class Settings
{
    public static int SpriteColumn { get; set; } = 0;
    public static int FrameRate { get; set; } = 0;
    public static string StartingChar { get; set; } = "";
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



