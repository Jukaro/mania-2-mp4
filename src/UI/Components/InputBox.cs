using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.UI;
using SharpHook;
using SharpHook.Native;

namespace Rythmify.Dev;

public class InputBox : Button {
	private bool _isListening = false;
	private readonly List<char> _currentChars = new();

	public string Input => Visuals.Texts[0].Str;

	public InputBox(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals) : base(graphics, pos, name, visuals) {
		SetOnClick(Focus);
		Visuals.Texts.Add(new Text("", new (0, 0)));
	}

	public void Focus() {
		if (_isListening) return;

		ManagedGlobalHook.Instance.Hook.KeyTyped += ManageInput;
		_isListening = true;
	}

	public void Unfocus() {
		if (!_isListening) return;

		ManagedGlobalHook.Instance.Hook.KeyTyped -= ManageInput;
		_isListening = false;
	}

	private void ManageInput(object sender, KeyboardHookEventArgs e) {
		if ((int)e.Data.KeyCode >= 32)
			_currentChars.Add(e.Data.KeyChar);

		if (e.Data.KeyCode == KeyCode.VcBackspace && _currentChars.Count > 0)
			_currentChars.RemoveAt(_currentChars.Count - 1);

		Visuals.Texts[0].Str = string.Join("", _currentChars);
	}

	public override void Update() {
		base.Update();

		if (MouseManager.IsLeftButtonPressed() && !IsMouseOver() && _isListening)
			Unfocus();
	}
}
