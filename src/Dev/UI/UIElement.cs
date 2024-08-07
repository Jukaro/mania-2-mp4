using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.UI;

public class UIElement {
	public Visuals Visuals;
	public Vector2 RelativePos; // position relative to a container
	public Vector2 AbsolutePos; // position relative to the window
	public string Name;
	public bool IsScrollable = true;
	public bool Hide = false;

	public UIElement(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) {
		RelativePos = pos;
		AbsolutePos = pos;
		Name = name;
		Visuals = new(graphics, width, height, color);
		Visuals.SetColor(color);
	}

	public UIElement(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals) {
		RelativePos = pos;
		AbsolutePos = pos;
		Name = name;
		Visuals = new(visuals);
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public int Height => Visuals.Height;
	public int Width => Visuals.Width;

	public virtual void SetAbsolutePos(Vector2 pos) {
		AbsolutePos = pos;
	}

	public virtual void Scroll(float scrollAmount) {
		AbsolutePos.Y += scrollAmount;
	}

	public void SetColor(Color color) => Visuals.SetColor(color);
	public void SetColor(Color[] colors) => Visuals.SetTextureRelatedData(colors);
	public void SetGradientAsColor(GradientList gradientList, int step) => Visuals.SetGradientAsColor(gradientList, step);

/* --------------------------------- Update --------------------------------- */

	public virtual void Update() {
		if (Hide)
			return;
		Visuals.Update(IsMouseOver(), MouseManager.IsLeftButtonPressed());
	}

/* --------------------------------- Render --------------------------------- */

	public virtual void Render(SpriteBatch spriteBatch) {
		if (Hide)
			return;
		Visuals.Render(spriteBatch, AbsolutePos);
	}

	public virtual void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		if (Hide)
			return;
		if (mode == 1)
			Visuals.RenderPartial(spriteBatch, AbsolutePos, new Rectangle(0, 0, Width, (int)(limitY - AbsolutePos.Y)));
		else
			Visuals.RenderPartial(spriteBatch, new Vector2(AbsolutePos.X, limitY), new Rectangle(0, (int)(limitY - AbsolutePos.Y), Width, (int)(Height - (limitY - AbsolutePos.Y))));
	}

/* ---------------------------------- Utils --------------------------------- */

	public bool IsMouseOver() {
		if (MouseManager.MouseX > AbsolutePos.X && MouseManager.MouseX < AbsolutePos.X + Width
			&& MouseManager.MouseY > AbsolutePos.Y && MouseManager.MouseY < AbsolutePos.Y + Height)
			return true;
		return false;
	}
}
