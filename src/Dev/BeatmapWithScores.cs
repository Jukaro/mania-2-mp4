using System.Collections.Generic;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using System.IO;
using System.Linq;
using System;

public class BeatmapWithScores {
	public BeatmapData Beatmap = null;
	public BeatmapDataFromDatabase BeatmapDBInfo = null;
	public List<ReplayData> Replays = new();
	public string TexturePath = null;
	public string AudioPath;
	public string FolderPath;
	public string FilePath;

	public BeatmapWithScores(BeatmapDataFromDatabase beatmapDBInfo) {
		BeatmapDBInfo = beatmapDBInfo;
		FolderPath = "C:/Users/shiro/AppData/Local/osu!/Songs/" + BeatmapDBInfo.FolderName.Trim() + "/";
		FilePath = FolderPath + BeatmapDBInfo.Filename;
		AudioPath = FolderPath + BeatmapDBInfo.AudioFilename;
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
		string folderPath = "C:/Users/shiro/AppData/Local/osu!/Songs/" + BeatmapDBInfo.FolderName + "/";
		string filePath = folderPath + BeatmapDBInfo.Filename;
		Beatmap = BeatmapParser.Parse(filePath);
	}
}
