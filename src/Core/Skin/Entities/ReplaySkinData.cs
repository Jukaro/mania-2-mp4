using System.IO;

namespace Rythmify.Core;

public class ReplaySkinData {
	public SkinData Data;
	public int LaneCount;

	public ReplaySkinData(SkinData data, int laneCount) {
		Data = data;
		LaneCount = laneCount;
	}

	public SkinManiaSection ManiaSection => Data.Mania[LaneCount];

	public string GetFilePath(string filePath) {
		if (Path.HasExtension(filePath)) return Path.Combine(Data.BasePath, filePath);

		return Path.Combine(Data.BasePath, filePath + ".png");
	}
}
