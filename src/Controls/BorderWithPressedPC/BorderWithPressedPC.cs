using System;
using Avalonia.Controls;

namespace Mania2mp4.Controls;

public class BorderWithPressedPC : Border {
	protected override Type StyleKeyOverride => typeof(Border);

	public BorderWithPressedPC() {
		PointerPressed += (_, _) => PseudoClasses.Add(":pressed");
		PointerReleased += (_, _) => PseudoClasses.Remove(":pressed");
	}
}
