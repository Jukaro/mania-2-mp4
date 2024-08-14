using System.IO;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.UI;

public class InputBox : Button {
	public string Input;
	private bool _isListening = false;

	public InputBox(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals) : base(graphics, pos, name, visuals) {
		SetOnClick(OnClick);
		Visuals.Texts.Add(new Text("", new (0, 0)));
	}

	private void OnClick() {
		if (!_isListening) {
			KeyboardManager.KeyPressed += ManageInput;
			_isListening = true;
		} else {
			KeyboardManager.KeyPressed -= ManageInput;
			_isListening = false;
		}
	}

	private void ManageInput(object sender, KeyPressedEventArgs e) {
		if (e.KeyChar >= 32 && e.KeyChar <= 126) {
			Logger.LogDebug($"key: {(int)e.KeyChar}");
			Input += e.KeyChar.ToString();
			Visuals.Texts[0].Str = Input;
		}
		if (e.KeyChar == 8) {
			if (Input.Length > 0) {
				Input = Input.Remove(Input.Length - 1);
				Visuals.Texts[0].Str = Input;
			}
		}
	}
}
