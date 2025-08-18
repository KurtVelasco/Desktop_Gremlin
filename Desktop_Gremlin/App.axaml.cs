using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SharpHook;
using SharpHook.Data;

namespace Desktop_Gremlin;

public partial class App : Application
{
    public static IGlobalHook hook;
    
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new SpriteSheet();
        }

        base.OnFrameworkInitializationCompleted();
    }
}