using System;
using System.Collections.Generic;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public class Collection {
	public string Name;
	public int BeatmapCount;

	public List<string> Beatmaps = new();
}
