using System;
using System.Collections.Generic;
using System.Media;
using System.IO;
public static class MediaManager
{
    private static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
    public static void PlaySound(string fileName, double delaySeconds = 0)
    {
        string path = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Sounds", Settings.StartingChar, fileName);

        if (!File.Exists(path))
            return;

        if (delaySeconds > 0)
        {
            if (LastPlayed.TryGetValue(fileName, out DateTime lastTime))
            {
                if ((DateTime.Now - lastTime).TotalSeconds < delaySeconds)
                    return;
            }
        }
        using (SoundPlayer sp = new SoundPlayer(path))
        {
            sp.Play();
        }
    }
}

