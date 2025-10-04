using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Gremlin
{
    public static class FrameCounts
    {
        public static int Intro { get; set; } = 0;
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
        public static int WalkIdle { get; set; } = 0;
        public static int Click { get; set; } = 0;
        public static int Dance { get; set; } = 0;
        public static int Hover { get; set; } = 0;
        public static int Sleep { get; set; } = 0;
        public static int LeftFire { get; set; } = 0;
        public static int RightFire { get; set; } = 0;
        public static int Reload { get; set; } = 0;
        public static int Pat { get; set; } = 0;
    }
    public static class FireArms
    {
        public static int ReloadArm { get; set; }
    }
    public static class CurrentFrames
    {
        public static int LeftFire { get; set; } = 0;
        public static int RightFire { get; set; } = 0;
        public static int Intro { get; set; } = 0;
        public static int Idle { get; set; } = 0;
        public static int Outro { get; set; } = 0;
        public static int WalkDown { get; set; } = 0;
        public static int WalkUp { get; set; } = 0;
        public static int WalkRight { get; set; } = 0;
        public static int WalkLeft { get; set; } = 0;

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
    }
    public static class AnimationStates
    {
        // Some of the States are unused, I'll use them in the future
        // If I need more precision when it comes to state management

        public static bool IsIntro { get; set; } = true;
        public static bool ForwardAnimation { get; set; } = true;
        public static bool IsRandom { get; set; } = false;
        public static bool IsHover { get; set; } = false;
        public static bool IsIdle { get; set; } = true;
        public static bool IsWalking { get; set; } = false;
        public static bool IsDragging { get; set; } = false;
        public static bool IsWalkIdle { get; set; } = false;
        public static bool IsClick { get; set; } = false;
        public static bool IsSleeping { get; set; } = false;
        public static bool IsFiring_Left { get; set; } = false;
        public static bool IsFiring_Right { get; set; } = false;
        public static bool IsReload { get; set; } = false;
        public static bool IsPat { get; set; } = false;
    }
}
