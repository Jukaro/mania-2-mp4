using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.UI;

public class Dropdown : ScrollableUIElementContainer {
	private int _margin;
	private int _lastUIElementHeight;

	public Dropdown(GraphicsDevice graphics, int width, int height, Vector2 pos, int margin, string name, Color color) : base(graphics, width, height, pos, name, color) {
		_margin = margin;
		_lastUIElementHeight = (int)AbsolutePos.Y;
	}

	public Dropdown(GraphicsDevice graphics, Vector2 pos, int margin, string name, Visuals visuals) : base(graphics, pos, name, visuals) {
		_margin = margin;
		_lastUIElementHeight = (int)AbsolutePos.Y;
	}

	public override void SetAbsolutePos(Vector2 pos) {
		base.SetAbsolutePos(pos);
		_lastUIElementHeight = (int)AbsolutePos.Y;

		foreach (var UIElement in UIElementsList) {
			UIElement.SetAbsolutePos(new (RelativePos.X, _lastUIElementHeight + _margin));
			_lastUIElementHeight += UIElement.Visuals.Texture.Height + _margin * 2;
		}
	}

	public override void Add(UIElement UIElement) {
		UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, _lastUIElementHeight + _margin));
		UpdateFirstAndLastUIElements(UIElement);
		UIElementsList.Add(UIElement);
		_lastUIElementHeight += UIElement.Height + _margin * 2;
	}

	public override void RemoveAll()
	{
		base.RemoveAll();
		_lastUIElementHeight = (int)AbsolutePos.Y;
	}
}
