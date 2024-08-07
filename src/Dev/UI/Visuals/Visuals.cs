using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Rythmify.UI;

public class Text {
	public string Str;
	public Vector2 RelativePos;

	public Text(string str, Vector2 pos) {
		Str = str;
		RelativePos = pos;
	}
}

public class Visuals {
	private readonly GraphicsDevice _graphics;

	public Texture2D Texture;
	private Texture2D RealTexture;
	public Color Color;
	private Color _baseColor;
	private Color[] BaseTextureData;
	private Color[] MouseOverTextureData;
	private Color[] OnClickTextureData;

	public List<Text> Texts;

	private const int BASE_STATE = 0;
	private const int MOUSE_OVER_STATE = 1;
	private const int CLICK_STATE = 2;

	private int _blinkState;
	public bool BlinkOnMouseOver;
	public bool BlinkOnMouseClick;

	public int Width;
	public int Height;

	public Visuals(GraphicsDevice graphics, int width, int height, Color color) {
		_graphics = graphics;
		Texture = new(_graphics, width, height);
		Width = width;
		Height = height;
		Init(color);
	}

	public Visuals(Texture2D texture) {
		Texture = texture;
		Init(Color.White);
	}

	public Visuals(Visuals visuals) {
		_graphics = visuals._graphics;
		Width = visuals.Width;
		Height = visuals.Height;
		Texture = new(_graphics, Width, Height);
		Init(visuals.Color);
		Color[] colors = new Color[Texture.Width * Texture.Height];
		visuals.Texture.GetData(colors);
		SetTextureRelatedData(colors);
		BlinkOnMouseOver = visuals.BlinkOnMouseOver;
		BlinkOnMouseClick = visuals.BlinkOnMouseClick;
	}

	private void Init(Color color) {
		BlinkOnMouseOver = false;
		BlinkOnMouseClick = false;
		_blinkState = BASE_STATE;
		BaseTextureData = new Color[Texture.Width * Texture.Height];
		MouseOverTextureData = new Color[Texture.Width * Texture.Height];
		OnClickTextureData = new Color[Texture.Width * Texture.Height];
		Texts = new();
		SetColor(color);
	}

	public void SetTextureFromFile(string path) {
		try {
			_baseColor = Color.DarkGray;
			Color = _baseColor;
			Texture = Texture2D.FromFile(_graphics, path);
			if (Texture.Width > Width || Texture.Height > Height) {
				RealTexture = Texture;
				Texture = ResizeTexture(RealTexture, Width, Height);
			}
			BaseTextureData = new Color[Texture.Width * Texture.Height];
			MouseOverTextureData = new Color[Texture.Width * Texture.Height];
			OnClickTextureData = new Color[Texture.Width * Texture.Height];
			Texture.GetData(BaseTextureData);
			SetMouseOverTexture();
			SetOnClickTexture();
		} catch (Exception e) {
			Logger.LogDebug($"Can't set texture: {e}");
		}
		// Init(Color.White);
	}

	public void SetTexture(Texture2D texture) {
		Color[] data = new Color[texture.Width * texture.Height];
		texture.GetData(data);
		Texture.SetData(data);
		SetMouseOverTexture();
		SetOnClickTexture();
	}

