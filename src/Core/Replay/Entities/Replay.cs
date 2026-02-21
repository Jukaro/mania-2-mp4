using System;
using System.Collections.Generic;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Replay;

public class ReplayData {
	public GameMode GameMode;
	public int GameVersion;
	public string BeatmapMD5;
	public string PlayerName;
	public string ReplayMD5;
	public short Nb300s;
	public short Nb100s;
	public short Nb50s;
	public short NbMax300s;
	public short Nb200s;
	public short NbMiss;
	public int Score;
	public short MaxCombo;
	public bool FullCombo;
	public int Mods;
	public string LifeBar;
	public DateTime TimeStamp;
	public long ReplayTimeStamp;
	public int CompressedReplayLength; // in bytes
	public List<Input> Inputs;
	public int LaneCount;
	public int TotalKeyPresses;
	public long ScoreID;
	public int SizeInBytes;

	// Used to store the offset between the beginning of the beatmap and the beginning of the replay
	public int StartDelay;

	public byte[] CompressedInputs;

	public string FilePath;

	public double PerformancePoints;
	public float Ratio;
	public float Accuracy;

	public int ScoreDifference;

	public ReplayData(int laneCount) {
		Inputs = new List<Input>();
		LaneCount = laneCount;
		StartDelay = -1;
	}

	public override string ToString() {
		string str;

		str = $"Gamemode: {GameMode}\n";
		str += $"GameVersion: {GameVersion}\n";
		str += $"BeatmapMD5: {BeatmapMD5}\n";
		str += $"PlayerName: {PlayerName}\n";
		str += $"ReplayMD5: {ReplayMD5}\n";
		str += $"Nb300s: {Nb300s}\n";
		str += $"Nb100s: {Nb100s}\n";
		str += $"Nb50s: {Nb50s}\n";
		str += $"NbMax300s: {NbMax300s}\n";
		str += $"Nb200s: {Nb200s}\n";
		str += $"NbMiss: {NbMiss}\n";
		str += $"Score: {Score}\n";
		str += $"MaxCombo: {MaxCombo}\n";
		str += $"FullCombo: {FullCombo}\n";
		str += $"Mods: {Mods}\n";
		str += $"LifeBar: {LifeBar}\n";
		str += $"TimeStamp: {TimeStamp}\n";
		str += $"CompressedReplayLength: {CompressedReplayLength}\n";
		str += $"LaneCount: {LaneCount}\n";
		str += $"TotalKeyPresses: {TotalKeyPresses}\n";
		str += $"ScoreID: {ScoreID}\n";
		str += $"PerformancePoints: {PerformancePoints}\n";
		str += $"Ratio: {Ratio}";

		return str;
	}

	public void DebugPrintAllInputs() {
		int currentLaneCount;
		int lastLaneCount = int.MaxValue;
		int backwardsCounter = TotalKeyPresses;
		string str;

		Logger.LogDebug($"TotalKeyPresses: {TotalKeyPresses}");
		for (int i = Inputs.Count - 1; i >= 0; i--) {
			currentLaneCount = Inputs[i].GetNbKeys();
			if (i > 0)
				lastLaneCount = Inputs[i - 1].GetNbKeys();
			str = $", Counter: {backwardsCounter}";
			if (i > 0 && currentLaneCount > lastLaneCount)
				backwardsCounter -= currentLaneCount - lastLaneCount;
			else if (i > 0 && currentLaneCount == lastLaneCount && Inputs[i].Keys != Inputs[i - 1].Keys)
				backwardsCounter -= currentLaneCount;
			else
				str = "";
			Logger.LogDebug($"[{i}]:\t{Inputs[i].ToString(LaneCount)}{str}");
		}
	}
}
