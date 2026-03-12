using System.Numerics;
using Avalonia;
using Avalonia.Media;
using Rythmify.Core;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class InputsRenderer {
	private readonly Rect _bounds;
	private SkinRenderer _skinRenderer;
	private readonly ScreenMath _screenMath;

	public InputsRenderer(SkinRenderer skinRenderer, Rect bounds) {
		_bounds = bounds;
		_skinRenderer = skinRenderer;
		_screenMath = new(_bounds);
	}

	public void UpdateSkinRenderer(SkinRenderer skinRenderer) {
		_skinRenderer = skinRenderer;
	}

	public void Render(InputsPlayer inputsPlayer, DrawingContext drawingContext) {
		for (int i = 0; i < inputsPlayer.RenderedInputs.Length; i++) {
			var inputTexture = _skinRenderer.GetInputTextureAtLane(i, inputsPlayer.RenderedInputs[i]);
			if (inputTexture == null) continue;

			float laneSize = _screenMath.GetLaneSize(i, _skinRenderer.GetSkin().ManiaSection);

			Vector2 scale = new(laneSize / (float)inputTexture.Size.Width, Playfield.AspectRatio);

			Vector2 screenSpacePos = new(
				_screenMath.GetLaneX(i, _skinRenderer.GetSkin().ManiaSection),
				(float)_bounds.Height - (float)inputTexture.Size.Height * scale.Y
			);

			Rendering.DrawScaled(drawingContext, inputTexture, screenSpacePos, scale);
		}
	}
}
