using System.Collections.Generic;
using System.IO;

namespace Rythmify.Core;

public class SkinData {
	public string BasePath;
	public SkinGeneralSection General = new();
	public SkinColorsSection Colors = new();
	public SkinFontsSection Fonts = new();
	public SkinCatchTheBeatSection CatchTheBeat = new();
	public Dictionary<int, SkinManiaSection> Mania = new();

	public string GetFilePath(string filePath) {
		if (Path.HasExtension(filePath)) return Path.Combine(BasePath, filePath);
		return Path.Combine(BasePath, filePath + ".png");
	}
}
