using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class InputsRenderer {
	private readonly GraphicsDeviceManager _graphics;
	private readonly SkinRenderer _skinRenderer;
	private readonly ScreenMath _screenMath;

	public InputsRenderer(GraphicsDeviceManager graphics, SkinRenderer skinRenderer) {
		_graphics = graphics;
		_skinRenderer = skinRenderer;
		_screenMath = new(graphics);
	}

	public void Render(InputsPlayer inputsPlayer, SpriteBatch spriteBatch) {
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

		for (int i = 0; i < inputsPlayer.RenderedInputs.Length; i++) {
			var inputTexture = _skinRenderer.GetInputTextureAtLane(i, inputsPlayer.RenderedInputs[i]);

			float laneSize = _screenMath.GetLaneSize(i, _skinRenderer.GetSkin().ManiaSection);


			Vector2 scale = new(laneSize / inputTexture.Width, Playfield.AspectRatio);
			Console.WriteLine(scale.X);

			Vector2 screenSpacePos = new(
				_screenMath.GetLaneX(i, _skinRenderer.GetSkin().ManiaSection),
				_graphics.PreferredBackBufferHeight - inputTexture.Height * scale.Y
			);

			Rendering.DrawScaled(spriteBatch, inputTexture, screenSpacePos, scale);
		}

		spriteBatch.End();
	}
}
