using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using System;
using System.Collections.Generic;

namespace Rythmify.UI;

public class Menu {
	private readonly GraphicsDevice _graphics;
	private KeyboardKey _volumeUp;
	private KeyboardKey _volumeDown;

	private KeyboardKey _numPad8;

	private KeyboardKey _F3;
	private KeyboardKey _F4;

	private ButtonContainer _buttonContainer;

	private bool _play;
	private bool _isPlaying;

	public Menu(GraphicsDevice graphics) {
		_graphics = graphics;
		_volumeUp = new(Keys.NumPad1);
		_volumeDown = new(Keys.NumPad2);
		_numPad8 = new(Keys.NumPad8);
		_F3 = new(Keys.F3);
		_F4 = new(Keys.F4);

		_play = false;
		_isPlaying = false;
	}

	public void InitGradientListsList(List<GradientList> gradientListsList) {
		GradientList RGBGradient = new();
		RGBGradient.Add(new(new(255, 0, 0), new(255, 255, 0)));
		RGBGradient.Add(new(new(255, 255, 0), new(0, 255, 0)));
		RGBGradient.Add(new(new(0, 255, 0), new(0, 255, 255)));
		RGBGradient.Add(new(new(0, 255, 255), new(0, 0, 255)));
		RGBGradient.Add(new(new(0, 0, 255), new(255, 0, 255)));
		RGBGradient.Add(new(new(255, 0, 255), new(255, 0, 0)));
		gradientListsList.Add(RGBGradient);

		GradientList BlackRedGradient = new();
		BlackRedGradient.Add(new(Color.Black, Color.Red));
		BlackRedGradient.Add(new(Color.Red, Color.Black));
		gradientListsList.Add(BlackRedGradient);
	}

	public void InitButtonVisualsList(List<ButtonVisuals> buttonVisualsList, List<GradientList> gradientListsList) {
		ButtonVisuals basicBlackButton = new(_graphics, 100, 50, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		buttonVisualsList.Add(basicBlackButton);

		ButtonVisuals basicRGBButton = new(_graphics, 300, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicRGBButton.SetGradientAsColor(gradientListsList[0], 10);
		buttonVisualsList.Add(basicRGBButton);

		ButtonVisuals basicBlackRedButton = new(_graphics, 100, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicBlackRedButton.SetGradientAsColor(gradientListsList[1], 100);
		buttonVisualsList.Add(basicBlackRedButton);

		ButtonVisuals basicThinBlackRedButton = new(_graphics, 100, 10, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicThinBlackRedButton.SetGradientAsColor(gradientListsList[1], 100);
		buttonVisualsList.Add(basicThinBlackRedButton);

		ButtonVisuals basicThickBlackRedButton = new(_graphics, 200, 200, Color.White) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		basicThickBlackRedButton.SetGradientAsColor(gradientListsList[0], 10);
		buttonVisualsList.Add(basicThickBlackRedButton);
	}

	public void Init() {
		List<GradientList> gradientListsList = new();
		List<ButtonVisuals> buttonVisualsList = new();
		InitGradientListsList(gradientListsList);
		InitButtonVisualsList(buttonVisualsList, gradientListsList);

		_buttonContainer = new(_graphics, 500, 1000, new(1000, 0), "firstButtonContainer", Color.Bisque);
		_buttonContainer.SetGradientAsColor(gradientListsList[0], 255);

		for (int i = 0; i < 10; i++) {
			_buttonContainer.Add(new(_graphics, new(0, 50 * i + 10 * i), "michel" + i, buttonVisualsList[0]));
			_buttonContainer[i].SetColor(new Color(Math.Min(255, i * 20), 0, 0));
		}

		_buttonContainer[2] = new Button(_graphics, new(0, 50 * 2 + 10 * 2), "michel2", buttonVisualsList[1]);
		_buttonContainer[3] = new SliderButton(_graphics, new(0, 50 * 3 + 10 * 3), "Slider", 1, 255, 10, (double d) => _buttonContainer.SetGradientAsColor(gradientListsList[0], (int)d), buttonVisualsList[1]);
		_buttonContainer[3].ButtonVisuals.BlinkOnMouseClick = false;
		_buttonContainer[3].ButtonVisuals.BlinkOnMouseOver = false;

		_buttonContainer.Add(new(_graphics, new(300, 50 + 10), "fixed1", buttonVisualsList[0]) {
			IsScrollable = false
		});

		_buttonContainer[4] = new ToggleButton(_graphics, new(0, 50 * 4 + 10 * 4), "Play", Play, Pause, buttonVisualsList[0]);

		// _buttonContainer["michel578"] = new ButtonContainer(_graphics, 200, 200, new(0, 0), "jsp", new(0, 0, 255));

		int pos1 = 1000;

		_buttonContainer[5] = new ButtonContainer(_graphics, 200, 200, new(0, pos1), "secondButtonContainer", new(0, 0, 255));
		// _buttonContainer[5].SetAbsolutePos(new Vector2(_buttonContainer.AbsolutePos.X, 2000)); // jsp quoi faire pour ça en relative
		// _buttonContainer[5] = new Button(_graphics, new(0, 2000), "secondButtonContainer", buttonVisualsList[0]);

		if (_buttonContainer["secondButtonContainer"] is ButtonContainer buttonContainer) {
			for (int i = 0; i < 5; i++)
				buttonContainer.Add(new(_graphics, 100, 10, new(0, 50 * i + 10 * i), "pierre" + i, new Color(0, 255, 0)));
			// buttonContainer[1] = new ButtonContainer(_graphics, 150, 100, new(0, 50 * 1 + 10 * 1), "thirdButtonContainer", new(255, 0, 255));
			// buttonContainer[1].SetGradientAsColor(gradientListsList[1], 255);
			// buttonContainer.Add(new SliderButton(_graphics, new(0, 0), "pierre4", -2, 2, 10, (double d) => Logger.LogDebug($"value: {d}"), buttonVisualsList[2]));
		}

		_buttonContainer[7] = new Dropdown(_graphics, new(0, 700), 1, "firstDropdown", buttonVisualsList[4]);
		if (_buttonContainer["firstDropdown"] is Dropdown dropdown) {
			for (int i = 0; i < 100; i++) {
				dropdown.Add(new(_graphics, new(15, 0), "drop" + i, buttonVisualsList[3]));
				dropdown[i].SetColor(new Color(0, 25 * (i % 10), 0));
			}
		}

		// Scrollbar test = new Scrollbar(_graphics, 50, 500, )

		// ContentManager contentManager = new(serviceProvider);

	}

	private void Play() {
		_play = true;
		_buttonContainer["Play"].SetColor(new Color(0, 255, 0));
	}

	private void Pause() {
		_play = false;
		_buttonContainer["Play"].SetColor(new Color(255, 0, 0));
	}

	public void アップデート(BeatmapPlayer beatmapPlayer, InputsPlayer inputsPlayer, AudioPlayer audioPlayer, ReplayData replay) {
		MouseManager.UpdateMouseState();

		if (_play && !_isPlaying) {
			beatmapPlayer.Play();
			inputsPlayer.Play();
			audioPlayer.Play();
			_isPlaying = true;
		}
		else if (!_play && _isPlaying) {
			beatmapPlayer.Pause();
			inputsPlayer.Pause();
			audioPlayer.Pause();
			_isPlaying = false;
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
