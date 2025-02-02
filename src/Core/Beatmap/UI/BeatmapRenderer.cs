using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class BeatmapRenderer
{
	private readonly GraphicsDeviceManager _graphics;
	private readonly SkinRenderer _skinRenderer;
	private readonly ScreenMath _screenMath;

	public BeatmapRenderer(GraphicsDeviceManager graphics, SkinRenderer skinRenderer) {
		_graphics = graphics;
		_skinRenderer = skinRenderer;
		_screenMath = new ScreenMath(graphics);
	}

	private static Vector2 GetSizeAfterScale(Texture2D texture, float targetWidth) {
		float scale = targetWidth / texture.Width;
		return new(texture.Width * scale, texture.Height * scale);
	}

	public void Render(BeatmapPlayer beatmapPlayer, SpriteBatch spriteBatch) {
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

		foreach (var note in beatmapPlayer.RenderedNotes) {
			if (note is HoldNote holdNote) {
				RenderHoldNote(holdNote, spriteBatch);
			} else if (note is GameNote gameNote) {
				RenderGameNote(gameNote, spriteBatch);
			}
		}
		RenderHitLine(spriteBatch);

		spriteBatch.End();
	}

	private void RenderGameNote(GameNote note, SpriteBatch spriteBatch) {
		var noteTexture = _skinRenderer.GetNoteTextureAtLane(note.Lane);

		var laneSize = _screenMath.GetLaneSize(note.Lane, _skinRenderer.GetSkin().ManiaSection);
		Vector2 scale = new(laneSize / noteTexture.Width, laneSize / noteTexture.Width);

		Vector2 screenSpacePos = new(
			_screenMath.GetLaneX(note.Lane, _skinRenderer.GetSkin().ManiaSection),
			_screenMath.PlayfieldToScreenSpaceY(note.Y) - noteTexture.Height * scale.Y
		);

		Rendering.DrawScaled(spriteBatch, noteTexture, screenSpacePos, scale);
	}

	private void RenderHoldNote(HoldNote note, SpriteBatch spriteBatch) {
		var holdNoteHeadTexture = _skinRenderer.GetHoldNoteHeadTextureAtLane(note.Lane);

		var laneX = _screenMath.GetLaneX(note.Lane, _skinRenderer.GetSkin().ManiaSection);
		var laneSize = _screenMath.GetLaneSize(note.Lane, _skinRenderer.GetSkin().ManiaSection);

		float screenSpaceHoldNoteSize = _screenMath.PlayfieldToScreenSpaceY(note.Height) - (GetSizeAfterScale(holdNoteHeadTexture, laneSize).Y / 2);
		float screenSpaceY = _screenMath.PlayfieldToScreenSpaceY(note.Y) - (GetSizeAfterScale(holdNoteHeadTexture, laneSize).Y / 2);
		var holdNoteTexture = _skinRenderer.GetHoldNoteTextureAtLane(note.Lane);
		var holdNoteTailTexture = _skinRenderer.GetHoldNoteTailTextureAtLane(note.Lane);


		Vector2 screenSpaceHoldNoteTop = new(laneX, screenSpaceY - screenSpaceHoldNoteSize);
		Vector2 scale = new(laneSize / holdNoteTexture.Width, screenSpaceHoldNoteSize / holdNoteTexture.Height);
		Rendering.DrawScaled(spriteBatch, holdNoteTexture, screenSpaceHoldNoteTop, scale);

		var tailScale = laneSize / holdNoteTailTexture.Width;
		Vector2 screenSpaceHoldNoteTailTop = new(laneX, screenSpaceHoldNoteTop.Y - holdNoteTailTexture.Height * tailScale);
		Rendering.DrawScaled(spriteBatch, holdNoteTailTexture, screenSpaceHoldNoteTailTop, new(tailScale, tailScale), SpriteEffects.FlipVertically);

		var headScale = laneSize / holdNoteHeadTexture.Width;

		Vector2 headPos = new(laneX, _screenMath.PlayfieldToScreenSpaceY(note.Y) - holdNoteHeadTexture.Height * headScale);
		Rendering.DrawScaled(spriteBatch, holdNoteHeadTexture, headPos, new(headScale, headScale));
	}

	private void RenderHitLine(SpriteBatch spriteBatch) {
		var hitLineY = _screenMath.PlayfieldToScreenSpaceY(_skinRenderer.GetSkin().ManiaSection.HitPosition);
		Vector2 screenSpaceHitlinePos = new(0, hitLineY);
		Vector2 hitlineScale = new(_graphics.PreferredBackBufferWidth, 1);
		Rendering.DrawScaled(spriteBatch, _skinRenderer.HitLineTexture, screenSpaceHitlinePos, hitlineScale);
	}
}
