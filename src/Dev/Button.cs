using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rythmify.UI;

public class Button {
	public Texture2D Texture;
	public Vector2 BasePos; // position without ScrollY
	public Vector2 RealPos; // position with ScrollY
	public int ScrollY;
	public Color Color;
	public string Name;

	public Button(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) {
		ScrollY = 0;
		BasePos = pos;
		RealPos = pos;
		Name = name;
		Color = color;
		Texture = new(graphics, width, height);
		SetColor(Color);
	}

	public virtual void SetBasePos(Vector2 pos) {
		BasePos = pos;
		RealPos = new(BasePos.X, BasePos.Y + ScrollY);
	}

	public virtual void SetScrollY(int scrollAmount) {
		ScrollY += scrollAmount;
		RealPos = new(BasePos.X, BasePos.Y + ScrollY);
	}

	public void SetColor(Color color) {
		Color = color;
		var colors = new Color[Texture.Height * Texture.Width];
		Array.Fill(colors, Color);
		Texture.SetData(colors);
	}

	public virtual void Update() {

	}

	public virtual void UpdateScroll() {

	}

	public virtual void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Texture, RealPos, Color);
	}

	public bool isMouseOver() {
		if (Mouse.GetState().X > RealPos.X && Mouse.GetState().X < RealPos.X + Texture.Width
			&& Mouse.GetState().Y > RealPos.Y && Mouse.GetState().Y < RealPos.Y + Texture.Height)
			return true;
		return false;
	}

	// public virtual void Render(SpriteBatch spriteBatch, Vector2 pos) {
	// 	spriteBatch.Draw(Texture, pos, Color);
	// }
}
