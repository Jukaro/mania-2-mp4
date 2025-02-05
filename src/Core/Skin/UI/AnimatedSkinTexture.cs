using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.Core;

public enum TextureModifier {
	None,
	FlipVertically
}

public class AnimatedSkinTexture {
	private List<Texture2D> _frames = new();
	private int _currentFrame = 0;
	private bool _isPlaying = false;
	private float _duration = 1f;

	private float _time = 0f;

	public int FrameCount => _frames.Count;

	public AnimatedSkinTexture(GraphicsDevice device, SkinData skin, string texturePath, TextureModifier modifier = TextureModifier.None) {
		var filePath = skin.GetFilePath(texturePath);
		if (File.Exists(filePath)) {
			AddFrame(Texture2D.FromFile(device, filePath), modifier);
			return;
		}

		var extension = Path.GetExtension(filePath);
		var withoutExtension = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));

		for (int frameId = 0; true; frameId++) {
			var filename = withoutExtension + $"-{frameId}" + extension;
			if (!File.Exists(filename)) break;
			AddFrame(Texture2D.FromFile(device, filename), modifier);
		}

		if (_frames.Count == 0) {
			Logger.LogError($"No frames found for animated texture at {filePath}");
			return;
		}

		_duration = skin.General.AnimationFramerate == -1 ? 1f : 1f / skin.General.AnimationFramerate * _frames.Count;
	}

	private void AddFrame(Texture2D frame, TextureModifier modifier) {
		if (modifier == TextureModifier.FlipVertically) {
			var data = new Color[frame.Width * frame.Height];
			frame.GetData(data);
			Array.Reverse(data);
			frame.SetData(data);
		}
		_frames.Add(frame);
	}

	public void Play() => _isPlaying = true;
	public void Pause() => _isPlaying = false;

	public void Update(float deltaTimeSeconds) {
		if (!_isPlaying) return;

		_time += deltaTimeSeconds;
		_currentFrame = (int)Math.Min(_time / _duration * _frames.Count, _frames.Count - 1);
	}

	public Texture2D GetCurrentFrame() => _frames[_currentFrame];
	public void SetCurrentFrame(int frame) => _currentFrame = frame;

}
