using System.Collections.Generic;

namespace Rythmify.Core.Databases;

public class Collection {
	public string Name;
	public int BeatmapCount;

	public List<string> Beatmaps = new();
}
