using Avalonia.Media;
using Avalonia.Threading;

public static class BrushColors {
	public static SolidColorBrush NoDiffBrush;
	public static SolidColorBrush PositiveDiffBrush;
	public static SolidColorBrush NegativeDiffBrush;

	static BrushColors() {
		Dispatcher.UIThread.Post(() => {
			NoDiffBrush = new SolidColorBrush(new Color(255, 255, 255, 100));
			PositiveDiffBrush = new SolidColorBrush(new Color(255, 100, 255, 100));
			NegativeDiffBrush = new SolidColorBrush(new Color(255, 255, 100, 100));
		});
	}
}
