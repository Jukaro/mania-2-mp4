using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class ToggleButton : Button {
	private bool _isOn;
	private Action _on;
	private Action _off;

	public ToggleButton(GraphicsDevice graphics, Vector2 pos, string name, Action on, Action off, ButtonVisuals buttonVisuals) : base(graphics, pos, name, buttonVisuals) {
		_isOn = false;
		_on = on;
		_off = off;
		SetOnClick(OnOffAction);
	}

	private void OnOffAction() {
		if (!_isOn) {
			_on();
			_isOn = true;
		}
		else if (_isOn) {
			_off();
			_isOn = false;
		}
	}
}

public class SliderButton : Button {
	private Action<double> _onSliderChange;
	private Color[] _baseTextureData;
	private int _radius;
	private double _cursorPos;
	private double _value;
	private double _min = -20;
	private double _max = 20;

	public SliderButton(GraphicsDevice graphics, Vector2 pos, string name, double min, double max, int cursorRadius, Action<double> onSliderChange, ButtonVisuals buttonVisuals) : base(graphics, pos, name, buttonVisuals) {
		_onSliderChange = onSliderChange;
		_baseTextureData = new Color[ButtonVisuals.Texture.Height * ButtonVisuals.Texture.Width];
		ButtonVisuals.Texture.GetData(_baseTextureData);
		SetOnClickHold(HandleSlider);
		DrawCursor(ButtonVisuals.Texture.Width / 2);
		_radius = cursorRadius;
		_min = min;
		_max = max;
	}

	private void DrawCursor(double PosX) {
		int width = ButtonVisuals.Texture.Width;
		int height = ButtonVisuals.Texture.Height;
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
		int width = ButtonVisuals.Texture.Width;
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
		if (_cursorPos == MouseManager.MouseX - AbsolutePos.X || MouseManager.MouseX - AbsolutePos.X < _radius || MouseManager.MouseX - AbsolutePos.X > ButtonVisuals.Texture.Width - _radius)
			return;
		_cursorPos = MouseManager.MouseX - AbsolutePos.X;
		_value = _min + ((_cursorPos - _radius) / (ButtonVisuals.Texture.Width - _radius * 2) * (_max - _min));
		DrawCursor(MouseManager.MouseX - AbsolutePos.X);
		_onSliderChange(_value);
	}

	public override void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(ButtonVisuals.Texture, AbsolutePos, ButtonVisuals.Color);
		spriteBatch.DrawString(Fonts.Arial, _value.ToString("F2"), AbsolutePos, Color.White);
	}
}

public class Scrollbar : Button {
	private Button _slider;
	private GraphicsDevice _graphics;
	private double _lastCursorPos;
	private double _currentCursorPos;
	private bool _isHeld;
	private double _baseMousePosY;
	private double _baseSliderPosY;
	private double _min;
	private double _max;
	private double _value;
	private double _lastValue;
	private Action<float> _onSliderChange;

	public Scrollbar(GraphicsDevice graphics, Vector2 pos, ButtonVisuals buttonVisuals) : base(graphics, pos, "scrollbar", buttonVisuals) {
		IsScrollable = false;
	}

	public Scrollbar(GraphicsDevice graphics, int width, int height, int max_height, int max, Action<float> onSliderChange, Vector2 pos) : base(graphics, width, height, pos, "scrollbar", Color.Gray) {
		_graphics = graphics;
		IsScrollable = false;
		_min = AbsolutePos.Y;
		_max = max;
		_slider = new(graphics, width, height * height / max_height, pos, "slider", Color.LightGray);
		_onSliderChange = onSliderChange;
	}

	public void UpdateSliderSize(int max_height) {
		_slider = new(_graphics, _slider.Width, Height * Height / max_height, AbsolutePos, "slider", Color.LightGray);
	}

	public void UpdateMax(double max) {
		_max = max;
		_lastValue = _min + _slider.AbsolutePos.Y / (Height - _slider.Height) * (_max - _min);
	}

	public override void Scroll(float scrollAmount)
	{
		base.Scroll(scrollAmount);
		_slider.Scroll(scrollAmount);
	}

	public void UpdateSliderFromScroll(float scrollAmount) {
		double currentSliderPos = _slider.AbsolutePos.Y + scrollAmount;
		UpdateSlider(currentSliderPos, scrollAmount);
	}

	private void UpdateSlider(double currentSliderPos, double scrollAmount) {
		if (currentSliderPos < AbsolutePos.Y || currentSliderPos > AbsolutePos.Y + Height - _slider.Height) {
			if ((currentSliderPos < AbsolutePos.Y && scrollAmount > 0) || (currentSliderPos > AbsolutePos.Y + Height - _slider.Height && scrollAmount < 0)) {
				_baseMousePosY = MouseManager.MouseY;
				_lastCursorPos = _currentCursorPos;
			}
			_baseSliderPosY = _slider.AbsolutePos.Y;
			double clampedSliderPos = Math.Clamp(currentSliderPos, AbsolutePos.Y, AbsolutePos.Y + Height - _slider.Height);
			if (_slider.AbsolutePos.Y != clampedSliderPos)
				UpdateSliderInfo(clampedSliderPos);
			return;
		}
		UpdateSliderInfo(currentSliderPos);
	}

	private void UpdateSliderInfo(double sliderPos) {
		_value = _min + sliderPos / (Height - _slider.Height) * (_max - _min);
		// Logger.LogDebug($"value: {_value}, lastValue: {_lastValue}, sliderPos: {sliderPos}");
		_lastCursorPos = _currentCursorPos;
		_slider.AbsolutePos.Y = (float)sliderPos;
		_onSliderChange((float)(_value - _lastValue) * -1);
		_lastValue = _value;
	}

	public override void Update() {
		if ((_slider.IsMouseOver() || _isHeld) && MouseManager.IsLeftButtonPressed()) {
			if (!_isHeld) {
				_baseMousePosY = MouseManager.MouseY;
				_currentCursorPos = MouseManager.MouseY;
				_lastCursorPos = MouseManager.MouseY;
				_baseSliderPosY = _slider.AbsolutePos.Y;
				_lastValue = _min + (_baseSliderPosY + (_currentCursorPos - _baseMousePosY)) / (Height - _slider.Height) * (_max - _min);
			}
			double scrollAmount = MouseManager.MouseY - _currentCursorPos;
			_currentCursorPos = MouseManager.MouseY;
			if (_lastCursorPos != _currentCursorPos) {
				double currentSliderPos = _baseSliderPosY + (_currentCursorPos - _baseMousePosY);
				UpdateSlider(currentSliderPos, scrollAmount);
			}
			_isHeld = true;
		}
		else
			_isHeld = false;

		ButtonVisuals.Update(IsMouseOver(), MouseManager.IsLeftButtonPressed());
	}

    public override void SetAbsolutePos(Vector2 pos)
	{
		base.SetAbsolutePos(pos);
		_slider.SetAbsolutePos(pos);
		double diff = _max - _min;
		_min = AbsolutePos.Y;
		_max = _min + diff;
	}

	public override void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(ButtonVisuals.Texture, AbsolutePos, ButtonVisuals.Color);
		_slider.Render(spriteBatch);
	}
}
