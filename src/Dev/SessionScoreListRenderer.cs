using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Mania2mp4.Displays;

public class SessionScoreListRenderer {
	public int ScoreCountPerScreen { get; set; } = 40;
	public int RenderWidth { get; set; } = 800;

	public void RenderScoresList(List<SessionScoreDisplay> displays, DataTemplate dataTemplate, DateTime date) {
		Task.Run(async () => {
			string sessionRendersDirectory = "SessionRenders";
			if (!Directory.Exists(sessionRendersDirectory))
				Directory.CreateDirectory(sessionRendersDirectory);

			string dateStr = date.ToShortDateString().Replace('/', '-');
			string sessionDirectory = Path.Combine(sessionRendersDirectory, dateStr);
			if (!Directory.Exists(sessionDirectory))
				Directory.CreateDirectory(sessionDirectory);

			while (displays.Any(d => d.BeatmapDisplay.Background == null)) await Task.Delay(500);

			Dispatcher.UIThread.Post(() => {
				int scoreCountPerScreen = ScoreCountPerScreen;
				int currentPart = 0;
				int currentStartIndex = 0;
				int currentEndIndex = scoreCountPerScreen;
				const int SESSION_SCORE_TEMPLATE_HEIGHT = 46;

				while (currentStartIndex < displays.Count) {
					int height = displays.Take(new Range(currentStartIndex, currentEndIndex)).Count() * SESSION_SCORE_TEMPLATE_HEIGHT;
					Size rootSize = new Size(RenderWidth, height);

					var root = new StackPanel();

					foreach (var display in displays.Take(new Range(currentStartIndex, currentEndIndex))) {
						root.Children.Add(new ContentPresenter() {
							Content = display,
							ContentTemplate = dataTemplate,
							Width = rootSize.Width
						});
					}

					string path = Path.Combine(sessionDirectory, $"{dateStr}-part{currentPart + 1}.png");
					ControlRendering.Render(root, rootSize, path);

					currentPart++;
					currentStartIndex = currentEndIndex;
					currentEndIndex = (currentPart + 1) * scoreCountPerScreen;
				}
			});
		});
	}
}
