using System.Collections.Generic;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using System.IO;
using System.Linq;
using System;
using Rythmify.UI;

public class BeatmapWithScores {
	public BeatmapData Beatmap = null;
	public BeatmapDataFromDB BeatmapDBInfo = null;
	public List<ReplayData> Replays = new();
	public string TexturePath = null;
	public string AudioPath;
	public string FolderPath;
	public string FilePath;

	public BeatmapWithScores(BeatmapDataFromDB beatmapDBInfo) {
		BeatmapDBInfo = beatmapDBInfo;
		FolderPath = Path.Combine(Paths.OsuSongsDirectoryPath, BeatmapDBInfo.FolderName.Trim() + "/");
		FilePath = Path.Combine(FolderPath, BeatmapDBInfo.Filename);
		AudioPath = Path.Combine(FolderPath, BeatmapDBInfo.AudioFilename);
	}

	public void SetTexturePath() {
		using (StreamReader sr = new StreamReader(FilePath)) {
			string line;

			while ((line = sr.ReadLine()) != null) {
				if (line == "//Background and Video events") {
					line = sr.ReadLine();
					if (!line.Contains(',')) {
						TexturePath = "";
						break;
					}
					string[] splitted_line = line.Split(',');
					while (splitted_line[0] != "0") {
						line = sr.ReadLine();
						splitted_line = line.Split(',');
					}
					TexturePath = FolderPath + splitted_line[2].Trim('\"');
					break;
				}
			}
		}
	}

	public void AddReplay(ReplayData replay) {
		Replays.Add(replay);
	}

	public void LoadBeatmap() {
		Beatmap = BeatmapParser.Parse(FilePath);
	}
}
