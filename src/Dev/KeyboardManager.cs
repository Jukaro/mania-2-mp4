using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Rythmify.UI;

public class KeyPressedEventArgs : EventArgs
{
	public char KeyChar { get; }

	public KeyPressedEventArgs(char keyChar)
	{
		KeyChar = keyChar;
	}
}

public static class KeyboardManager {
	public delegate void KeyPressedEventHandler(object sender, KeyPressedEventArgs e);
	public static event KeyPressedEventHandler KeyPressed;

	private static KeyboardState _previousState;
	private static Keys _currentKey;
	private static System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();

	static KeyboardManager() {
		_previousState = Keyboard.GetState();
	}

	private static void OnKeyPressed(char keyChar)
	{
		KeyPressed?.Invoke(null, new KeyPressedEventArgs(keyChar));
	}

	public static void Update()
	{
		KeyboardState state = Keyboard.GetState();
		int currentKeyCount = state.GetPressedKeyCount();
		int previousKeyCount = _previousState.GetPressedKeyCount();

		if (currentKeyCount > previousKeyCount) {
			Keys[] keys = state.GetPressedKeys();
			Keys[] previousKeys = _previousState.GetPressedKeys();
			foreach (Keys key in keys) { // a remplacer par mieux
				if (!previousKeys.Contains(key)) {
					_currentKey = key;
					break;
				}
			}
			// if (state.CapsLock == false)
			// 	_currentKey -= 32;
			OnKeyPressed((char)_currentKey);
			_previousState = state;
			RestartWatch();
		}
		else if (currentKeyCount > 0 && currentKeyCount == previousKeyCount && _watch.ElapsedMilliseconds > 300 && _watch.ElapsedMilliseconds % 100 == 0) {
			OnKeyPressed((char)_currentKey);
		}
		else if (currentKeyCount < previousKeyCount) {
			_previousState = state;
			RestartWatch();
		}
	}

	private static void RestartWatch() {
		_watch.Stop();
		_watch.Reset();
		_watch.Start();
	}
}
