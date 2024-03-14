using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class BeatmapRenderer
{
	private GraphicsDeviceManager _graphics;
	private SkinRenderer _skinRenderer;

	public BeatmapRenderer(GraphicsDeviceManager graphics, SkinRenderer skinRenderer) {
		_graphics = graphics;
		_skinRenderer = skinRenderer;
	}

	public void Render(BeatmapPlayer beatmapPlayer, SpriteBatch spriteBatch) {
		foreach (var note in beatmapPlayer.RenderedNotes) {
			if (note is HoldNote holdNote) {
				RenderHoldNote(holdNote, spriteBatch);
			} if (note is GameNote gameNote) {
				RenderGameNote(gameNote, spriteBatch);
			}
		}

		if (beatmapPlayer.RenderedInputs[0])
		{
			Vector2 screenSpacePos = new(0, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (beatmapPlayer.RenderedInputs[1])
		{
			Vector2 screenSpacePos = new(150 * 1 + 30 * 1, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (beatmapPlayer.RenderedInputs[2])
		{
			Vector2 screenSpacePos = new(150 * 2 + 30 * 2, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}
		if (beatmapPlayer.RenderedInputs[3])
		{
			Vector2 screenSpacePos = new(150 * 3 + 30 * 3, _graphics.PreferredBackBufferHeight - 200);
			spriteBatch.Draw(_skinRenderer.InputTexture, screenSpacePos, Color.White);
		}

		RenderHitLine(spriteBatch);
	}

	private float ToScreenSpaceY(double y) => (float)(y * _graphics.PreferredBackBufferHeight / BeatmapPlayer.PlayfieldHeight);
	private static float GetXFromLane(int lane, int laneWidth) => lane * laneWidth + 30 * lane;

	private static void DrawScaled(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 scale) =>
		spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

	private void RenderGameNote(GameNote note, SpriteBatch spriteBatch) {
		Vector2 screenSpacePos = new(
			GetXFromLane(note.Lane, _skinRenderer.HoldNoteBodyTexture.Width),
			ToScreenSpaceY(note.Y) - _skinRenderer.NoteTexture.Height
		);
		spriteBatch.Draw(_skinRenderer.NoteTexture, screenSpacePos, Color.White);
	}

	private void RenderHoldNote(HoldNote note, SpriteBatch spriteBatch) {
		float screenSpaceHoldNoteSize = ToScreenSpaceY(note.Height);
		float screenSpaceY = ToScreenSpaceY(note.Y);

		Vector2 screenSpaceHoldNoteTop = new(
			GetXFromLane(note.Lane, _skinRenderer.HoldNoteBodyTexture.Width),
			screenSpaceY - screenSpaceHoldNoteSize
		);

		Vector2 scale = new(1, screenSpaceHoldNoteSize / _skinRenderer.HoldNoteBodyTexture.Height);
		DrawScaled(spriteBatch, _skinRenderer.HoldNoteBodyTexture, screenSpaceHoldNoteTop, scale);
	}

	private void RenderHitLine(SpriteBatch spriteBatch) {
		Vector2 screenSpaceHitlinePos = new(0, ToScreenSpaceY(_skinRenderer.Skin.HitPosition));
		Vector2 hitlineScale = new(_graphics.PreferredBackBufferWidth, 1);
		DrawScaled(spriteBatch, _skinRenderer.HitLineTexture, screenSpaceHitlinePos, hitlineScale);
	}
}
