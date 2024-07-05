using System.IO;
using System.Linq;

namespace Rythmify.Core;

public static class SkinParser {
	public static void Parse(string skinPath) {
		IniParser skinIniParser = new(Path.Combine(skinPath, "skin.ini"));

		SkinData skin = new()
		{
			General = skinIniParser.GetSection("General").Bind<SkinGeneralSection>(),
			Colors = skinIniParser.GetSection("Colours").Bind<SkinColorsSection>(),
			Fonts = skinIniParser.GetSection("Fonts").Bind<SkinFontsSection>(),
			Mania = skinIniParser.GetSections("Mania").ConvertAll(section => section.Bind<SkinManiaSection>()).ToDictionary(section => section.Keys)
		};
	}
}
