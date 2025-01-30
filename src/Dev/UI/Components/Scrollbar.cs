using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

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

	public Scrollbar(GraphicsDevice graphics, Vector2 pos, Visuals visuals) : base(graphics, pos, "scrollbar", visuals) {
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

		Visuals.Update(IsMouseOver(), MouseManager.IsLeftButtonPressed());
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
		spriteBatch.Draw(Visuals.Texture, AbsolutePos, Visuals.Color);
		_slider.Render(spriteBatch);
	}
}
