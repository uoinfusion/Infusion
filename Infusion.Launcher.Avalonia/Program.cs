using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Infusion.Proxy;
using Microsoft.Extensions.CommandLineUtils;

namespace Infusion.Launcher.Avalonia
{
    //class Program
    //{
    //    public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

    //    public static AppBuilder BuildAvaloniaApp()
    //        => AppBuilder.Configure<App>()
    //            .UseReactiveUI()
    //            .UsePlatformDetect()
    //            .LogToDebug();

    //    private static void AppMain(Application app, string[] args)
    //    {
    //        var cmdLine = new CommandLineApplication();
    //        cmdLine.Name = "Infusion Launcher";

    //        var rootPath = cmdLine.Option("-r|--rootPath", "Root directory path for Infusion launcher.", CommandOptionType.SingleValue);
    //        cmdLine.OnExecute(() =>
    //        {
    //            if (rootPath.HasValue())
    //                PathUtilities.SetRootPath(rootPath.Value());
    //            return 0;
    //        });

    //        cmdLine.Execute(args);

    //        app.Run(new MainWindow(new NullLauncher()));
    //    }
    //}
}
