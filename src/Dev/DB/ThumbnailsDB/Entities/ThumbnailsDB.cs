using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Rythmify.Core.Beatmap;
using SkiaSharp;

namespace Rythmify.Core.Databases;

public class BeatmapThumbnail {
	public Bitmap Bitmap;
	public int UseCount;

	public BeatmapThumbnail(Bitmap bitmap) {
		Bitmap = bitmap;
	}
}

public class ThumbnailsDB {
	public int Version;
	public int ThumbnailsCount;
	public ConcurrentDictionary<string, string> MD5ToThumbnailID = new();

	public ConcurrentDictionary<string, BeatmapThumbnail> BeatmapThumbnails = new();
	private HashSet<string> _thumbnailsInFolder = new();
	private ManagedTaskPool _managedTaskPool = new(Environment.ProcessorCount / 2);
	private ConcurrentDictionary<string, object> _currentlyGeneratingThumbnails = new();
	private Bitmap _defaultBackground;

	private string _folderPath = "Thumbnails";

	public ThumbnailsDB() {
		_thumbnailsInFolder = Directory.GetFiles(_folderPath).ToHashSet();

		_defaultBackground = Dispatcher.UIThread.Invoke(() => {
			int width = 10;
			int height = 10;

			var bitmap = new RenderTargetBitmap(new PixelSize(width, height));
			using (var context = bitmap.CreateDrawingContext()) {
				var brush = new SolidColorBrush(Colors.Black);
				context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
			}
			return bitmap;
		});
	}

	public override string ToString() {
		string str;

		str = $"Version: {Version}\n";
		str += $"ThumbnailsCount: {ThumbnailsCount}\n";

		return str;
	}

	public Bitmap GetThumbnail(BeatmapWithScores beatmap) {
		string beatmapMD5 = beatmap.BeatmapDBInfo.BeatmapMD5;
		string thumbnailID;

		bool dbHasThumbnail = MD5ToThumbnailID.TryGetValue(beatmapMD5, out thumbnailID);
		bool thumbnailIsValid = thumbnailID == "NoTexture" || _thumbnailsInFolder.Contains(GetThumbnailPath(thumbnailID));

		if (!dbHasThumbnail || !thumbnailIsValid)
			UpdateThumbnailEntry(beatmap);

		if (MD5ToThumbnailID.TryGetValue(beatmapMD5, out thumbnailID)) {
			if (thumbnailID == "NoTexture") return _defaultBackground;

			if (!BeatmapThumbnails.ContainsKey(thumbnailID)) {
				var newBitmap = new Bitmap(GetThumbnailPath(thumbnailID));
				BeatmapThumbnails[thumbnailID] = new(newBitmap);
			}

			BeatmapThumbnails[thumbnailID].UseCount++;
			return BeatmapThumbnails[thumbnailID].Bitmap;
		}

		TryGenerateThumbnail(beatmap);
		return GetOriginalBackground(beatmap);
	}

	public void ReleaseThumbnail(string beatmapMD5) {
		if (!MD5ToThumbnailID.TryGetValue(beatmapMD5, out string thumbnailID)) return;

		if (BeatmapThumbnails.TryGetValue(thumbnailID, out var beatmapThumbnail)) {
			beatmapThumbnail.UseCount--;
			if (beatmapThumbnail.UseCount == 0) {
				BeatmapThumbnails.Remove(thumbnailID, out var toDispose);
				toDispose.Bitmap.Dispose();
			}
		}
	}

	private void UpdateThumbnailEntry(BeatmapWithScores beatmap) {
		string beatmapMD5 = beatmap.BeatmapDBInfo.BeatmapMD5;

		beatmap.SetTexturePath();
		bool hasValidBackground = beatmap.TexturePath != null && File.Exists(beatmap.TexturePath);

		if (!hasValidBackground) {
			MD5ToThumbnailID[beatmapMD5] = "NoTexture";
			return;
		}

		string thumbnailID = GetThumbnailID(beatmap);
		string thumbnailPath = GetThumbnailPath(thumbnailID);

		if (_thumbnailsInFolder.Contains(thumbnailPath)) {
			MD5ToThumbnailID[beatmapMD5] = thumbnailID;
			return;
		}

		MD5ToThumbnailID.Remove(beatmapMD5, out _);
	}

	private void TryGenerateThumbnail(BeatmapWithScores beatmap) {
		string thumbnailID = GetThumbnailID(beatmap);
		string thumbnailPath = GetThumbnailPath(thumbnailID);

		if ( _currentlyGeneratingThumbnails.TryAdd(thumbnailID, new object())) {
			_managedTaskPool.AddTaskToPool(async () => {
				GenerateThumbnail(beatmap);
				_thumbnailsInFolder.Add(thumbnailPath);
			});
		}
	}

	private Bitmap GetOriginalBackground(BeatmapWithScores beatmap) {
		return new Bitmap(beatmap.TexturePath);
	}

	private string GetThumbnailID(BeatmapWithScores beatmap) {
		BeatmapDataFromDB beatmapDBInfo = beatmap.BeatmapDBInfo;
		int beatmapSetID = beatmapDBInfo.BeatmapID;
		string beatmapGroupID = beatmapSetID <= 0 ? beatmapDBInfo.FolderName : beatmapSetID.ToString();
		string bgFilename = Path.GetFileNameWithoutExtension(beatmap.TexturePath);

		return beatmapGroupID + "-" + bgFilename;
	}

	private string GetThumbnailPath(string thumbnailID) {
		string filepath = thumbnailID + ".jpeg";
		return Path.Combine(_folderPath, filepath);
	}

	private void GenerateThumbnail(BeatmapWithScores beatmap) {
		string thumbnailID = GetThumbnailID(beatmap);
		string thumbnailPath = GetThumbnailPath(thumbnailID);

		if (File.Exists(thumbnailPath)) return;

		using var bitmap = new Bitmap(beatmap.TexturePath);
		using var ms = new MemoryStream();
		bitmap.Save(ms);
		ms.Seek(0, SeekOrigin.Begin);
		using var skStream = new SKManagedStream(ms);
		using var skBitmap = SKBitmap.Decode(skStream);

		int newWidth = 1000;
		float ratio = (float)newWidth / skBitmap.Width;
		int newHeight = (int)(skBitmap.Height * ratio);

		using var resized = skBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

		using var image = SKImage.FromBitmap(resized);
		using var data = image.Encode(SKEncodedImageFormat.Jpeg, 80);

		using var fs = new FileStream(thumbnailPath, FileMode.Create);
		data.SaveTo(fs);
		fs.Close();

		MD5ToThumbnailID[beatmap.BeatmapDBInfo.BeatmapMD5] = thumbnailID;
		_currentlyGeneratingThumbnails.Remove(thumbnailID, out _);
	}
}
