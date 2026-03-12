using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Rythmify.Core;

namespace Rythmify.UI;

public class LaneTextures {
	public AnimatedSkinTexture NoteTexture;
	public AnimatedSkinTexture HoldNoteBodyTexture;
	public AnimatedSkinTexture HoldNoteTailTexture;
	public AnimatedSkinTexture HoldNoteHeadTexture;
	public AnimatedSkinTexture? InputTexture;
	public AnimatedSkinTexture InputTextureHeld;
}

public class SkinRenderer {
	private List<LaneTextures> _laneTextures = new();
	public Bitmap HitLineTexture;

	private ReplaySkinData _skin;

	public SkinRenderer(ReplaySkinData skin) {
		LoadSkin(skin);
	}

	public void LoadLaneTextures(int lane) {
		var noteTexturePath = _skin.ManiaSection.GetNoteImageLane(lane);
		var holdNoteBodyTexturePath = _skin.ManiaSection.GetNoteImageLaneL(lane);
		var inputTexturePath = _skin.ManiaSection.GetKeyImageLane(lane);
		var inputTextureHeldPath = _skin.ManiaSection.GetKeyImageLaneD(lane);

		var tailModifier = _skin.ManiaSection.ShouldFlipTail(lane) ? TextureModifier.FlipVertically : TextureModifier.None;

		AnimatedSkinTexture tailAnimatedTexture = new(_skin.Data, _skin.ManiaSection.GetNoteImageLaneT(lane), tailModifier);
		if (tailAnimatedTexture.FrameCount == 0)
			tailAnimatedTexture = new(_skin.Data, _skin.ManiaSection.GetNoteImageLaneH(lane), tailModifier);

		AnimatedSkinTexture headAnimatedTexture = new(_skin.Data, _skin.ManiaSection.GetNoteImageLaneH(lane));
		if (headAnimatedTexture.FrameCount == 0)
			headAnimatedTexture = new(_skin.Data, _skin.ManiaSection.GetNoteImageLane(lane));

		AnimatedSkinTexture inputAnimatedTexture = new(_skin.Data, inputTexturePath);
		if (inputAnimatedTexture.FrameCount == 0)
			inputAnimatedTexture = null;

		AnimatedSkinTexture inputHeldAnimatedTexture = new(_skin.Data, inputTextureHeldPath);
		if (inputHeldAnimatedTexture.FrameCount == 0)
			inputHeldAnimatedTexture = new(_skin.Data, _skin.ManiaSection.GetNoteImageLane(lane));

		LaneTextures laneTextures = new() {
			NoteTexture = new(_skin.Data, noteTexturePath),
			HoldNoteBodyTexture = new(_skin.Data, holdNoteBodyTexturePath),
			HoldNoteTailTexture = tailAnimatedTexture,
			HoldNoteHeadTexture = headAnimatedTexture,
			InputTexture = inputAnimatedTexture,
			InputTextureHeld = inputHeldAnimatedTexture
		};

		_laneTextures.Add(laneTextures);
	}

	public void LoadSkin(ReplaySkinData skin) {
		_skin = skin;

		HitLineTexture = Dispatcher.UIThread.Invoke(() => {
			int width = 1;
			int height = 4;

			var bitmap = new RenderTargetBitmap(new PixelSize(width, height));
			using (var context = bitmap.CreateDrawingContext())
			{
				var brush = new SolidColorBrush(Colors.Red);
				context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
			}
			return bitmap;
		});

		for (int i = 0; i < 8; i++) LoadLaneTextures(i);
	}

	public void Update(float deltaTimeSeconds) {
		foreach (var laneTexture in _laneTextures) {
			laneTexture.NoteTexture.Update(deltaTimeSeconds);
			laneTexture.HoldNoteBodyTexture.Update(deltaTimeSeconds);
			laneTexture.HoldNoteTailTexture.Update(deltaTimeSeconds);
			laneTexture.HoldNoteHeadTexture.Update(deltaTimeSeconds);
			laneTexture.InputTexture.Update(deltaTimeSeconds);
			laneTexture.InputTextureHeld.Update(deltaTimeSeconds);
		}
	}

	public Bitmap GetNoteTextureAtLane(int lane) => _laneTextures[lane].NoteTexture.GetCurrentFrame();
	public Bitmap GetHoldNoteTextureAtLane(int lane) => _laneTextures[lane].HoldNoteBodyTexture.GetCurrentFrame();
	public Bitmap GetHoldNoteHeadTextureAtLane(int lane) => _laneTextures[lane].HoldNoteHeadTexture.GetCurrentFrame();
	public Bitmap GetHoldNoteTailTextureAtLane(int lane) => _laneTextures[lane].HoldNoteTailTexture.GetCurrentFrame();

	public Bitmap GetInputTextureAtLane(int lane, bool held) {
		if (held) return _laneTextures[lane].InputTextureHeld.GetCurrentFrame();
		else if (_laneTextures[lane].InputTexture != null) return _laneTextures[lane].InputTexture.GetCurrentFrame();
		return null;
	}

	public ReplaySkinData GetSkin() => _skin;
}
