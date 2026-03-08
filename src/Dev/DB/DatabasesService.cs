using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.UI;

namespace Mania2mp4.Models;

public partial class DatabasesService : ObservableObject {
	public BeatmapDB BeatmapDB { get; private set; }
	public ScoreDB ScoreDB { get; private set; }
	public CollectionDB CollectionDB { get; private set; }
	public SessionList SessionList { get; private set; }
	public ThumbnailsDB ThumbnailsDB { get; private set; }

	private string _databasesFolder = "Databases";

	public event EventHandler DatabasesInitialized;

	public DatabasesService() {
		Directory.CreateDirectory(_databasesFolder);

		TryInit();
		InitThumbnailDatabase();
	}

	public void TryInit() {
		if (Paths.OsuDirectoryPath == null || Paths.OsuSongsDirectoryPath == null) {
			Logger.LogInfo($"[DatabasesService] One or more of the paths are not set");
			return;
		}

		Task.Run(Init);
	}

	private async Task Init() {
		await Task.Delay(500);

		try {
			BeatmapDB = BeatmapDBParser.Parse(Path.Combine(Paths.OsuDirectoryPath, "osu!.db"));
			ScoreDB = ScoreDBParser.Parse(Path.Combine(Paths.OsuDirectoryPath, "scores.db"), BeatmapDB);
			CollectionDB = CollectionDBParser.Parse(Path.Combine(Paths.OsuDirectoryPath, "collection.db"));

			SessionList = new SessionList(ScoreDB);

			DatabasesInitialized?.Invoke(this, EventArgs.Empty);

			Paths.Save();
		} catch (Exception e) {
			Logger.LogError($"[DatabasesService] Error: {e.Message}\nStacktrace: {e.StackTrace}");
		}
	}

	public Bitmap GetThumbnailFromDB(BeatmapWithScores beatmap) {
		return ThumbnailsDB.GetThumbnail(beatmap);
	}

	public void SaveThumbnailDatabase() {
		ThumbnailsDBWriter.Write(ThumbnailsDB, Path.Combine(_databasesFolder, "thumbnails.db"));
	}

	private void InitThumbnailDatabase() {
		ThumbnailsDB = new();
		string thumbnailsDBPath = Path.Combine(_databasesFolder, "thumbnails.db");
		if (File.Exists(thumbnailsDBPath))
			ThumbnailsDB = ThumbnailsDBParser.Parse(thumbnailsDBPath);
	}
}
