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
			// Logger.LogDebug($"key: {e.KeyChar}");
			Visuals.Texts[0].Str += e.KeyChar.ToString();
		}
		if (e.KeyChar == 8) {
			if (Visuals.Texts[0].Str.Length > 0)
				Visuals.Texts[0].Str = Visuals.Texts[0].Str.Remove(Visuals.Texts[0].Str.Length - 1);
		}
	}
}
