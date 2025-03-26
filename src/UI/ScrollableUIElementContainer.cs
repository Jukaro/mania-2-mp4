using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.UI;

public class ScrollableUIElementContainer : UIElementContainer {
	private UIElement _firstUIElement;
	private UIElement _lastUIElement;
	private Scrollbar _scrollbar;

	public bool HideScrollbar = false;

	public ScrollableUIElementContainer(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {
		Init(graphics);
	}

	public ScrollableUIElementContainer(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals)  : base(graphics, pos, name, visuals) {
		Init(graphics);
	}

	private void Init(GraphicsDevice graphics) {
		UIElementsList = new();
		_firstUIElement = null;
		_lastUIElement = null;
		SetOnScroll(UpdateScroll);
		int scrollBarWidth = 20;
		_scrollbar = new Scrollbar(graphics, scrollBarWidth, Height, Height, 0, ScrollUIElements, new Vector2(AbsolutePos.X + Width - scrollBarWidth, AbsolutePos.Y));
		UsableWidth = Width - scrollBarWidth;
		UsableHeight = Height;
	}

/* -------------------------------- Accessors ------------------------------- */

	new public UIElement this[int index] {
		get => UIElementsList[index];
		set {
			value.SetAbsolutePos(new (AbsolutePos.X + value.RelativePos.X, AbsolutePos.Y + value.RelativePos.Y));
			UpdateFirstAndLastUIElements(value);
			UIElementsList[index] = value;
		}
	}

	new public UIElement this[string key] {
		get => UIElementsList.FirstOrDefault(o => o.Name == key);
		set {
			UIElement UIElement = UIElementsList.FirstOrDefault(o => o.Name == key);
			if (UIElement != null) {
				UIElement = value;
				UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, AbsolutePos.Y + UIElement.RelativePos.Y));
				UpdateFirstAndLastUIElements(UIElement);
			}
			else
				Logger.LogDebug($"Didn't find the UIElement \"{key}\"");
		}
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public override void SetAbsolutePos(Vector2 pos) {
		base.SetAbsolutePos(pos);
		_scrollbar.SetAbsolutePos(new Vector2(AbsolutePos.X + Width - _scrollbar.Width, AbsolutePos.Y));
	}

	public override void Scroll(float scrollAmount) {
		base.Scroll(scrollAmount);
		_scrollbar.Scroll(scrollAmount);

		foreach (var UIElement in UIElementsList) {
			UIElement.Scroll(scrollAmount);
		}
	}

	protected void UpdateFirstAndLastUIElements(UIElement UIElement) {
		if (UIElement.IsScrollable && (_firstUIElement == null || UIElement.AbsolutePos.Y < _firstUIElement.AbsolutePos.Y)) {
			_firstUIElement = UIElement;
		}
		if (UIElement.IsScrollable && (_lastUIElement == null || UIElement.AbsolutePos.Y > _lastUIElement.AbsolutePos.Y)) {
			_lastUIElement = UIElement;
			if (_lastUIElement.AbsolutePos.Y + _lastUIElement.Height > AbsolutePos.Y + Height && !HideScrollbar) {
				_scrollbar.UpdateSliderSize((int)_lastUIElement.AbsolutePos.Y + _lastUIElement.Height - (int)AbsolutePos.Y);
				_scrollbar.UpdateMax(_lastUIElement.AbsolutePos.Y + _lastUIElement.Height - Height);
			}
		}
	}

/* --------------------------------- Methods -------------------------------- */

	public override void Add(UIElement UIElement) {
		UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, AbsolutePos.Y + UIElement.RelativePos.Y));
		UpdateFirstAndLastUIElements(UIElement);
		UIElementsList.Add(UIElement);

		// Logger.LogDebug($"Added {UIElement.Name} in {Name}, scrollbar height: {_scrollbar.Height}");
	}

	public override void RemoveAll() {
		UIElementsList.Clear();
		_firstUIElement = null;
		_lastUIElement = null;
		// _scrollbar.UpdateSliderSize(Height);
		// _scrollbar.UpdateMax(0);
		_scrollbar.Reset();
	}

/* --------------------------------- Update --------------------------------- */

	public override void Update() {
		_scrollbar.Update();

		base.Update();
	}

	private void ScrollUIElements(float scrollAmount) {
		if (scrollAmount >= 0 && (_firstUIElement == null || _firstUIElement.AbsolutePos.Y >= AbsolutePos.Y) )
			return;
		if (scrollAmount < 0 && (_lastUIElement == null || _lastUIElement.AbsolutePos.Y + _lastUIElement.Height <= AbsolutePos.Y + Height))
			return;
		foreach (var UIElement in UIElementsList) {
			if (UIElement.IsScrollable) {
				UIElement.Scroll(scrollAmount);
			}
		}
	}

	public void UpdateScroll() {
		if (HideScrollbar)
			return;
		if (MouseManager.MouseWheelState == MouseManager.SCROLL_UP)
			_scrollbar.UpdateSliderFromScroll(-10);
		else
			_scrollbar.UpdateSliderFromScroll(10);
	}

/* --------------------------------- Render --------------------------------- */

	public override void Render(SpriteBatch spriteBatch) {
		base.Render(spriteBatch);
		RenderUIElements(spriteBatch);
	}

	public override void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		base.RenderPartial(spriteBatch, limitY, mode);
		RenderUIElements(spriteBatch);
	}

	protected override void RenderUIElements(SpriteBatch spriteBatch) {
		if (Hide)
			return;
		base.RenderUIElements(spriteBatch);
		RenderUIElement(_scrollbar, spriteBatch);
	}

	private void RenderUIElement(UIElement UIElement, SpriteBatch spriteBatch) {
		if (UIElement.AbsolutePos.Y >= AbsolutePos.Y && UIElement.AbsolutePos.Y + UIElement.Height <= AbsolutePos.Y + Height) { // if (UIElement.AbsolutePos.Y + UIElement.Height) puis else if (UIElement.AbsolutePos.Y): PartialRender
			UIElement.Render(spriteBatch);
		}
		else if (UIElement.AbsolutePos.Y <= AbsolutePos.Y + Height && UIElement.AbsolutePos.Y + UIElement.Height >= AbsolutePos.Y + Height) {
			UIElement.RenderPartial(spriteBatch, AbsolutePos.Y + Height, 1);
		}
		else if (UIElement.AbsolutePos.Y <= AbsolutePos.Y && UIElement.AbsolutePos.Y + UIElement.Height > AbsolutePos.Y) {
			UIElement.RenderPartial(spriteBatch, AbsolutePos.Y, 0);
		}
	}
}
