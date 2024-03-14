using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;

namespace Rythmify.UI;

public class SkinRenderer {
	public Texture2D NoteTexture;
	public Texture2D HitLineTexture;
	public Texture2D HoldNoteBodyTexture;
	public readonly Skin Skin;

	public SkinRenderer(Skin skin, GraphicsDevice graphicsDevice) {
		Skin = skin;

		NoteTexture = new(graphicsDevice, 150, 50);
		var colors = new Color[150 * 50];
		Array.Fill(colors, Color.White);
		NoteTexture.SetData(colors);

		HoldNoteBodyTexture = new(graphicsDevice, 150, 1);
		var colors2 = new Color[150 * 1];
		Array.Fill(colors2, Color.White);
		HoldNoteBodyTexture.SetData(colors2);

		HitLineTexture = new(graphicsDevice, 1, 4);
		var colors1 = new Color[1 * 4];
		Array.Fill(colors1, Color.Red);
		HitLineTexture.SetData(colors1);
	}
}
