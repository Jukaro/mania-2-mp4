using System.IO;

namespace Rythmify.Core.Replay;

public static partial class ReplayWriter {
	public static void Write(string filePath, ReplayData replay) {
		if (File.Exists(filePath)) {
			FileStream erase = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
			erase.Close();
		}
		FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);

		Writer.WriteByte((byte)replay.GameMode, fs);
		Writer.WriteInt(replay.GameVersion, fs);
		Writer.WriteString(replay.BeatmapMD5, fs);
		Writer.WriteString(replay.PlayerName, fs);
		Writer.WriteString(replay.ReplayMD5, fs);
		Writer.WriteShort(replay.Nb300s, fs);
		Writer.WriteShort(replay.Nb100s, fs);
		Writer.WriteShort(replay.Nb50s, fs);
		Writer.WriteShort(replay.NbMax300s, fs);
		Writer.WriteShort(replay.Nb200s, fs);
		Writer.WriteShort(replay.NbMiss, fs);
		Writer.WriteInt(replay.Score, fs);
		Writer.WriteShort(replay.MaxCombo, fs);
		Writer.WriteBool(replay.FullCombo, fs);
		Writer.WriteInt(replay.Mods, fs);
		Writer.WriteString(replay.LifeBar, fs);
		Writer.WriteLong(replay.TimeStamp.ToUniversalTime().Ticks, fs);
		Writer.WriteInt(replay.CompressedReplayLength, fs);

		// write inputs
		fs.Write(replay.CompressedInputs);

		Writer.WriteLong(replay.ScoreID, fs);

		fs.Close();
	}
}
