using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.UI;
using SkiaSharp;

public static class BeatmapDisplayHelper
{
	private static string _folderPath = "Thumbnails";
	private static Bitmap _defaultBackground = null;

	public static void CreateThumbnailsFolder()
	{
		if (!Directory.Exists(_folderPath))
			Directory.CreateDirectory(_folderPath);
	}

	public static Bitmap GetBackground(BeatmapWithScores beatmap)
	{
		Bitmap background = null;

		beatmap.SetTexturePath();
		string texturePath = beatmap.TexturePath;
		if (!string.IsNullOrEmpty(texturePath) && File.Exists(texturePath))
			background = new Bitmap(texturePath);

		return background;
	}

	public static Bitmap GetSeasonalBackground()
	{
		Bitmap seasonalBackground = null;

		string seasonalBackgroundsFolder = Path.Combine(Paths.OsuDirectoryPath, "Data/bg");
		string[] seasonalBackgroundsPaths = Directory.GetFiles(seasonalBackgroundsFolder);

		Random random = new Random();
		seasonalBackground = new Bitmap(seasonalBackgroundsPaths[random.Next(seasonalBackgroundsPaths.Count())]);

		return seasonalBackground;
	}

	public static void GenerateThumbnail(BeatmapWithScores beatmap)
	{
		beatmap.SetTexturePath();
		if (beatmap.TexturePath == null || !File.Exists(beatmap.TexturePath))
			throw new FileLoadException($"Couldn't find a background to generate a thumbnail for the beatmap {beatmap.BeatmapDBInfo.SongTitle} [{beatmap.BeatmapDBInfo.Difficulty}]");

		string filePath = GetThumbnailFilePath(beatmap);

		if (!File.Exists(filePath))
		{
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

			using var fs = new FileStream(filePath, FileMode.Create);
			data.SaveTo(fs);

			fs.Close();

			bitmap.Dispose();
			ms.Dispose();
			skStream.Dispose();
			skBitmap.Dispose();
			resized.Dispose();
			image.Dispose();
			data.Dispose();
			fs.Dispose();
		}
	}

	public static MemoryStream GetThumbnail(BeatmapWithScores beatmap)
	{
		MemoryStream thumbnail = null;

		beatmap.SetTexturePath();
		if (beatmap.TexturePath == null || !File.Exists(beatmap.TexturePath))
			return thumbnail;

		string filePath = GetThumbnailFilePath(beatmap);

		using var fs = new FileStream(filePath, FileMode.Open);
		thumbnail = new();
		fs.CopyTo(thumbnail);

		return thumbnail;
	}

	public static bool HasThumbnail(BeatmapWithScores beatmap)
	{
		return File.Exists(GetThumbnailFilePath(beatmap));
	}

	public static string GetThumbnailFilePath(BeatmapWithScores beatmap)
	{
		return Path.Combine(_folderPath, beatmap.BeatmapDBInfo.BeatmapID.ToString() + "-" + Path.GetFileNameWithoutExtension(beatmap.TexturePath) + ".jpeg");
	}

	public static Bitmap GetOsuThumbnail(BeatmapWithScores beatmap) {
		Bitmap bg = null;

		string thumbnailBackgroundsFolder = Path.Combine(Paths.OsuDirectoryPath, "Data", "bt");
		string thumbnailPath = Path.Combine(thumbnailBackgroundsFolder, beatmap.BeatmapDBInfo.BeatmapID.ToString() + ".jpg");
		string alternateThumbnailPath = Path.Combine(thumbnailBackgroundsFolder, beatmap.BeatmapDBInfo.BeatmapID.ToString() + "l" + ".jpg");;

		if (File.Exists(thumbnailPath))
		{
			bg = new Bitmap(thumbnailPath);
		}
		else if (File.Exists(alternateThumbnailPath))
		{
			bg = new Bitmap(alternateThumbnailPath);
		}
		else
		{
			if (_defaultBackground == null)
			{
				_defaultBackground = Dispatcher.UIThread.Invoke(() =>
				{
					int width = 4;
					int height = 4;

					var bitmap = new RenderTargetBitmap(new Avalonia.PixelSize(width, height));
					using (var context = bitmap.CreateDrawingContext())
					{
						var brush = new SolidColorBrush(Colors.Black);
						context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
					}
					return bitmap;
				});
			}
			bg = _defaultBackground;
			// Logger.LogDebug($"not found: {thumbnailPath} or {alternateThumbnailPath}");
		}

		return bg;
	}
}
