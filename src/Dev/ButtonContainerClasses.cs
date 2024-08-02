using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.UI;

public class Dropdown : ButtonContainer {
	private int _margin;
	private int _lastButtonHeight;

	public Dropdown(GraphicsDevice graphics, int width, int height, Vector2 pos, int margin, string name, Color color) : base(graphics, width, height, pos, name, color) {
		_margin = margin;
		_lastButtonHeight = (int)AbsolutePos.Y;
	}

	public Dropdown(GraphicsDevice graphics, Vector2 pos, int margin, string name, ButtonVisuals buttonVisuals) : base(graphics, pos, name, buttonVisuals) {
		_margin = margin;
		_lastButtonHeight = (int)AbsolutePos.Y;
	}

	public override void SetAbsolutePos(Vector2 pos) {
		base.SetAbsolutePos(pos);
		_lastButtonHeight = (int)AbsolutePos.Y;

		foreach (var button in ButtonsList) {
			button.SetAbsolutePos(new (RelativePos.X, _lastButtonHeight + _margin));
			_lastButtonHeight += button.ButtonVisuals.Texture.Height + _margin * 2;
		}
	}

	public override void Add(Button button) {
		button.SetAbsolutePos(new (AbsolutePos.X + button.RelativePos.X, _lastButtonHeight + _margin));
		UpdateFirstAndLastButtons(button);
		ButtonsList.Add(button);
		_lastButtonHeight += button.Height + _margin * 2;
	}
}
