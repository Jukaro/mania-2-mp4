using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class InputsRenderer {
	private GraphicsDeviceManager _graphics;
	private SkinRenderer _skinRenderer;

	public InputsRenderer(GraphicsDeviceManager graphics, SkinRenderer skinRenderer) {
		_graphics = graphics;
		_skinRenderer = skinRenderer;
	}

	public void Render(InputsPlayer inputsPlayer, SpriteBatch spriteBatch) {
		if (inputsPlayer.RenderedInputs[0])
		{
			Vector2 screenSpacePos = new(0, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[1])
		{
			Vector2 screenSpacePos = new(150 * 1 + 30 * 1, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[2])
		{
			Vector2 screenSpacePos = new(150 * 2 + 30 * 2, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[3])
		{
			Vector2 screenSpacePos = new(150 * 3 + 30 * 3, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[4])
		{
			Vector2 screenSpacePos = new(150 * 4 + 30 * 4, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[5])
		{
			Vector2 screenSpacePos = new(150 * 5 + 30 * 5, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (inputsPlayer.RenderedInputs[6])
		{
			Vector2 screenSpacePos = new(150 * 6 + 30 * 6, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
	}
}
