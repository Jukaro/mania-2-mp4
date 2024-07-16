using Microsoft.Xna.Framework.Input;

namespace Rythmify.UI;

public static class MouseManager {
	public const int NO_SCROLL = 0;
	public const int SCROLL_UP = 1;
	public const int SCROLL_DOWN = 2;

	private static MouseState _state;
	private static int _lastMouseWheelState = 0;
	public static int MouseWheelState = NO_SCROLL;

	private static bool _isLeftButtonDown = false;
	private static bool _isRightButtonDown = false;

	public static int MouseX = 0;
	public static int MouseY = 0;

	public static void UpdateMouseState() {
		_state = Mouse.GetState();

		if (_state.ScrollWheelValue != _lastMouseWheelState) {
			if (_state.ScrollWheelValue > _lastMouseWheelState)
				MouseWheelState = SCROLL_UP;
			else
				MouseWheelState = SCROLL_DOWN;
			_lastMouseWheelState = _state.ScrollWheelValue;
		}
		else
			MouseWheelState = NO_SCROLL;

		MouseX = _state.X;
		MouseY = _state.Y;
	}

	public static bool IsLeftButtonPressed() => _state.LeftButton == ButtonState.Pressed;
	public static bool IsRightButtonPressed() => _state.RightButton == ButtonState.Pressed;

	public static bool IsLeftButtonPressedOnce() {
		if (_state.LeftButton == ButtonState.Pressed && !_isLeftButtonDown) {
			_isLeftButtonDown = true;
			return true;
		}
		else if (_state.LeftButton == ButtonState.Released)
			_isLeftButtonDown = false;
		return false;
	}

	public static bool IsRightButtonPressedOnce() {
		if (_state.RightButton == ButtonState.Pressed && !_isRightButtonDown) {
			_isRightButtonDown = true;
			return true;
		}
		else if (_state.RightButton == ButtonState.Released)
			_isRightButtonDown = false;
		return false;
	}
}
