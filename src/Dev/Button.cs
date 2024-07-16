using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rythmify.UI;

public class Button {
	public Texture2D Texture;
	public Vector2 RelativePos; // position without ScrollY
	public Vector2 AbsolutePos; // position with ScrollY
	public int ScrollY;
	public Color Color;
	public string Name;

	public Button(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) {
		ScrollY = 0;
		RelativePos = pos;
		AbsolutePos = pos;
		Name = name;
		Color = color;
		Texture = new(graphics, width, height);
		SetColor(Color);
	}

	public virtual void SetRelativePos(Vector2 pos) {
		RelativePos = pos;
		AbsolutePos = new(RelativePos.X, RelativePos.Y + ScrollY);
	}

	public virtual void SetScrollY(int scrollAmount) {
		ScrollY += scrollAmount;
		AbsolutePos = new(RelativePos.X, RelativePos.Y + ScrollY);
	}

	public void SetColor(Color color) {
		Color = color;
		var colors = new Color[Texture.Height * Texture.Width];
		Array.Fill(colors, Color);
		Texture.SetData(colors);
	}

	public void SetGradientAsColor(GradientList gdList) {
		var colors = new Color[Texture.Height * Texture.Width];
		for (int y = 0; y < Texture.Height; y++) {
			for (int x = 0; x < Texture.Width; x++) {
				double index = x + y;
				int step = 255;
				colors[y * Texture.Width + x] = gdList.GetColor(index, step);
			}
		}
		Texture.SetData(colors);
	}

	public virtual void Update() {

	}

	public virtual void UpdateScroll() {

	}

	public virtual void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Texture, AbsolutePos, Color);
	}

	public virtual void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		if (mode == 1)
			spriteBatch.Draw(Texture, AbsolutePos, new Rectangle(0, 0, Texture.Width, (int)(limitY - AbsolutePos.Y)), Color);
		else
			spriteBatch.Draw(Texture, new Vector2(AbsolutePos.X, limitY), new Rectangle(0, (int)(limitY - AbsolutePos.Y), Texture.Width, (int)(Texture.Height - (limitY - AbsolutePos.Y))), Color);
	}

	public bool isMouseOver() {
		if (Mouse.GetState().X > AbsolutePos.X && Mouse.GetState().X < AbsolutePos.X + Texture.Width
			&& Mouse.GetState().Y > AbsolutePos.Y && Mouse.GetState().Y < AbsolutePos.Y + Texture.Height)
			return true;
		return false;
	}
}
