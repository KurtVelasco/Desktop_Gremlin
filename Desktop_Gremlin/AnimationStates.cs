using System;
using System.Collections.Generic;
using System.Linq;
public static class AnimationStates
{
    public static bool IsLocked { get; set; } = false;
    private static readonly Dictionary<string, bool> _animationStates = new Dictionary<string, bool>()
    {
        { "Intro", true },
        { "Random", false },
        { "Hover", false },
        { "Idle", false },
        { "Walking", false },
        { "Dragging", false },
        { "WalkIdle", false },
        { "Click", false },
        { "Sleeping", false },
        { "Firing_Left", false },
        { "Firing_Right", false },
        { "Reload", false },
        { "Pat", false },
        { "RandomMovement", false },
        { "Emote1", false },
        { "Emote2", false }
    };
    public static void ResetAllExceptIdle()
    {
        foreach (var key in _animationStates.Keys.ToList())
        {
            _animationStates[key] = key.Equals("Idle", StringComparison.OrdinalIgnoreCase);
        }
    }
    public static void SetState(string stateName)
    {
<<<<<<< Updated upstream
        if (IsLocked) return;
=======
        if (IsLocked)
        {
            return;
        } 

>>>>>>> Stashed changes
        string normalized = stateName.Trim();

        if (!_animationStates.ContainsKey(normalized))
        {
            return;
        }
        foreach (var key in _animationStates.Keys.ToList())
        {
            _animationStates[key] = false;
        }

        _animationStates[normalized] = true;
    }
    public static bool GetState(string stateName)
    {
        return _animationStates.TryGetValue(stateName, out bool value) && value;
    }
    public static bool IsCompletelyIdle()
    {
        foreach (var kv in _animationStates)
        {
            if (!kv.Key.Equals("Idle", StringComparison.OrdinalIgnoreCase) && kv.Value)
                return false;
        }
        return true;
    }
<<<<<<< Updated upstream
=======
    public static void LockState() => IsLocked = true;
    public static void UnlockState() => IsLocked = false;

>>>>>>> Stashed changes
}
