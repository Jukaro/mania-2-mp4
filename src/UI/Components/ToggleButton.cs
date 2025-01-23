using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class ToggleButton : Button {
	private bool _isOn;
	private Action _on;
	private Action _off;

	public ToggleButton(GraphicsDevice graphics, Vector2 pos, string name, Action on, Action off, Visuals visuals) : base(graphics, pos, name, visuals) {
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
