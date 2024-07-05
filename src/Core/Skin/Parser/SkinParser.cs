using System.IO;
using System.Linq;

namespace Rythmify.Core;

public static class SkinParser {
	public static void Parse(string skinPath) {
		IniParser skinIniParser = new(Path.Combine(skinPath, "skin.ini"));

		SkinData skin = new()
		{
			General = skinIniParser.GetSection("General")?.Bind<SkinGeneralSection>() ?? new(),
			Colors = skinIniParser.GetSection("Colours")?.Bind<SkinColorsSection>() ?? new(),
			Fonts = skinIniParser.GetSection("Fonts")?.Bind<SkinFontsSection>() ?? new(),
			CatchTheBeat = skinIniParser.GetSection("CatchTheBeat")?.Bind<SkinCatchTheBeatSection>() ?? new(),
			Mania = skinIniParser.GetSections("Mania")?.ConvertAll(section => section.Bind<SkinManiaSection>()).ToDictionary(section => section.Keys) ?? new()
		};

		Logger.LogInfo(skin.General.ToString());
		Logger.LogInfo(skin.Colors.ToString());
	}
}
