using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Rendering.Composition;
using System;

namespace Mania2mp4;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new CompositionOptions() {
                UseRegionDirtyRectClipping = true
            })
            .WithInterFont()
            .LogToTrace()
            // .UseReactiveUI()
            .UseSkia()
            .With(new SkiaOptions() {
                MaxGpuResourceSizeBytes = 256000000
            });
}
