using System.Collections.Generic;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using System.IO;
using System.Linq;
using System;
using Rythmify.UI;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Beatmap;

public class BeatmapWithScores {
	public BeatmapData Beatmap = null;
	public BeatmapDataFromDB BeatmapDBInfo { get; set; } = null;
	public List<ReplayData> Replays = new();
	public string TexturePath { get; set; } = null;
	public string AudioPath { get; set; }
	public string FolderPath;
	public string FilePath;
	public string PrintName;

	public IImage Background { get; set; }

	public string StarRating { get; set; }

	public BeatmapWithScores(BeatmapDataFromDB beatmapDBInfo) {
		BeatmapDBInfo = beatmapDBInfo;
		if (beatmapDBInfo.ManiaStarRating != null && beatmapDBInfo.ManiaStarRating.ContainsKey((int)Mods.None))
			StarRating = beatmapDBInfo.ManiaStarRating[0].ToString("F2") + "*";
		else
			StarRating = "";
		// FolderPath = Path.Combine(Paths.OsuSongsDirectoryPath, BeatmapDBInfo.FolderName.Trim() + "/");
		FolderPath = Paths.OsuSongsDirectoryPath + "/" + BeatmapDBInfo.FolderName.Trim() + "/";
		FilePath = Path.Combine(FolderPath, BeatmapDBInfo.Filename);
		AudioPath = Path.Combine(FolderPath, BeatmapDBInfo.AudioFilename);
		PrintName = $"{BeatmapDBInfo.SongTitle} [{BeatmapDBInfo.Difficulty}]";
	}

	public BeatmapWithScores(BeatmapData beatmapData, string folderPath, string filename) {
		Beatmap = beatmapData;
		FolderPath = folderPath;
		FilePath = Path.Combine(FolderPath, filename);
		AudioPath = Path.Combine(FolderPath, Beatmap.GeneralData.AudioFilename);
	}

	public void SetTexturePath() {
		if (!File.Exists(FilePath))
			return;

		using StreamReader sr = new StreamReader(FilePath);
		string line;

		while ((line = sr.ReadLine()) != null && line != "[Events]");

		while ((line = sr.ReadLine()) != null && !string.IsNullOrEmpty(line)) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());
			var eventType = BeatmapEvent.TryGetEventType(parameters[0]);

			if (eventType == BeatmapEventType.Background) {
				TexturePath = Path.Combine(FolderPath, parameters[2].Trim('\"'));
				break;
			}
		}
	}

	public void SetBackgroundBitmap() {
		if (Background != null) return;
		SetTexturePath();
		if (!string.IsNullOrEmpty(TexturePath) && File.Exists(TexturePath))
			Background = new Bitmap(TexturePath);
	}

	public void AddReplay(ReplayData replay) {
		Replays.Add(replay);
	}

	public void LoadBeatmap() {
		try {
			Beatmap = BeatmapParser.Parse(FilePath);
		} catch (Exception e) {
			Logger.LogError($"[BeatmapWithScores] Couldn't parse the beatmap ({PrintName}): {e.Message}");
			Beatmap = null;
		}
	}
}
