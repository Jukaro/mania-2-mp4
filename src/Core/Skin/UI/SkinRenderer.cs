using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;

namespace Rythmify.UI;

public class SkinRenderer {
	private Texture2D[] _noteTextures;
	public Texture2D HitLineTexture;
	private Texture2D[] _holdNoteBodyTextures;
	private Texture2D[] _inputTextures;
	private Texture2D[] _inputTexturesHeld;

	private ReplaySkinData _skin;
	private readonly GraphicsDevice _graphicsDevice;


	public SkinRenderer(ReplaySkinData skin, GraphicsDevice graphicsDevice) {
		_graphicsDevice = graphicsDevice;
		LoadSkin(skin);
	}

	public void LoadSkin(ReplaySkinData skin) {
		_skin = skin;

		_noteTextures = new Texture2D[_skin.LaneCount];
		for (int i = 0; i < _skin.LaneCount; i++) {
			var filePath = _skin.GetFilePath(_skin.ManiaSection.NoteImageLanes[i]);
			Logger.LogDebug($"Loading note texture at lane {i} from {filePath}");
			_noteTextures[i] = Texture2D.FromFile(_graphicsDevice, filePath);
		}

		_holdNoteBodyTextures = new Texture2D[_skin.LaneCount];
		for (int i = 0; i < _skin.LaneCount; i++) {
			var filePath = _skin.GetFilePath(_skin.ManiaSection.NoteImageLanesL[i]);
			Logger.LogDebug($"Loading hold note body texture at lane {i} from {filePath}");
			_holdNoteBodyTextures[i] = Texture2D.FromFile(_graphicsDevice, filePath);
		}

		HitLineTexture = new(_graphicsDevice, 1, 4);
		var colors1 = new Color[1 * 4];
		Array.Fill(colors1, Color.Red);
		HitLineTexture.SetData(colors1);

		_inputTextures = new Texture2D[_skin.LaneCount];
		for (int i = 0; i < _skin.LaneCount; i++) {
			var filePath = _skin.GetFilePath(_skin.ManiaSection.KeyImageLanes[i]);
			Logger.LogDebug($"Loading input texture at lane {i} from {filePath}");
			_inputTextures[i] = Texture2D.FromFile(_graphicsDevice, filePath);
		}

		_inputTexturesHeld = new Texture2D[_skin.LaneCount];
		for (int i = 0; i < _skin.LaneCount; i++) {
			var filePath = _skin.GetFilePath(_skin.ManiaSection.KeyImageLanesD[i]);
			Logger.LogDebug($"Loading input texture at lane {i} from {filePath}");
			_inputTexturesHeld[i] = Texture2D.FromFile(_graphicsDevice, filePath);
		}
	}

	public Texture2D GetNoteTextureAtLane(int lane) => _noteTextures[lane];
	public Texture2D GetHoldNoteTextureAtLane(int lane) => _holdNoteBodyTextures[lane];
	public Texture2D GetInputTextureAtLane(int lane, bool held) => held ? _inputTexturesHeld[lane] : _inputTextures[lane];
	public ReplaySkinData GetSkin() => _skin;
}
