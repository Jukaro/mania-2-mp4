using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using System;

namespace Rythmify.UI;

public class Menu {
	private readonly GraphicsDevice _graphics;
	private KeyboardKey _volumeUp;
	private KeyboardKey _volumeDown;

	private KeyboardKey _numPad4;
	private KeyboardKey _numPad5;

	private KeyboardKey _numPad8;

	private KeyboardKey _F3;
	private KeyboardKey _F4;

	public Menu(GraphicsDevice graphics) {
		_graphics = graphics;
		_volumeUp = new(Keys.NumPad1);
		_volumeDown = new(Keys.NumPad2);
		_numPad4 = new(Keys.NumPad4);
		_numPad5 = new(Keys.NumPad5);
		_numPad8 = new(Keys.NumPad8);
		_F3 = new(Keys.F3);
		_F4 = new(Keys.F4);
	}

	public void Update(BeatmapPlayer beatmapPlayer, InputsPlayer inputsPlayer, AudioPlayer audioPlayer, ReplayData replay) {
		if (_numPad4.IsPressed()) {
			beatmapPlayer.Play();
			inputsPlayer.Play();
			audioPlayer.Play();
		}
		else if (_numPad5.IsPressed()) {
			beatmapPlayer.Pause();
			inputsPlayer.Pause();
			audioPlayer.Pause();
		}

		if (_numPad8.IsPressed()) {
			beatmapPlayer.Reset(replay);
			inputsPlayer.Init(replay);
			audioPlayer.Reset();

			beatmapPlayer.Play();
			inputsPlayer.Play();
			audioPlayer.Play();
		}

		if (_volumeUp.IsPressed())
			audioPlayer.VolumeUp();
		if (_volumeDown.IsPressed())
			audioPlayer.VolumeDown();

		if (_F3.IsPressed())
			beatmapPlayer.ScrollSpeedDown();

		if (_F4.IsPressed())
			beatmapPlayer.ScrollSpeedUp();
	}

	public void レンダー(SpriteBatch spriteBatch) {

	}

}
