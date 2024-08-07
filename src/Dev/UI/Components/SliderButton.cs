using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class SliderButton : Button {
	private Action<double> _onSliderChange;
	private Color[] _baseTextureData;
	private int _radius;
	private double _cursorPos;
	private double _value;
	private double _min = -20;
	private double _max = 20;

	public SliderButton(GraphicsDevice graphics, Vector2 pos, string name, double min, double max, int cursorRadius, Action<double> onSliderChange, Visuals visuals) : base(graphics, pos, name, visuals) {
		_onSliderChange = onSliderChange;
		_baseTextureData = new Color[Visuals.Texture.Height * Visuals.Texture.Width];
		Visuals.Texture.GetData(_baseTextureData);
		SetOnClickHold(HandleSlider);
		DrawCursor(Visuals.Texture.Width / 2);
		_radius = cursorRadius;
		_min = min;
		_max = max;
	}

	private void DrawCursor(double PosX) {
		int width = Visuals.Texture.Width;
		int height = Visuals.Texture.Height;
		var colors = new Color[height * width];
		Array.Copy(_baseTextureData, colors, height * width);

		for (int y = 0; y < height; y++) {
			if (y == height / 2)
				continue;
			for (int x = 0; x < width; x++) {
				colors[y * width + x] = Color.Transparent;
			}
		}
		DrawCircleOnColorArray(_baseTextureData, colors, _radius, (int)PosX - _radius, height / 2 - _radius);
		Color[] mdr = new Color[height * width];
		Array.Fill(mdr, Color.Transparent);
		DrawCircleOnColorArray(mdr, colors, _radius - 2, (int)PosX - _radius + 2, height / 2 - _radius + 2);
		SetColor(colors);
	}

	private void DrawCircleOnColorArray(Color[] source, Color[] dest, int radius, int PosX, int PosY) {
		int diameter = radius * 2;
		float di = 1.0f / radius;
		int width = Visuals.Texture.Width;
		int offset = PosX + PosY * width;

		for (int y = 0; y < diameter; y++) {
			for (int x = 0; x < diameter; x++) {
				var pos = new Vector2(x * di - 1, y * di - 1);
				if (pos.LengthSquared() <= 1) {
					dest[x + y * width + offset] = source[x + y * width + offset];
				}
			}
		}
	}

	private void HandleSlider() {
		if (_cursorPos == MouseManager.MouseX - AbsolutePos.X || MouseManager.MouseX - AbsolutePos.X < _radius || MouseManager.MouseX - AbsolutePos.X > Visuals.Texture.Width - _radius)
			return;
		_cursorPos = MouseManager.MouseX - AbsolutePos.X;
		_value = _min + ((_cursorPos - _radius) / (Visuals.Texture.Width - _radius * 2) * (_max - _min));
		DrawCursor(MouseManager.MouseX - AbsolutePos.X);
		_onSliderChange(_value);
	}

	public override void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Visuals.Texture, AbsolutePos, Visuals.Color);
		spriteBatch.DrawString(Fonts.Arial, _value.ToString("F2"), AbsolutePos, Color.White);
	}
}
