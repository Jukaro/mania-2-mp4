using System.Collections.Generic;

namespace Rythmify.Core;

public class SkinData {
	public SkinGeneralSection General = new();
	public SkinColorsSection Colors = new();
	public SkinFontsSection Fonts = new();
	public SkinCatchTheBeatSection CatchTheBeat = new();
	public Dictionary<int, SkinManiaSection> Mania = new();

	public int HitPosition = 0;
}
