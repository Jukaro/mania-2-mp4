using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rythmify.UI;

public class Button : UIElement {
	private Action _onClick = null;
	private Action _onClickHold = null;
	private bool _isHeld = false;
	private Action _onScroll = null;

	public Button(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {

	}

	public Button(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals) : base(graphics, pos, name, visuals) {

	}

/* ---------------------------- Getters / Setters --------------------------- */

	public void SetOnClick(Action onClick) {
		_onClick = onClick;
	}

	public void SetOnClickHold(Action onClickHold) {
		_onClickHold = onClickHold;
	}

	public void SetOnScroll(Action onScroll) {
		_onScroll = onScroll;
	}

/* --------------------------------- Update --------------------------------- */

	public override void Update() {
		if ((IsMouseOver() || _isHeld) && MouseManager.IsLeftButtonPressed()) {
			if (_onClickHold != null)
				_onClickHold();
			_isHeld = true;
		}
		else {
			if (_onClick != null && IsMouseOver() && _isHeld)
				_onClick();
			_isHeld = false;
		}

		if (_onScroll != null && IsMouseOver() && MouseManager.MouseWheelState != MouseManager.NO_SCROLL) {
			_onScroll();
			MouseManager.MouseWheelState = MouseManager.NO_SCROLL;
		}

		base.Update();
	}
}
