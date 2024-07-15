using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.UI;

public class ButtonContainer : Button {
	public List<Button> ButtonList;
	private int lastButtonHeight;
	private int _margin;

	public int nbRenderedButtons = 0;

	public ButtonContainer(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {
		ButtonList = new();
		lastButtonHeight = (int)RelativePos.Y;
		_margin = 10;
	}

/* -------------------------------- Accessors ------------------------------- */

	public Button this[int index] {
		get => ButtonList[index];
		set {
			ButtonList[index] = value;
			SetRelativePos(RelativePos);
		}
	}

	public Button this[string key] {
		get => ButtonList.FirstOrDefault(o => o.Name == key);
		set {
			Button button = ButtonList.FirstOrDefault(o => o.Name == key);
			if (button != null) {
				button = value;
				SetRelativePos(RelativePos);
			}
			else
				Logger.LogDebug($"Didn't find the button \"{key}\"");
		}
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public override void SetRelativePos(Vector2 pos) {
		base.SetRelativePos(pos);
		lastButtonHeight = (int)RelativePos.Y;

		foreach (var button in ButtonList) {
			button.SetRelativePos(new (RelativePos.X, lastButtonHeight + _margin));
			lastButtonHeight += button.Texture.Height + _margin * 2;
		}
	}

	public override void SetScrollY(int scrollAmount) {
		base.SetScrollY(scrollAmount);

		foreach (var button in ButtonList) {
			button.SetScrollY(scrollAmount);
		}
	}

/* --------------------------------- Methods -------------------------------- */

	public void Add(Button button) {
		button.SetRelativePos(new (RelativePos.X, lastButtonHeight + _margin));
		ButtonList.Add(button);
		lastButtonHeight += button.Texture.Height + _margin * 2;
	}

	public int GetIndexOfButton(Button button) {
		return ButtonList.IndexOf(button);
	}

/* --------------------------------- Update --------------------------------- */

	public override void Update() {
		foreach (var button in ButtonList) {
			if (button.AbsolutePos.Y <= AbsolutePos.Y + Texture.Height) {
				button.Update();
			}
		}

		if (MouseStates.State != MouseStates.NO_SCROLL && isMouseOver() == true)
			UpdateScroll();
	}

	public override void UpdateScroll() {
		if (MouseStates.State == MouseStates.SCROLL_UP) {
			foreach (var button in ButtonList)
				button.SetScrollY(10);
		}
		else {
			foreach (var button in ButtonList)
				button.SetScrollY(-10);
		}
		MouseStates.State = MouseStates.NO_SCROLL;
		// Logger.LogDebug($"{Name}: nbRenderedButtons: {nbRenderedButtons}");
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

	// render tjrs les boutons quand ils sont en dehors de la fenetre, modif la condition avec
	private void RenderButtons(SpriteBatch spriteBatch) {
		int i = 0;
		foreach (var button in ButtonList) {
			if (button.AbsolutePos.Y + button.Texture.Height < 0 || button.AbsolutePos.Y > 1000)
				continue;
			if (button.AbsolutePos.Y >= AbsolutePos.Y && button.AbsolutePos.Y + button.Texture.Height <= AbsolutePos.Y + Texture.Height) { // if (button.AbsolutePos.Y + button.Texture.Height) puis else if (button.AbsolutePos.Y): PartialRender
				button.Render(spriteBatch);
				i++;
			}
			else if (button.AbsolutePos.Y <= AbsolutePos.Y + Texture.Height && button.AbsolutePos.Y + button.Texture.Height >= AbsolutePos.Y + Texture.Height) {
				button.RenderPartial(spriteBatch, AbsolutePos.Y + Texture.Height, 1);
				i++;
			}
			else if (button.AbsolutePos.Y <= AbsolutePos.Y && button.AbsolutePos.Y + button.Texture.Height > AbsolutePos.Y) {
				button.RenderPartial(spriteBatch, AbsolutePos.Y, 0);
				i++;
			}
		}
		nbRenderedButtons = i;
	}
}
