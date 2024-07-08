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
		lastButtonHeight = (int)BasePos.Y;
		_margin = 10;
	}

	public Button this[int index] {
		get => ButtonList[index];
		set {
			ButtonList[index] = value;
			SetBasePos(BasePos);
		}
	}

	public Button this[string key] {
		get => ButtonList.FirstOrDefault(o => o.Name == key);
		set {
			Button button = ButtonList.FirstOrDefault(o => o.Name == key);
			if (button != null) {
				button = value;
				SetBasePos(BasePos);
			}
			else
				Logger.LogDebug($"Didn't find the button \"{key}\"");
		}
	}

	public int GetIndexOfButton(Button button) {
		return ButtonList.IndexOf(button);
	}

	public override void SetBasePos(Vector2 pos) {
		base.SetBasePos(pos);
		lastButtonHeight = (int)BasePos.Y;

		foreach (var button in ButtonList) {
			button.SetBasePos(new (BasePos.X, lastButtonHeight + _margin));
			lastButtonHeight += button.Texture.Height + _margin * 2;
		}
	}

	public void Add(Button button) {
		button.SetBasePos(new (BasePos.X, lastButtonHeight + _margin));
		ButtonList.Add(button);
		lastButtonHeight += button.Texture.Height + _margin * 2;
	}

	public override void SetScrollY(int scrollAmount) {
		base.SetScrollY(scrollAmount);

		foreach (var button in ButtonList) {
			button.SetScrollY(scrollAmount);
		}
	}

	public override void Update() {
		foreach (var button in ButtonList) {
			if (button.BasePos.Y < BasePos.Y + Texture.Height) {
				button.Update();
			}
		}

		if (MouseStates.State != MouseStates.NO_SCROLL && isMouseOver() == true)
			UpdateScroll();
	}

	public override void UpdateScroll() {
		if (MouseStates.State == MouseStates.SCROLL_UP) {
			Logger.LogDebug($"{Name}: scrollUp");
			foreach (var button in ButtonList)
				button.SetScrollY(10);
		}
		else {
			Logger.LogDebug($"{Name}: scrollDown");
			foreach (var button in ButtonList)
				button.SetScrollY(-10);
		}
		MouseStates.State = MouseStates.NO_SCROLL;
		// Logger.LogDebug($"{Name}: nbRenderedButtons: {nbRenderedButtons}");
	}

	public override void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Texture, RealPos, Color);

		int i = 0;
		foreach (var button in ButtonList) {
			if (button.RealPos.Y > RealPos.Y && button.RealPos.Y < RealPos.Y + Texture.Height) { // if (button.RealPos.Y + button.Texture.Height) puis else if (button.RealPos.Y): PartialRender
				button.Render(spriteBatch);
				i++;
			}
		}
		nbRenderedButtons = i;
		// Logger.LogDebug($"Rendered buttons: {i}");
	}
}
