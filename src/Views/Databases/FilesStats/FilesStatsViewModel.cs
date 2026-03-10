using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Rythmify.Core;
using Rythmify.UI;
using SkiaSharp;

namespace Mania2mp4.ViewModels;

public partial class FilesStatsViewModel : ViewModelBase {
	[ObservableProperty]
	private ISeries[] _fileSizeByExtensionColumns = [];

	[ObservableProperty]
	private List<string> _extensions = new();

	[ObservableProperty]
	private List<float> _extensionSizes = new();

	[ObservableProperty]
	private Dictionary<string, string> _extensionsAndSizes = new();

	[ObservableProperty]
	private Axis[] _xAxes;

	[ObservableProperty]
	private Axis[] _yAxes;

	public SolidColorPaint LegendTextPaint { get; } = new SolidColorPaint(SKColors.White);

	public FilesStatsViewModel() {
		XAxes = [CreateXAxis([])];
		YAxes = [CreateYAxis()];
	}

	public void UpdateFileSizeByExtensionColumns() {
		if (string.IsNullOrEmpty(Paths.OsuSongsDirectoryPath)) return;

		var sortedFileExtensionsBySize = GetSortedFileExtensionsBySize(Paths.OsuSongsDirectoryPath);
		var sortedGeneralFormatsBySize = GetSortedGeneralFormatsBySize(sortedFileExtensionsBySize);

		FileSizeByExtensionColumns = [
			new ColumnSeries<float> {
				YToolTipLabelFormatter = (point) => $"{point.Coordinate.PrimaryValue:F1} Go",
				Values = sortedGeneralFormatsBySize.Select(kv => kv.Value / 1024).ToArray(),
			}
		];

		string[] labels = sortedGeneralFormatsBySize.Select(kv => kv.Key).ToArray();
		XAxes = [CreateXAxis(labels)];

		Dictionary<string, string> temp = new();

		string[] sizeUnits = ["o", "Ko", "Mo", "Go", "To"];

		foreach (var entry in sortedFileExtensionsBySize) {
			float sizeInOctets = entry.Value * 1024 * 1024;
			float finalSize = sizeInOctets;
			int unitIndex = 0;

			while (finalSize > 1000) {
				finalSize /= 1024;
				unitIndex++;
			}

			temp.Add(entry.Key, $"{finalSize:F1}{sizeUnits[unitIndex]}");
		}

		ExtensionsAndSizes = temp;
		Logger.LogInfo($"[FileStats] {string.Join(", ", ExtensionsAndSizes.Select(kv => $"{kv.Key}: {kv.Value}"))}");
	}

	public void UpdateData() {
		Task.Run(async () => {
			UpdateFileSizeByExtensionColumns();
		});
	}

	private Axis CreateXAxis(string[] labels) {
		return new Axis() {
			Labels = labels,
			LabelsPaint = new SolidColorPaint(SKColors.White),
			SeparatorsPaint = new SolidColorPaint(SKColors.White),
			SeparatorsAtCenter = false,
			TicksPaint = new SolidColorPaint(SKColors.White),
			TicksAtCenter = true,
			MinStep = 1,
			ForceStepToMin = true
		};
	}

	private Axis CreateYAxis() {
		return new Axis() {
			Labeler = value => $"{value}Go",
			LabelsPaint = new SolidColorPaint(SKColors.White),
			SeparatorsPaint = new SolidColorPaint(SKColors.White),
			SeparatorsAtCenter = false,
			TicksPaint = new SolidColorPaint(SKColors.White),
			TicksAtCenter = false,
		};
	}

	private IOrderedEnumerable<KeyValuePair<string, float>> GetSortedFileExtensionsBySize(string folderPath) {
		string[] songsPaths = Directory.GetDirectories(folderPath);
		OrderedDictionary<string, float> sizeByFileExtension = new();

		foreach (string songPath in songsPaths) {
			var dirInfo = new DirectoryInfo(songPath);

			foreach (FileInfo file in dirInfo.GetFiles()) {
				float filesize = (float)file.Length / 1024 / 1024;
				string extension = file.Extension.ToLower();

				if (!sizeByFileExtension.ContainsKey(extension))
					sizeByFileExtension.Add(extension, filesize);
				else
					sizeByFileExtension[extension] += filesize;
			}
		}

		IOrderedEnumerable<KeyValuePair<string, float>> sortedDict = from entry in sizeByFileExtension orderby entry.Value descending select entry;
		return sortedDict;
	}

	private IOrderedEnumerable<KeyValuePair<string, float>> GetSortedGeneralFormatsBySize(IOrderedEnumerable<KeyValuePair<string, float>> sortedFileExtensionsBySize) {
		float totalSize = 0;

		Dictionary<string, float> sizeByFormat = new() {
			{ "Audio", 0 },
			{ "Images", 0 },
			{ "Videos", 0 },
			{ "Other", 0 },
		};

		string[] audioFormats = [".mp3", ".ogg", ".wav"];
		string[] imageFormats = [".jpg", ".png", ".jpeg"];
		string[] videoFormats = [".avi", ".flv", ".mp4", ".wmv"];

		List<string> keys = new();
		List<float> values = new();

		foreach (var entry in sortedFileExtensionsBySize) {
			if (audioFormats.Contains(entry.Key))
				sizeByFormat["Audio"] += entry.Value;
			else if (imageFormats.Contains(entry.Key))
				sizeByFormat["Images"] += entry.Value;
			else if (videoFormats.Contains(entry.Key))
				sizeByFormat["Videos"] += entry.Value;
			else
				sizeByFormat["Other"] += entry.Value;

			keys.Add(entry.Key);
			values.Add(entry.Value);

			totalSize += entry.Value;
		}

		Extensions = keys;
		ExtensionSizes = values;

		IOrderedEnumerable<KeyValuePair<string, float>> sortedDict = from entry in sizeByFormat orderby entry.Value descending select entry;
		return sortedDict;
	}
}
