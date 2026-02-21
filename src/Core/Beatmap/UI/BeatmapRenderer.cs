using System.Numerics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class BeatmapRenderer
{
	private SkinRenderer _skinRenderer;
	private readonly ScreenMath _screenMath;
	private readonly Rect _bounds;

	public BeatmapRenderer(SkinRenderer skinRenderer, Rect bounds) {
		_skinRenderer = skinRenderer;
		_bounds = bounds;
		_screenMath = new ScreenMath(_bounds);
	}

	public void UpdateSkinRenderer(SkinRenderer skinRenderer) {
		_skinRenderer = skinRenderer;
	}

	private static Vector2 GetSizeAfterScale(Bitmap texture, float targetWidth) {
		float scale = targetWidth / (float)texture.Size.Width;
		return new((float)texture.Size.Width * scale, (float)texture.Size.Height * scale);
	}

	public void Render(BeatmapPlayer beatmapPlayer, DrawingContext drawingContext) {
		foreach (var note in beatmapPlayer.RenderedNotes) {
			if (note is HoldNote holdNote) {
				RenderHoldNote(holdNote, drawingContext);
			} else if (note is GameNote gameNote) {
				RenderGameNote(gameNote, drawingContext);
			}
		}

		RenderHitLine(drawingContext);
	}

	private void RenderGameNote(GameNote note, DrawingContext drawingContext) {
		var noteTexture = _skinRenderer.GetNoteTextureAtLane(note.Lane);

		var laneSize = _screenMath.GetLaneSize(note.Lane, _skinRenderer.GetSkin().ManiaSection);
		Vector2 scale = new(laneSize / (float)noteTexture.Size.Width, laneSize / (float)noteTexture.Size.Width);

		Vector2 screenSpacePos = new(
			_screenMath.GetLaneX(note.Lane, _skinRenderer.GetSkin().ManiaSection),
			_screenMath.PlayfieldToScreenSpaceY(note.Y) - (float)noteTexture.Size.Height * scale.Y
		);

		Rendering.DrawScaled(drawingContext, noteTexture, screenSpacePos, scale);
	}

	private void RenderHoldNote(HoldNote note, DrawingContext drawingContext) {
		var holdNoteHeadTexture = _skinRenderer.GetHoldNoteHeadTextureAtLane(note.Lane);

		var laneX = _screenMath.GetLaneX(note.Lane, _skinRenderer.GetSkin().ManiaSection);
		var laneSize = _screenMath.GetLaneSize(note.Lane, _skinRenderer.GetSkin().ManiaSection);

		float screenSpaceHoldNoteSize = _screenMath.PlayfieldToScreenSpaceY(note.Height) - (GetSizeAfterScale(holdNoteHeadTexture, laneSize).Y / 2);
		float screenSpaceY = _screenMath.PlayfieldToScreenSpaceY(note.Y) - (GetSizeAfterScale(holdNoteHeadTexture, laneSize).Y / 2);
		var holdNoteTexture = _skinRenderer.GetHoldNoteTextureAtLane(note.Lane);
		var holdNoteTailTexture = _skinRenderer.GetHoldNoteTailTextureAtLane(note.Lane);


		Vector2 screenSpaceHoldNoteTop = new(laneX, screenSpaceY - screenSpaceHoldNoteSize);
		Vector2 scale = new(laneSize / (float)holdNoteTexture.Size.Width, screenSpaceHoldNoteSize / (float)holdNoteTexture.Size.Height);
		Rendering.DrawScaled(drawingContext, holdNoteTexture, screenSpaceHoldNoteTop, scale);

		var tailScale = laneSize / (float)holdNoteTailTexture.Size.Width;
		Vector2 screenSpaceHoldNoteTailTop = new(laneX, screenSpaceHoldNoteTop.Y - (float)holdNoteTailTexture.Size.Height * tailScale);
		Rendering.DrawScaled(drawingContext, holdNoteTailTexture, screenSpaceHoldNoteTailTop, new(tailScale, tailScale));

		var headScale = laneSize / (float)holdNoteHeadTexture.Size.Width;

		Vector2 headPos = new(laneX, _screenMath.PlayfieldToScreenSpaceY(note.Y) - (float)holdNoteHeadTexture.Size.Height * headScale);
		Rendering.DrawScaled(drawingContext, holdNoteHeadTexture, headPos, new(headScale, headScale));
	}

	private void RenderHitLine(DrawingContext drawingContext) {
		var hitLineY = _screenMath.PlayfieldToScreenSpaceY(_skinRenderer.GetSkin().ManiaSection.HitPosition);
		Vector2 screenSpaceHitlinePos = new(0, hitLineY);
		Vector2 hitlineScale = new((float)_bounds.Width, 1);
		Rendering.DrawScaled(drawingContext, _skinRenderer.HitLineTexture, screenSpaceHitlinePos, hitlineScale);
	}
}
