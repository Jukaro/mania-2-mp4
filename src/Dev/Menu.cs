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

	private ButtonContainer _buttonContainer;

	private int _mouseWheelState;

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

	public void Init() {
		_buttonContainer = new(_graphics, 500, 1000, new(1000, 0), "firstButtonContainer", Color.Bisque);
		for (int i = 0; i < 20; i++)
			_buttonContainer.Add(new(_graphics, 100, 50, new(0, 0), "michel" + i, new(Math.Min(255, i * 20), 0, 0)));

		_buttonContainer[2].SetColor(Color.AliceBlue);
		_buttonContainer["michel3"].SetColor(Color.Crimson);

		_buttonContainer["michel578"] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		// ajouter un bouton avec add
		// _buttonContainer.Add(new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255)));

		// remplacer un bouton avec son index
		// _buttonContainer[5] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		// remplacer un bouton avec son nom
		// int index = _buttonContainer.GetIndexOfButton(_buttonContainer["michel5"]);
		// if (index != -1)
			// _buttonContainer[index] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		// pas possible pour l'instant
		// _buttonContainer["michel5"] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		// if (_buttonContainer["jsp"] is ButtonContainer buttonContainer)
		// 	for (int i = 0; i < 5; i++)
		// 		buttonContainer.Add(new(_graphics, 100, 10, new(0, 0), "michel" + i, new(0, 255, 0)));
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
		_buttonContainer.Render(spriteBatch);
	}

}
