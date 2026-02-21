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
}
