using Microsoft.Xna.Framework;
using Rythmify.Core;

namespace Rythmify.UI;

public class ScreenMath {
	private readonly GraphicsDeviceManager _graphics;

	public ScreenMath(GraphicsDeviceManager graphics) {
		_graphics = graphics;
	}

	public float PlayfieldToScreenSpaceY(double y) => (float)(y * _graphics.PreferredBackBufferHeight / Playfield.PlayfieldHeight);
	public float PlayfieldToScreenSpaceX(double x) => (float)(x * (_graphics.PreferredBackBufferHeight * Playfield.AspectRatio) / Playfield.PlayfieldWidth);
	public float GetLaneX(int lane, SkinManiaSection maniaSectrion) => PlayfieldToScreenSpaceX(lane * maniaSectrion.ColumnWidth[lane]);
	public float GetLaneSize(int lane, SkinManiaSection maniaSection) => PlayfieldToScreenSpaceX(maniaSection.ColumnWidth[lane]);
}
