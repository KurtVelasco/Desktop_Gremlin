using System;
using System.Collections.Generic;
using System.Linq;

public static class AnimationStates
{
    public static bool IsLocked { get; set; } = false;
    private static readonly Dictionary<string, bool> STATES = new Dictionary<string, bool>()
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
        { "Emote2", false },
        { "Emote3", false },
        { "Emote4", false }
    };

    public static void LockState() => IsLocked = true;
    public static void UnlockState() => IsLocked = false;
    public static void ResetAllExceptIdle()
    {
        foreach (var key in STATES.Keys.ToList())
        {
            STATES[key] = key.Equals("Idle", StringComparison.OrdinalIgnoreCase);
        }
    }
    public static void SetState(string stateName)
    {
        if (IsLocked) {
            return;
        } 
        string normalized = stateName.Trim();

        if (!STATES.ContainsKey(normalized))
        {
            return;
        }
        foreach (var key in STATES.Keys.ToList())
            STATES[key] = false;

        STATES[normalized] = true;
    }
    public static bool GetState(string stateName)
    {
        return STATES.TryGetValue(stateName, out bool value) && value;
    }
    public static bool IsCompletelyIdle()
    {
        foreach (var kv in STATES)
        {
            if (!kv.Key.Equals("Idle", StringComparison.OrdinalIgnoreCase) && kv.Value)
                return false;
        }
        return true;
    }

}
