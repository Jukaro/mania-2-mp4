using System.IO;
using System.Linq;

namespace Rythmify.Core;

public static class SkinParser {
	public static SkinData Parse(string skinPath) {
		IniParser skinIniParser = new(Path.Combine(skinPath, "skin.ini"));

		SkinData skin = new()
		{
			BasePath = skinPath,
			General = skinIniParser.GetSection("General")?.As<SkinGeneralSection>() ?? new(),
			Colors = skinIniParser.GetSection("Colours")?.As<SkinColorsSection>() ?? new(),
			Fonts = skinIniParser.GetSection("Fonts")?.As<SkinFontsSection>() ?? new(),
			CatchTheBeat = skinIniParser.GetSection("CatchTheBeat")?.As<SkinCatchTheBeatSection>() ?? new(),
			Mania = skinIniParser.GetSections("Mania")?.ConvertAll(section => section.As<SkinManiaSection>()).ToDictionary(section => section.Keys) ?? new()
		};

		Logger.LogInfo(skin.General.ToString());
		Logger.LogInfo(skin.Colors.ToString());
		Logger.LogInfo(skin.Fonts.ToString());
		Logger.LogInfo(skin.CatchTheBeat.ToString());
		skin.Mania.Values.Select(section => section.ToString()).ToList().ForEach(Logger.LogInfo);

		return skin;
	}
}
