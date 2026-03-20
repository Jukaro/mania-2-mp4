using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rythmify.Core;

namespace Mania2mp4.Controls;

public class OsuReplayControl : TemplatedControl {
	private readonly Stopwatch _stopwatch = new();

	private Stopwatch _myTimer = new();
	private int _framesCounter = 0;

	public static readonly DirectProperty<OsuReplayControl, OsuReplay> OsuReplayProperty =
		AvaloniaProperty.RegisterDirect<OsuReplayControl, OsuReplay>(
			nameof(OsuReplay),
			o => o.OsuReplay,
			(o, v) => o.OsuReplay = v
		);

	private OsuReplay _osuReplay;

	public OsuReplay OsuReplay {
		get { return _osuReplay; }
		set {
			SetAndRaise(OsuReplayProperty, ref _osuReplay, value);
			_stopwatch.Restart();
		}
	}

	public OsuReplayControl() {
		DataContext = Services.ServiceProvider.GetRequiredService<OsuReplayViewModel>();
		Task.Run(Init);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);
		Bind(OsuReplayProperty, new Binding("OsuReplay"));
	}

	private void Init() {
		while (Bounds.Width == 0 && Bounds.Height == 0);
		// _osuReplayController = new(Bounds);

		_myTimer.Start();
	}

	public override void Render(DrawingContext context) {
		double elapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;

		// var watch = new Stopwatch();
		// watch.Start();
		_osuReplay?.Update(elapsedMilliseconds, 1.0);
		_osuReplay?.Render(context);
		// watch.Stop();
		// Logger.LogDebug($"Generated frame in {watch.ElapsedMilliseconds}ms");

		// if (_myTimer.Elapsed.TotalMilliseconds < 1000)
		// 	_framesCounter++;
		// else {
		// 	Logger.LogDebug($"fps: {_framesCounter}");
		// 	_framesCounter = 0;
		// 	_myTimer.Restart();
		// }

		Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Send);
		_stopwatch.Restart();
	}
}
