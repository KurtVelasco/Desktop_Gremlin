using System;
using System.Collections.Generic;
using System.Linq;

public class AnimationStates
{
    private readonly Random _random = new Random();
    public bool IsLocked { get; private set; } = false;

    private readonly Dictionary<string, bool> _animationStates = new Dictionary<string, bool>()
    {
        { "Intro", true },
        { "Random", false },
        { "Hover", false },
        { "Idle", false },
        { "Outro", false },
        { "Walking", false },
        { "Grab", false },
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
        { "Emote4", false },
        { "FollowItem", false },
    };

    public void ResetAllExceptIdle()
    {
        foreach (var key in _animationStates.Keys.ToList())
        {
            _animationStates[key] = key.Equals("Idle", StringComparison.OrdinalIgnoreCase);
        }
        ChangeIdle();
    }

    public void ChangeIdle()
    {
        int idleState = _random.Next(0, 3);

        switch (idleState)
        {
            case 0:
                Settings.CurrendIdle = 0;
                break;
            case 1:
                Settings.CurrendIdle = 1;
                break;
            case 2:
                Settings.CurrendIdle = 0;
                break;
        }
    }

    public void PlayOutro()
    {
        foreach (var key in _animationStates.Keys.ToList())
        {
            _animationStates[key] = false;
        }
        _animationStates["Outro"] = true;
    }

    public void SetState(string stateName)
    {
        if (IsLocked)
            return;

        string normalized = stateName.Trim();

        if (!_animationStates.ContainsKey(normalized))
            return;

        foreach (var key in _animationStates.Keys.ToList())
            _animationStates[key] = false;

        _animationStates[normalized] = true;
    }

    public bool GetState(string stateName)
    {
        return _animationStates.TryGetValue(stateName, out bool value) && value;
    }

    public bool IsCompletelyIdle()
    {
        foreach (var kv in _animationStates)
        {
            if (!kv.Key.Equals("Idle", StringComparison.OrdinalIgnoreCase) && kv.Value)
                return false;
        }
        return true;
    }

    public void LockState() => IsLocked = true;
    public void UnlockState() => IsLocked = false;
}

