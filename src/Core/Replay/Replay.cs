using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework.Input;

namespace Rythmify.Core.Replay;

public enum GameMode {
	Standard = 0,
	Taiko = 1,
	CatchTheBeat = 2,
	Mania = 3
}

public enum Mods {
	None = 0,
	NoFail = 1,
	Easy = 2,
	TouchDevice = 4,
	Hidden = 8,
	HardRock = 16,
	SuddenDeath = 32,
	DoubleTime = 64,
	Relax = 128,
	HalfTime = 256,
	Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576
	Flashlight = 1024,
	Autoplay = 2048,
	SpunOut = 4096,
	Relax2 = 8192,    // Autopilot
	Perfect = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416
	KeyMod4 = 32768,
	KeyMod5 = 65536,
	KeyMod6 = 131072,
	KeyMod7 = 262144,
	KeyMod8 = 524288,
	FadeIn = 1048576,
	Random = 2097152,
	Cinema = 4194304,
	Target = 8388608,
	KeyMod9 = 16777216,
	KeyModCoop = 33554432,
	KeyMod1 = 67108864,
	KeyMod3 = 134217728,
	KeyMod2 = 268435456,
	ScoreV2 = 536870912,
	Mirror = 1073741824,
	KeyMod = KeyMod1 | KeyMod2 | KeyMod3 | KeyMod4 | KeyMod5 | KeyMod6 | KeyMod7 | KeyMod8 | KeyMod9 | KeyModCoop,
	FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
	ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
}

public class Input {
	public int Timestamp;
	public int Keys;
	public int HoldTime;

	public Input (int timestamp, int keys, int hold) {
		Timestamp = timestamp;
		Keys = keys;
		HoldTime = hold;
	}

	public int GetNbKeys() {
		int keyID = 1;
		int nbKeys = 0;

		for (int i = 0; i < 10; i++) {
			if ((Keys & keyID) != 0)
				nbKeys++;
			keyID *= 2;
		}
		return nbKeys;
	}

	public override string ToString() {
		return $"Keys: {Keys}, Timestamp: {Timestamp}, HoldTime: {HoldTime}";
	}

	public string ToString(int nbKeys) {
		string keysStr = "";
		int keyID = 1;

		for (int i = 0; i < nbKeys; i++) {
			keysStr += (Keys & keyID) == 0 ? "| " : "|X";
			keyID <<= 1;
		}
		keysStr += '|';

		return $"Keys: {keysStr}, Timestamp: {Timestamp}, HoldTime: {HoldTime}";
	}
}

public class Replay {

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
	public long TimeStamp;
	public int CompressedReplayLength; // in bytes
	public List<Input> Inputs;
	public int NbKeys;
	public int TotalKeyPresses;
	public long ScoreID;

	public Replay() {
		Inputs = new List<Input>();
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
		str += $"NbKeys: {NbKeys}\n";
		str += $"TotalKeyPresses: {TotalKeyPresses}\n";
		str += $"ScoreID: {ScoreID}";

		return str;
	}

	public void DebugPrintAllInputs() {
		int currentNbKeys;
		int lastNbKeys = int.MaxValue;
		int backwardsCounter = TotalKeyPresses;
		string str;

		Logger.LogDebug($"TotalKeyPresses: {TotalKeyPresses}");
		for (int i = Inputs.Count - 1; i >= 0; i--) {
			currentNbKeys = Inputs[i].GetNbKeys();
			if (i > 0)
				lastNbKeys = Inputs[i - 1].GetNbKeys();
			str = $", Counter: {backwardsCounter}";
			if (i > 0 && currentNbKeys > lastNbKeys)
				backwardsCounter -= currentNbKeys - lastNbKeys;
			else if (i > 0 && currentNbKeys == lastNbKeys && Inputs[i].Keys != Inputs[i - 1].Keys)
				backwardsCounter -= currentNbKeys;
			else
				str = "";
			Logger.LogDebug($"[{i}]:\t{Inputs[i].ToString(NbKeys)}{str}");
		}
	}
}
