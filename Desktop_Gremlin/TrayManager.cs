using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

public class TrayManager
{
    private readonly Window _window;  // Reference to main window
    private readonly dynamic _context; // dynamic reference for accessing variables
    private NotifyIcon _trayIcon;
    private DispatcherTimer _closeTimer;

    public TrayManager(Window window)
    {
        _window = window;
    }

    public void SetupTrayIcon()
    {
        _trayIcon = new NotifyIcon();

        if (File.Exists("ico.ico"))
            _trayIcon.Icon = new Icon("ico.ico");
        else
            _trayIcon.Icon = SystemIcons.Application;

        _trayIcon.Visible = true;
        _trayIcon.Text = "Gremlin";

        var menu = new ContextMenuStrip();
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Reappear", null, (s, e) => ResetApp());
        menu.Items.Add("Close", null, (s, e) => CloseApp());
        _trayIcon.ContextMenuStrip = menu;
    }

    private void ResetApp()
    {
        _trayIcon.Visible = false;
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(exePath);
        System.Windows.Application.Current.Shutdown();
    }

    private void CloseApp()
    {

        _closeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / Settings.FrameRate)
        };

        _closeTimer.Tick += (s, e) =>
        {
            try
            {
                _context.CurrentFrames.Outro = _context.AnimationPlayer.PlayAnimation(
                    "outro",
                    _context.CurrentFrames.Outro,
                    _context.FrameCounts.Outro,
                    _context.SpriteImage
                );

                if (_context.CurrentFrames.Outro == 0)
                {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            }
        };

        _closeTimer.Start();
    }
}
