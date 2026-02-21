using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;

public static class ChartsHelper {
	public static void CreatePieChart(ObservableCollection<PieSeries<int>> series, Dictionary<string, int> values, int hoverPushout) {
		foreach (var entry in values) {
			series.Add(new PieSeries<int> { Name = entry.Key.ToString(), Values = [entry.Value], HoverPushout = hoverPushout });
		}
	}

	public static void UpdatePieChart(ObservableCollection<PieSeries<int>> series, Dictionary<string, int> values) {
		foreach (var serie in series) {
			serie.Values = [values[serie.Name]];
		}
	}
}
