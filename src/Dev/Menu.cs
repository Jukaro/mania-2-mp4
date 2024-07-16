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

		GradientList gdList = new();
		gdList.Add(new(new(255, 0, 0), new(255, 255, 0)));
		gdList.Add(new(new(255, 255, 0), new(0, 255, 0)));
		gdList.Add(new(new(0, 255, 0), new(0, 255, 255)));
		gdList.Add(new(new(0, 255, 255), new(0, 0, 255)));
		gdList.Add(new(new(0, 0, 255), new(255, 0, 255)));
		gdList.Add(new(new(255, 0, 255), new(255, 0, 0)));
		_buttonContainer.SetGradientAsColor(gdList);

		ButtonVisuals buttonVisuals = new(_graphics, 100, 50, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};

		for (int i = 0; i < 20; i++)
			_buttonContainer.Add(new(_graphics, new(0, 0), "michel" + i, new ButtonVisuals(_graphics, 100, 50, new Color(Math.Min(255, i * 20), 0, 0)) {
				BlinkOnMouseClick = true,
				BlinkOnMouseOver = true
			}));

		_buttonContainer[2].SetColor(Color.AliceBlue);
		_buttonContainer["michel3"].SetColor(Color.Blue);
		_buttonContainer["michel3"].SetOnClick(() => Logger.LogDebug("coucou"));
		_buttonContainer["michel3"].ButtonVisuals.BlinkOnMouseOver = false;

		_buttonContainer["michel578"] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		// ajouter un bouton avec add
		// _buttonContainer.Add(new ButtonContainer(_graphics, 200, 200, new(0, 0), "secondButtonContainer", new(0, 0, 255)));

		// remplacer un bouton avec son index
		_buttonContainer[5] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "secondButtonContainer", new(0, 0, 255));

		// remplacer un bouton avec son nom
		// int index = _buttonContainer.GetIndexOfButton(_buttonContainer["michel5"]);
		// if (index != -1)
		// 	_buttonContainer[index] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "secondButtonContainer", new(0, 0, 255));

		// pas possible pour l'instant
		// _buttonContainer["michel5"] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "secondButtonContainer", new(0, 0, 255));
		// ContentManager content = new ContentManager();

		if (_buttonContainer["secondButtonContainer"] is ButtonContainer buttonContainer) {
			for (int i = 0; i < 5; i++)
				buttonContainer.Add(new(_graphics, 100, 10, new(0, 0), "pierre" + i, new Color(0, 255, 0)));
			buttonContainer[1] = new ButtonContainer(_graphics, 150, 100, new(0, 0), "thirdButtonContainer", new(255, 0, 255));
			GradientList gdList2 = new();
			gdList2.Add(new(Color.Black, Color.Red));
			gdList2.Add(new(Color.Red, Color.Black));
			buttonContainer[1].SetGradientAsColor(gdList2);
		}

	}

	public void アップデート(BeatmapPlayer beatmapPlayer, InputsPlayer inputsPlayer, AudioPlayer audioPlayer, ReplayData replay) {
		MouseManager.UpdateMouseState();

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

		_buttonContainer.Update();
	}

	public void レンダー(SpriteBatch spriteBatch) {
		_buttonContainer.Render(spriteBatch);
	}

}
