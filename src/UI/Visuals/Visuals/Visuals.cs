using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using StbImageSharp;
using FontStashSharp;
using System;
using System.Collections.Generic;
using System.IO;

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
	private Texture2D _realTexture;
	private Texture2D _overlayTexture;
	private Texture2D _gradientTexture;
	public Color Color;
	private Color _baseColor;
	private Color[] BaseTextureData;

	public List<Text> Texts;

	enum BlinkState {
		BaseState,
		MouseOverState,
		ClickState
	}

	private BlinkState _blinkState;
	public bool BlinkOnMouseOver;
	public bool BlinkOnMouseClick;

	public int Width;
	public int Height;
	private System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();

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
		Texture.SetData(colors);
		if (visuals._gradientTexture != null) {
			_gradientTexture = new(_graphics, Width, Height);
			visuals._gradientTexture.GetData(colors);
			_gradientTexture.SetData(colors);
		}

		SetTextureRelatedData();
		BlinkOnMouseOver = visuals.BlinkOnMouseOver;
		BlinkOnMouseClick = visuals.BlinkOnMouseClick;
	}

	private void Init(Color color) {
		BlinkOnMouseOver = false;
		BlinkOnMouseClick = false;
		_blinkState = BlinkState.BaseState;
		BaseTextureData = new Color[Texture.Width * Texture.Height];
		Texts = new();
		SetColor(color);
		InitOverlayTextures();
	}

	public void InitGradientTexture(GradientList gradientList, int step) {
		_gradientTexture = new(_graphics, Width, Height);

		Color[] colors = new Color[Width * Height];
		for (int y = 0; y < Height; y++) {
			for (int x = 0; x < Width; x++) {
				double index = x / (double)Width * 255;
				colors[y * Width + x] = gradientList.GetColor(index, step);
			}
		}
		_gradientTexture.SetData(colors);
	}

	private void InitOverlayTextures() {
		_overlayTexture = new(_graphics, Width, Height);

		Color[] colors = new Color[Width * Height];
		Array.Fill(colors, Color.White);
		_overlayTexture.SetData(colors);
	}

	public void SetTextureFromFileAsync(string path) {
		if (path == null || path == "")
			return;

		try {
			TaskEnBien<ImageResult> task = new() {
				Callback = UpdateTexture
			};
			task.Start(() => {
				ImageResult res = GetImageResultFromFile(path);
				return res;
			});
		} catch (Exception e) {
			Logger.LogDebug($"Can't set texture: {e}");
		}
	}

	public void SetTextureFromFile(string path) {
		if (path == null || path == "")
			return;
		UpdateTexture(GetImageResultFromFile(path));
	}

	private void UpdateTexture(ImageResult img) {
		Texture2D texture = new Texture2D(_graphics, img.Width, img.Height);
		texture.SetData(img.Data);
		SetTexture(texture);
	}

	private ImageResult GetImageResultFromFile(string path) {
		using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

		if (stream.CanSeek && stream.Length == stream.Position)
		{
			stream.Seek(0L, SeekOrigin.Begin);
		}

		ImageResult imageResult;
		if (stream.CanSeek)
		{
			imageResult = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
		}
		else
		{
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			imageResult = ImageResult.FromStream(memoryStream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
		}

		return imageResult;
	}

	public void SetTexture(Texture2D texture) {
		Color = _baseColor;
		Texture = texture;

		if (Texture.Width > Width || Texture.Height > Height) {
			_realTexture = Texture;
			Texture = ResizeTexture(_realTexture, Width, Height);
		}

		SetTextureRelatedData();
	}

	public void Resize(int width, int height) {
		Width = width;
		Height = height;
		Texture = ResizeTexture(Texture, Width, Height);
		SetTextureRelatedData();
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

	public void SetTextureRelatedData() {
		BaseTextureData = new Color[Texture.Width * Texture.Height];
		Texture.GetData(BaseTextureData);
	}

	public void SetColor(Color color) {
		Color = color;
		_baseColor = Color;
		var colors = new Color[Texture.Height * Texture.Width];
		Array.Fill(colors, Color);
		Texture.SetData(colors);
		SetTextureRelatedData();
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
		Texture.SetData(colors);
		SetTextureRelatedData();
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

	public void Update(bool isMouseOver, bool isLeftButtonPressed) {
		UpdateBlinkState(isMouseOver, isLeftButtonPressed);
	}

	private void UpdateBlinkState(bool isMouseOver, bool isLeftButtonPressed) {
		if (BlinkOnMouseOver && isMouseOver && (_blinkState == BlinkState.BaseState || (_blinkState == BlinkState.ClickState && !isLeftButtonPressed))) {
			Color = Color.White;
			_blinkState = BlinkState.MouseOverState;
		}
		else if (BlinkOnMouseClick && isMouseOver && isLeftButtonPressed && _blinkState != BlinkState.ClickState) {
			Color = Color.White;
			_blinkState = BlinkState.ClickState;
		}
		else if ((_blinkState == BlinkState.MouseOverState && !isMouseOver) || (_blinkState == BlinkState.ClickState && ((isMouseOver && !isLeftButtonPressed) || !isMouseOver))) {
			Color = _baseColor;
			_blinkState = BlinkState.BaseState;
		}
	}

	public void Render(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRectangle) {
		// DrawResized(spriteBatch, Texture, pos, sourceRectangle);
		spriteBatch.Draw(Texture, pos, sourceRectangle, Color);

		Color overlayColor = _blinkState switch {
			BlinkState.MouseOverState => Color.White * 0.3f,
			BlinkState.ClickState => Color.White * 0.6f,
			_ => Color.Transparent
		};

		if (_gradientTexture != null)
			spriteBatch.Draw(_gradientTexture, pos, sourceRectangle, Color.White);

		if (_blinkState == BlinkState.ClickState || _blinkState == BlinkState.MouseOverState)
			spriteBatch.Draw(_overlayTexture, pos, sourceRectangle, overlayColor);
		foreach (Text text in Texts) {
			spriteBatch.DrawString(FontsStore.Arial2, text.Str, new(pos.X + text.RelativePos.X, pos.Y + text.RelativePos.Y), Color.White);
		}
	}

	public void DrawResized(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Rectangle sourceRectangle) {
		int source_width = texture.Width;
		int source_height = texture.Width * sourceRectangle.Height / sourceRectangle.Width;
		int source_y = texture.Height / 2 - source_height / 2;
		spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, sourceRectangle.Width, sourceRectangle.Height), new Rectangle(0, source_y + sourceRectangle.Y, source_width, source_height), Color.White);
	}
}