	private Texture2D ResizeTexture(Texture2D texture, int width, int height)
	{
		RenderTarget2D renderTarget = new RenderTarget2D(_graphics, width, height);
		SpriteBatch spriteBatch = new(_graphics);

		_graphics.SetRenderTarget(renderTarget);
		_graphics.Clear(Color.Transparent);

		int source_width = texture.Width;
		int source_height = texture.Width * height / width;

		spriteBatch.Begin();
		spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), new Rectangle(0, texture.Height / 2 - source_height / 2, source_width, source_height), Color.White);
		spriteBatch.End();

		_graphics.SetRenderTarget(null);

		Texture2D result = new Texture2D(_graphics, width, height);
		Color[] data = new Color[width * height];
		renderTarget.GetData(data);
		result.SetData(data);

		return result;
	}

	public void SetTextureRelatedData(Color[] colors) {
		Texture.SetData(colors);
		Texture.GetData(BaseTextureData);
		SetMouseOverTexture();
		SetOnClickTexture();
	}

	public void SetColor(Color color) {
		Color = color;
		_baseColor = Color;
		var colors = new Color[Texture.Height * Texture.Width];
		Array.Fill(colors, Color);
		SetTextureRelatedData(colors);
	}

	public void SetGradientAsColor(GradientList gradientList, int step) {
		_baseColor = Color.White;
		var colors = new Color[Texture.Height * Texture.Width];
		for (int y = 0; y < Texture.Height; y++) {
			for (int x = 0; x < Texture.Width; x++) {
				double index = x + y;
				colors[y * Texture.Width + x] = gradientList.GetColor(index, step);
			}
		}
		SetTextureRelatedData(colors);
	}

	private void AddValueToTextureData(Color[] textureData, int value) {
		for (int i = 0; i < textureData.Length; i++) {
			Color pixel = textureData[i];

			int r = Math.Clamp(pixel.R + value, 0, 255);
			int g = Math.Clamp(pixel.G + value, 0, 255);
			int b = Math.Clamp(pixel.B + value, 0, 255);

			textureData[i] = new Color(r, g, b, pixel.A);
		}
	}

	private void SetMouseOverTexture() {
		Texture.GetData(MouseOverTextureData);
		AddValueToTextureData(MouseOverTextureData, 50);
	}

	private void SetOnClickTexture() {
		Texture.GetData(OnClickTextureData);
		AddValueToTextureData(OnClickTextureData, 100);
	}

	public void Update(bool isMouseOver, bool isLeftButtonPressed) {
		UpdateBlinkState(isMouseOver, isLeftButtonPressed);
	}

	private void UpdateBlinkState(bool isMouseOver, bool isLeftButtonPressed) {
		if (BlinkOnMouseOver && isMouseOver && (_blinkState == BASE_STATE || (_blinkState == CLICK_STATE && !isLeftButtonPressed))) {
			Texture.SetData(MouseOverTextureData);
			Color = Color.White;
			_blinkState = MOUSE_OVER_STATE;
		}
		else if (BlinkOnMouseClick && isMouseOver && isLeftButtonPressed && _blinkState != CLICK_STATE) {
			Texture.SetData(OnClickTextureData);
			Color = Color.White;
			_blinkState = CLICK_STATE;
		}
		else if ((_blinkState == MOUSE_OVER_STATE && !isMouseOver) || (_blinkState == CLICK_STATE && ((isMouseOver && !isLeftButtonPressed) || !isMouseOver))) {
			Texture.SetData(BaseTextureData);
			Color = _baseColor;
			_blinkState = BASE_STATE;
		}
	}

	public void Render(SpriteBatch spriteBatch, Vector2 pos) {
		// DrawResized(spriteBatch, Texture, pos, new Rectangle(0, 0, Width, Height));
		spriteBatch.Draw(Texture, pos, new Rectangle(0, 0, Width, Height), Color);
		foreach (Text text in Texts) {
			spriteBatch.DrawString(Fonts.Arial, text.Str, new(pos.X + text.RelativePos.X, pos.Y + text.RelativePos.Y), Color.White);
		}
	}

	public void RenderPartial(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRectangle) {
		// if (RealTexture != null)
		// 	DrawResized(spriteBatch, RealTexture, pos, sourceRectangle);
		// else
			spriteBatch.Draw(Texture, pos, sourceRectangle, Color);
	}

	public void DrawResized(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Rectangle sourceRectangle) {
		int source_width = texture.Width;
		int source_height = texture.Width * sourceRectangle.Height / sourceRectangle.Width;
		int source_y = texture.Height / 2 - source_height / 2;
		spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, sourceRectangle.Width, sourceRectangle.Height), new Rectangle(0, source_y + sourceRectangle.Y, source_width, source_height), Color.White);
	}
}
