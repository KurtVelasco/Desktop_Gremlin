using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

public static class MediaManager
{
    private static Dictionary<string, DateTime> LastPlayed = new Dictionary<string, DateTime>();
    private static MediaPlayer player = new MediaPlayer();

    public static void PlaySound(string fileName, string startChar, double delaySeconds = 0, double volume = 1.0)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", startChar, fileName);
        if (!File.Exists(path)) return;
        if (delaySeconds > 0 &&
            LastPlayed.TryGetValue(fileName, out DateTime lastTime) &&
            (DateTime.Now - lastTime).TotalSeconds < delaySeconds)
        {
            return;
        }
        player.Open(new Uri(path));
        player.Volume = Settings.VolumeLevel;   
        player.Play();
        LastPlayed[fileName] = DateTime.Now;
    }
}


