using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class Button {
	public Texture2D Texture;
	public Vector2 Pos;
	public Color Color;
	public string Name;

	public Button(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) {
		Pos = pos;
		Name = name;
		Color = color;
		Texture = new(graphics, width, height);
		SetColor(Color);
	}

	public virtual void SetPos(Vector2 pos) {
		Pos = pos;
	}

	public void SetColor(Color color) {
		Color = color;
		var colors = new Color[Texture.Height * Texture.Width];
		Array.Fill(colors, Color);
		Texture.SetData(colors);
	}

	public virtual void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Texture, Pos, Color);
	}
}
