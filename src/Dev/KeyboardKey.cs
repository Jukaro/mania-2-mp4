using Microsoft.Xna.Framework.Input;

namespace Rythmify.UI;

public class KeyboardKey {
	private Keys _key;
	private bool _isDown;

	public KeyboardKey(Keys key) {
		_key = key;
	}

	public bool IsPressed() {
		if (Keyboard.GetState().IsKeyDown(_key) && !_isDown) {
			_isDown = true;
			return true;
		}
		else if (Keyboard.GetState().IsKeyUp(_key))
			_isDown = false;
		return false;
	}
}
