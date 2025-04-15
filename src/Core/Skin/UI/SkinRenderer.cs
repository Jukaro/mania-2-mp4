using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;

namespace Rythmify.UI;

public class LaneTextures {
	public AnimatedSkinTexture NoteTexture;
	public AnimatedSkinTexture HoldNoteBodyTexture;
	public AnimatedSkinTexture HoldNoteTailTexture;
	public AnimatedSkinTexture HoldNoteHeadTexture;
	public AnimatedSkinTexture InputTexture;
	public AnimatedSkinTexture InputTextureHeld;
}

public class SkinRenderer {
	private List<LaneTextures> _laneTextures = new();
	public Texture2D HitLineTexture;

	private ReplaySkinData _skin;
	private readonly GraphicsDevice _graphicsDevice;


	public SkinRenderer(ReplaySkinData skin, GraphicsDevice graphicsDevice) {
		_graphicsDevice = graphicsDevice;
		LoadSkin(skin);
	}

	public void LoadLaneTextures(int lane) {
		var noteTexturePath = _skin.ManiaSection.GetNoteImageLane(lane);
		var holdNoteBodyTexturePath = _skin.ManiaSection.GetNoteImageLaneL(lane);
		var inputTexturePath = _skin.ManiaSection.GetKeyImageLane(lane);
		var inputTextureHeldPath = _skin.ManiaSection.GetKeyImageLaneD(lane);

		var tailModifier = _skin.ManiaSection.ShouldFlipTail(lane) ? TextureModifier.FlipVertically : TextureModifier.None;

		AnimatedSkinTexture tailAnimatedTexture = new(_graphicsDevice, _skin.Data, _skin.ManiaSection.GetNoteImageLaneT(lane), tailModifier);
		if (tailAnimatedTexture.FrameCount == 0)
			tailAnimatedTexture = new(_graphicsDevice, _skin.Data, _skin.ManiaSection.GetNoteImageLaneH(lane), tailModifier);

		AnimatedSkinTexture headAnimatedTexture = new(_graphicsDevice, _skin.Data, _skin.ManiaSection.GetNoteImageLaneH(lane));
		if (headAnimatedTexture.FrameCount == 0)
			headAnimatedTexture = new(_graphicsDevice, _skin.Data, _skin.ManiaSection.GetNoteImageLane(lane));

		LaneTextures laneTextures = new() {
			NoteTexture = new(_graphicsDevice, _skin.Data, noteTexturePath),
			HoldNoteBodyTexture = new(_graphicsDevice, _skin.Data, holdNoteBodyTexturePath),
			HoldNoteTailTexture = tailAnimatedTexture,
			HoldNoteHeadTexture = headAnimatedTexture,
			InputTexture = new(_graphicsDevice, _skin.Data, inputTexturePath),
			InputTextureHeld = new(_graphicsDevice, _skin.Data, inputTextureHeldPath)
		};

		_laneTextures.Add(laneTextures);
	}

	public void LoadSkin(ReplaySkinData skin) {
		_skin = skin;

		HitLineTexture = new Texture2D(_graphicsDevice, 1, 4);
		var colors1 = new Color[1 * 4];
		Array.Fill(colors1, Color.Red);
		HitLineTexture.SetData(colors1);

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

	public Texture2D GetNoteTextureAtLane(int lane) => _laneTextures[lane].NoteTexture.GetCurrentFrame();
	public Texture2D GetHoldNoteTextureAtLane(int lane) => _laneTextures[lane].HoldNoteBodyTexture.GetCurrentFrame();
	public Texture2D GetHoldNoteHeadTextureAtLane(int lane) => _laneTextures[lane].HoldNoteHeadTexture.GetCurrentFrame();
	public Texture2D GetHoldNoteTailTextureAtLane(int lane) => _laneTextures[lane].HoldNoteTailTexture.GetCurrentFrame();
	public Texture2D GetInputTextureAtLane(int lane, bool held) => held ? _laneTextures[lane].InputTextureHeld.GetCurrentFrame() : _laneTextures[lane].InputTexture.GetCurrentFrame();
	public ReplaySkinData GetSkin() => _skin;
}
