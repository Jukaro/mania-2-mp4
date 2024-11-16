using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
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

	public void Render(BeatmapPlayer beatmapPlayer, SpriteBatch spriteBatch) {
		foreach (var note in beatmapPlayer.RenderedNotes) {
			if (note is HoldNote holdNote) {
				RenderHoldNote(holdNote, spriteBatch);
			}

			if (note is GameNote gameNote) {
				RenderGameNote(gameNote, spriteBatch);
			}
		}

		RenderHitLine(spriteBatch);
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
		float screenSpaceHoldNoteSize = _screenMath.PlayfieldToScreenSpaceY(note.Height);
		float screenSpaceY = _screenMath.PlayfieldToScreenSpaceY(note.Y);
		var holdNoteTexture = _skinRenderer.GetHoldNoteTextureAtLane(note.Lane);

		Vector2 screenSpaceHoldNoteTop = new(
			_screenMath.GetLaneX(note.Lane, _skinRenderer.GetSkin().ManiaSection),
			screenSpaceY - screenSpaceHoldNoteSize
		);

		var laneSize = _screenMath.GetLaneSize(note.Lane, _skinRenderer.GetSkin().ManiaSection);
		Vector2 scale = new(laneSize / holdNoteTexture.Width, screenSpaceHoldNoteSize / holdNoteTexture.Height);
		Rendering.DrawScaled(spriteBatch, holdNoteTexture, screenSpaceHoldNoteTop, scale);
	}

	private void RenderHitLine(SpriteBatch spriteBatch) {
		var hitLineY = _screenMath.PlayfieldToScreenSpaceY(_skinRenderer.GetSkin().ManiaSection.HitPosition);
		Vector2 screenSpaceHitlinePos = new(0, hitLineY);
		Vector2 hitlineScale = new(_graphics.PreferredBackBufferWidth, 1);
		Rendering.DrawScaled(spriteBatch, _skinRenderer.HitLineTexture, screenSpaceHitlinePos, hitlineScale);
	}
}
