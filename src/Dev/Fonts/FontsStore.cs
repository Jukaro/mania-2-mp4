using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;

public static class FontsStore {
	public static FontSystem FontSystem;

	public static SpriteFont Arial;
	public static SpriteFontBase Arial2;

	public static void InitFonts() {
		FontSystem = new FontSystem();

		FontSystem.AddFont(File.ReadAllBytes("../../../src/Dev/Assets/Fonts/arial.ttf"));
		Arial2 = FontSystem.GetFont(20);
	}
}
