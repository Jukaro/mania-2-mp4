using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class ButtonVisuals {
	public Texture2D Texture;
	public Color Color;
	private Color _baseColor;
	private Color[] BaseTextureData;
	private Color[] MouseOverTextureData;
	private Color[] OnClickTextureData;

	private const int BASE_STATE = 0;
	private const int MOUSE_OVER_STATE = 1;
	private const int CLICK_STATE = 2;

	private int _blinkState;
	public bool BlinkOnMouseOver;
	public bool BlinkOnMouseClick;

	public ButtonVisuals(GraphicsDevice graphics, int width, int height, Color color) {
		Texture = new(graphics, width, height);
		Init(color);
	}

	public ButtonVisuals(Texture2D texture) {
		Texture = texture;
		Init(Color.White);
	}

	public ButtonVisuals(GraphicsDevice graphics, ButtonVisuals buttonVisuals) {
		Texture = new(graphics, buttonVisuals.Texture.Width, buttonVisuals.Texture.Height);
		Init(buttonVisuals.Color);
		BlinkOnMouseOver = buttonVisuals.BlinkOnMouseOver;
		BlinkOnMouseClick = buttonVisuals.BlinkOnMouseClick;
	}

	private void Init(Color color) {
		BlinkOnMouseOver = false;
		BlinkOnMouseClick = false;
		_blinkState = BASE_STATE;
		BaseTextureData = new Color[Texture.Width * Texture.Height];
		MouseOverTextureData = new Color[Texture.Width * Texture.Height];
		OnClickTextureData = new Color[Texture.Width * Texture.Height];
		SetColor(color);
	}

	private void SetTextureRelatedData(Color[] colors) {
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

	public void SetGradientAsColor(GradientList gradientList) {
		var colors = new Color[Texture.Height * Texture.Width];
		for (int y = 0; y < Texture.Height; y++) {
			for (int x = 0; x < Texture.Width; x++) {
				double index = x + y;
				int step = 255;
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

			textureData[i] = new Color(r, g, b);
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
}
