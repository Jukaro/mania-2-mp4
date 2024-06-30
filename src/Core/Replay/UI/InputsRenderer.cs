using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class InputsRenderer {
	private readonly GraphicsDeviceManager _graphics;
	private readonly SkinRenderer _skinRenderer;

	public InputsRenderer(GraphicsDeviceManager graphics, SkinRenderer skinRenderer) {
		_graphics = graphics;
		_skinRenderer = skinRenderer;
	}

	public void Render(InputsPlayer inputsPlayer, SpriteBatch spriteBatch) {
		for (int i = 0; i < inputsPlayer.RenderedInputs.Length; i++) {
			if (inputsPlayer.RenderedInputs[i]) {
				Vector2 screenSpacePos = new(150 * i + 30 * i, _graphics.PreferredBackBufferHeight - 200);
				spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
			}
		}
	}
}
