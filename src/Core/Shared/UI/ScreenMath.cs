using Avalonia;
using Rythmify.Core;

namespace Rythmify.UI;

public class ScreenMath {
	private readonly Rect _bounds;

	public ScreenMath(Rect bounds) {
		_bounds = bounds;
	}

	public float PlayfieldToScreenSpaceY(double y) => (float)(y * _bounds.Height / Playfield.PlayfieldHeight);
	public float PlayfieldToScreenSpaceX(double x) => (float)(x * (_bounds.Height * (4/3f)) / Playfield.PlayfieldWidth);
	public float GetLaneX(int lane, SkinManiaSection maniaSectrion) => PlayfieldToScreenSpaceX(lane * maniaSectrion.ColumnWidth[lane]);
	public float GetLaneSize(int lane, SkinManiaSection maniaSection) => PlayfieldToScreenSpaceX(maniaSection.ColumnWidth[lane]);
}
