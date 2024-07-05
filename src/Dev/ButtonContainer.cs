using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.UI;

public class ButtonContainer : Button {
	public List<Button> ButtonList;
	private int lastButtonHeight;
	private int margin;

	public ButtonContainer(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {
		ButtonList = new();
		lastButtonHeight = (int)Pos.Y;
		// Logger.LogDebug($"{Name}'s height: {lastButtonHeight}");
		margin = 10;
	}

	public Button this[int index] {
		get => ButtonList[index];
		set {
			ButtonList[index] = value;
			SetPos(Pos);
		}
	}

	public Button this[string key] {
		get => ButtonList.FirstOrDefault(o => o.Name == key);
		set {
			Button button = ButtonList.FirstOrDefault(o => o.Name == key);
			if (button != null) {
				button = value;
				SetPos(Pos);
			}
			else
				Logger.LogDebug($"Didn't find the button \"{key}\"");
		}
	}

	public int GetIndexOfButton(Button button) {
		return ButtonList.IndexOf(button);
	}

	public override void SetPos(Vector2 pos) {
		base.SetPos(pos);
		lastButtonHeight = (int)Pos.Y;

		foreach (var button in ButtonList) {
			button.SetPos(new (Pos.X, lastButtonHeight + margin));
			lastButtonHeight += button.Texture.Height + margin * 2;
		}
	}

	public void Add(Button button) {
		button.SetPos(new (Pos.X, lastButtonHeight + margin));
		ButtonList.Add(button);
		lastButtonHeight += button.Texture.Height + margin * 2;
	}

	public override void Render(SpriteBatch spriteBatch) {
		spriteBatch.Draw(Texture, Pos, Color);

		int i = 0;
		foreach (var button in ButtonList) {
			if (button.Pos.Y < Pos.Y + Texture.Height) { // if (button.Pos.Y + button.Texture.Height) puis else if (button.Pos.Y): PartialRender
				button.Render(spriteBatch);
				i++;
			}
		}
		// Logger.LogDebug($"Rendered buttons: {i}");
	}
}
