using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rythmify.UI;

public class Button {
	public ButtonVisuals ButtonVisuals;
	public Vector2 RelativePos; // position without ScrollY
	public Vector2 AbsolutePos; // position with ScrollY
	public int ScrollY;
	public string Name;
	private Action _onClick = null;
	private Action _onScroll = null;

	public Button(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) {
		ScrollY = 0;
		RelativePos = pos;
		AbsolutePos = pos;
		Name = name;
		ButtonVisuals = new(new Texture2D(graphics, width, height));
		ButtonVisuals.SetColor(color);
	}

	public Button(GraphicsDevice graphics, Vector2 pos, string name, ButtonVisuals buttonVisuals) {
		ScrollY = 0;
		RelativePos = pos;
		AbsolutePos = pos;
		Name = name;
		ButtonVisuals = new(graphics, buttonVisuals);
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public virtual void SetRelativePos(Vector2 pos) {
		RelativePos = pos;
		AbsolutePos = new(RelativePos.X, RelativePos.Y + ScrollY);
	}

	public virtual void SetScrollY(int scrollAmount) {
		ScrollY += scrollAmount;
		AbsolutePos = new(RelativePos.X, RelativePos.Y + ScrollY);
	}

	public void SetColor(Color color) => ButtonVisuals.SetColor(color);
	public void SetGradientAsColor(GradientList gradientList) => ButtonVisuals.SetGradientAsColor(gradientList);

	public void SetOnClick(Action onClick) {
		_onClick = onClick;
	}

	public void SetOnScroll(Action onScroll) {
		_onScroll = onScroll;
	}

/* --------------------------------- Update --------------------------------- */

	public virtual void Update() {
		if (_onClick != null && IsMouseOver() && MouseManager.IsLeftButtonPressedOnce()) {
			_onClick();
		}

		if (_onScroll != null && IsMouseOver() && MouseManager.MouseWheelState != MouseManager.NO_SCROLL) {
			_onScroll();
			MouseManager.MouseWheelState = MouseManager.NO_SCROLL;
		}

		ButtonVisuals.Update(IsMouseOver(), MouseManager.IsLeftButtonPressed());
	}

/* --------------------------------- Render --------------------------------- */

	public virtual void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(ButtonVisuals.Texture, AbsolutePos, ButtonVisuals.Color);
	}

	public virtual void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		if (mode == 1)
			spriteBatch.Draw(ButtonVisuals.Texture, AbsolutePos, new Rectangle(0, 0, ButtonVisuals.Texture.Width, (int)(limitY - AbsolutePos.Y)), ButtonVisuals.Color);
		else
			spriteBatch.Draw(ButtonVisuals.Texture, new Vector2(AbsolutePos.X, limitY), new Rectangle(0, (int)(limitY - AbsolutePos.Y), ButtonVisuals.Texture.Width, (int)(ButtonVisuals.Texture.Height - (limitY - AbsolutePos.Y))), ButtonVisuals.Color);
	}

/* ---------------------------------- Utils --------------------------------- */

	public bool IsMouseOver() {
		if (MouseManager.MouseX > AbsolutePos.X && MouseManager.MouseX < AbsolutePos.X + ButtonVisuals.Texture.Width
			&& MouseManager.MouseY > AbsolutePos.Y && MouseManager.MouseY < AbsolutePos.Y + ButtonVisuals.Texture.Height)
			return true;
		return false;
	}
}
