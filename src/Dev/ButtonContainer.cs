using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.UI;

public class ButtonContainer : Button {
	public List<Button> ButtonsList;
	private Button _firstButton;
	private Button _lastButton;
	private Scrollbar _scrollbar;

	public int nbRenderedButtons = 0;

	public ButtonContainer(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {
		ButtonsList = new();
		_firstButton = null;
		_lastButton = null;
		SetOnScroll(UpdateScroll);
		int scrollBarWidth = 20;
		_scrollbar = new Scrollbar(graphics, scrollBarWidth, Height, Height, 0, ScrollButtons, new Vector2(AbsolutePos.X + Width - scrollBarWidth, AbsolutePos.Y));
	}

	public ButtonContainer(GraphicsDevice graphics, Vector2 pos, string name, ButtonVisuals buttonVisuals)  : base(graphics, pos, name, buttonVisuals) {
		ButtonsList = new();
		_firstButton = null;
		_lastButton = null;
		SetOnScroll(UpdateScroll);
		int scrollBarWidth = 20;
		_scrollbar = new Scrollbar(graphics, scrollBarWidth, Height, Height, 0, ScrollButtons, new Vector2(AbsolutePos.X + Width - scrollBarWidth, AbsolutePos.Y));
	}

/* -------------------------------- Accessors ------------------------------- */

	public Button this[int index] {
		get => ButtonsList[index];
		set {
			value.SetAbsolutePos(new (AbsolutePos.X + value.RelativePos.X, AbsolutePos.Y + value.RelativePos.Y));
			UpdateFirstAndLastButtons(value);
			ButtonsList[index] = value;
		}
	}

	public Button this[string key] {
		get => ButtonsList.FirstOrDefault(o => o.Name == key);
		set {
			Button button = ButtonsList.FirstOrDefault(o => o.Name == key);
			if (button != null) {
				button = value;
				button.SetAbsolutePos(new (AbsolutePos.X + button.RelativePos.X, AbsolutePos.Y + button.RelativePos.Y));
				UpdateFirstAndLastButtons(button);
			}
			else
				Logger.LogDebug($"Didn't find the button \"{key}\"");
		}
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public override void SetAbsolutePos(Vector2 pos) {
		base.SetAbsolutePos(pos);

		foreach (var button in ButtonsList) {
			button.SetAbsolutePos(new (AbsolutePos.X + button.RelativePos.X, AbsolutePos.Y + button.RelativePos.Y));
		}
		_scrollbar.SetAbsolutePos(new Vector2(AbsolutePos.X + Width - _scrollbar.Width, AbsolutePos.Y));
	}

	public override void Scroll(float scrollAmount) {
		base.Scroll(scrollAmount);
		_scrollbar.Scroll(scrollAmount);

		foreach (var button in ButtonsList) {
			button.Scroll(scrollAmount);
		}
	}

	protected void UpdateFirstAndLastButtons(Button button) {
		if (button.IsScrollable && (_firstButton == null || button.AbsolutePos.Y < _firstButton.AbsolutePos.Y)) {
			_firstButton = button;
		}
		if (button.IsScrollable && (_lastButton == null || button.AbsolutePos.Y > _lastButton.AbsolutePos.Y)) {
			_lastButton = button;
			if (_lastButton.AbsolutePos.Y + _lastButton.Height > AbsolutePos.Y + Height) {
				_scrollbar.UpdateSliderSize((int)_lastButton.AbsolutePos.Y + _lastButton.Height - (int)AbsolutePos.Y);
				_scrollbar.UpdateMax(_lastButton.AbsolutePos.Y + _lastButton.Height - Height);
			}
		}
	}

/* --------------------------------- Methods -------------------------------- */

	public virtual void Add(Button button) {
		button.SetAbsolutePos(new (AbsolutePos.X + button.RelativePos.X, AbsolutePos.Y + button.RelativePos.Y));
		UpdateFirstAndLastButtons(button);
		ButtonsList.Add(button);
	}

	public int GetIndexOfButton(Button button) {
		return ButtonsList.IndexOf(button);
	}

/* --------------------------------- Update --------------------------------- */

	public override void Update() {
		foreach (var button in ButtonsList) {
			if (button.AbsolutePos.Y <= AbsolutePos.Y + ButtonVisuals.Texture.Height) { // mdr non faut ameliorer
				button.Update();
			}
		}
		_scrollbar.Update();

		base.Update();
	}

	private void ScrollButtons(float scrollAmount) {
		if (scrollAmount >= 0 && (_firstButton == null || _firstButton.AbsolutePos.Y >= AbsolutePos.Y) )
			return;
		if (scrollAmount < 0 && (_lastButton == null || _lastButton.AbsolutePos.Y + _lastButton.Height <= AbsolutePos.Y + Height))
			return;
		foreach (var button in ButtonsList) {
			if (button.IsScrollable) {
				button.Scroll(scrollAmount);
			}
		}
	}

	public void UpdateScroll() {
		if (MouseManager.MouseWheelState == MouseManager.SCROLL_UP)
			_scrollbar.UpdateSliderFromScroll(-10);
		else
			_scrollbar.UpdateSliderFromScroll(10);
	}

/* --------------------------------- Render --------------------------------- */

	public override void Render(SpriteBatch spriteBatch) {
		base.Render(spriteBatch);
		RenderButtons(spriteBatch);
	}

	public override void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		base.RenderPartial(spriteBatch, limitY, mode);
		RenderButtons(spriteBatch);
	}

	private void RenderButtons(SpriteBatch spriteBatch) {
		foreach (var button in ButtonsList) {
			if (button.AbsolutePos.Y + button.Height < 0 || button.AbsolutePos.Y > 1000)
				continue;
			RenderButton(button, spriteBatch);
		}
		RenderButton(_scrollbar, spriteBatch);
	}

	private void RenderButton(Button button, SpriteBatch spriteBatch) {
		if (button.AbsolutePos.Y >= AbsolutePos.Y && button.AbsolutePos.Y + button.Height <= AbsolutePos.Y + Height) { // if (button.AbsolutePos.Y + button.Height) puis else if (button.AbsolutePos.Y): PartialRender
			button.Render(spriteBatch);
		}
		else if (button.AbsolutePos.Y <= AbsolutePos.Y + Height && button.AbsolutePos.Y + button.Height >= AbsolutePos.Y + Height) {
			button.RenderPartial(spriteBatch, AbsolutePos.Y + Height, 1);
		}
		else if (button.AbsolutePos.Y <= AbsolutePos.Y && button.AbsolutePos.Y + button.Height > AbsolutePos.Y) {
			button.RenderPartial(spriteBatch, AbsolutePos.Y, 0);
		}
	}
}
