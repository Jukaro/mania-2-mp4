using System.Collections.Generic;
using Rythmify.Core.Beatmap;
using Rythmify.Core.BeatmapDB;
using Rythmify.Core.Replay;
using System.IO;
using System.Linq;
using System;

public class BeatmapWithScores {
	public BeatmapData Beatmap = null;
	public BeatmapDataFromDatabase BeatmapDBInfo;
	public List<ReplayData> Replays;
	public string TexturePath;
	public string AudioPath;

	public BeatmapWithScores(BeatmapDataFromDatabase beatmapDBInfo) {
		BeatmapDBInfo = beatmapDBInfo;
		Replays = new();
		GetTexturePath();
	}

	private void GetTexturePath() {
		string folderPath = "E:/osu maps de giga ultra mort/" + BeatmapDBInfo.FolderName.Trim() + "/";
		string filePath = folderPath + BeatmapDBInfo.Filename;
		AudioPath = folderPath + BeatmapDBInfo.AudioFilename;
		// try {
		// 	Beatmap = BeatmapParser.Parse(filePath);
		// } catch (Exception e) {
		// 	Logger.LogDebug($"Couldn't load the map at: {filePath}");
		// }
		string[] lines = File.ReadAllLines(filePath);
		// Logger.LogDebug($"filePath: {filePath}");
		for (int j = 0; j < lines.Count(); j++) {
			if (lines[j] == "//Background and Video events") {
				j++;
				string[] splitted_line = lines[j].Split(',');
				if (splitted_line.Length == 1) {
					TexturePath = "";
					break;
				}
				while (splitted_line[0]!= "0" && splitted_line[1] != "0") {
					j++;
					splitted_line = lines[j].Split(',');
				}
				TexturePath = folderPath + splitted_line[2].Trim('\"');
				break;
			}
		}
	}

	public void AddReplay(ReplayData replay) {
		Replays.Add(replay);
	}

	public void LoadBeatmap() {
		string folderPath = "E:/osu maps de giga ultra mort/" + BeatmapDBInfo.FolderName + "/";
		string filePath = folderPath + BeatmapDBInfo.Filename;
		Beatmap = BeatmapParser.Parse(filePath);
	}
}
