using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Mania2mp4.Controls;

public partial class CardButtonControl : Button {
	public static readonly StyledProperty<object> HeadContentProperty =
		AvaloniaProperty.Register<CardButtonControl, object>(nameof(HeadContent));

	public object HeadContent
	{
		get => GetValue(HeadContentProperty);
		set => SetValue(HeadContentProperty, value);
	}

	public static readonly StyledProperty<object> BodyContentProperty =
		AvaloniaProperty.Register<CardButtonControl, object>(nameof(BodyContent));

	public object BodyContent
	{
		get => GetValue(BodyContentProperty);
		set => SetValue(BodyContentProperty, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		var head = e.NameScope.Find<Border>("PART_Head");
		head.CornerRadius = new CornerRadius(0, 0, CornerRadius.BottomRight, CornerRadius.BottomLeft);

		var darkOverlay1 = e.NameScope.Find<Border>("PART_DarkOverlay1");
		darkOverlay1.CornerRadius = new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0);

		var darkOverlay2 = e.NameScope.Find<Border>("PART_DarkOverlay2");
		darkOverlay2.CornerRadius = new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0);
	}

	public CardButtonControl() {

	}
}
