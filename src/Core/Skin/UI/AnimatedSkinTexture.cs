using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
// using Microsoft.Xna.Framework;

namespace Rythmify.Core;

public enum TextureModifier {
	None,
	FlipVertically
}

public class AnimatedSkinTexture {
	private List<Bitmap> _frames = new();
	private int _currentFrame = 0;
	private bool _isPlaying = false;
	private float _duration = 1f;

	private float _time = 0f;

	public int FrameCount => _frames.Count;

	public AnimatedSkinTexture(SkinData skin, string texturePath, TextureModifier modifier = TextureModifier.None) {
		var filePath = skin.GetFilePath(texturePath);
		if (File.Exists(filePath)) {
			AddFrame(new Bitmap(filePath), modifier);
			return;
		}

		var extension = Path.GetExtension(filePath);
		var withoutExtension = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));

		for (int frameId = 0; true; frameId++) {
			var filename = withoutExtension + $"-{frameId}" + extension;
			if (!File.Exists(filename)) break;
			AddFrame(new Bitmap(filename), modifier);
		}

		if (_frames.Count == 0) {
			Logger.LogWarning($"No frames found for animated texture at {filePath}");
			return;
		}

		_duration = skin.General.AnimationFramerate == -1 ? 1f : 1f / skin.General.AnimationFramerate * _frames.Count;
	}

	private void AddFrame(Bitmap frame, TextureModifier modifier) {
		if (modifier == TextureModifier.FlipVertically) {
			// var data = new Color[frame.Width * frame.Height];
			// frame.GetData(data);
			// Array.Reverse(data);
			// frame.SetData(data);
			frame = Dispatcher.UIThread.Invoke(() => {
				int width = frame.PixelSize.Width;
				int height = frame.PixelSize.Height;

				var bitmap = new RenderTargetBitmap(new Avalonia.PixelSize(width, height), frame.Dpi);
				using (var context = bitmap.CreateDrawingContext())
				{
					// var flipMatrix = Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(0, height, 0);
					var flipMatrix = Matrix.CreateScale(1, -1) * Matrix.CreateTranslation(0, height);
					var transform = context.PushTransform(flipMatrix);
					context.DrawImage(frame, new Rect(0, 0, width, height));
					transform.Dispose();
				}
				return bitmap;
			});
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

	public Bitmap GetCurrentFrame() => _frames[_currentFrame];
	public void SetCurrentFrame(int frame) => _currentFrame = frame;

}
