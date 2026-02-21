using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Mania2mp4.Controls;

public partial class SessionCalendarControl : Calendar {
	protected override Type StyleKeyOverride => typeof(Calendar);

	public static readonly StyledProperty<IReadOnlyList<DateTime>> HighlightedDatesProperty =
		AvaloniaProperty.Register<SessionCalendarControl, IReadOnlyList<DateTime>>(nameof(HighlightedDates));

	public IReadOnlyList<DateTime> HighlightedDates
	{
		get => GetValue(HighlightedDatesProperty);
		set => SetValue(HighlightedDatesProperty, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		PropertyChanged += (_, _) => {
			ApplyHighlights();
			if (HighlightedDates != null) {
				DisplayDateStart = HighlightedDates.Min();
			}
		};
		DisplayDateChanged += (_, _) => ApplyHighlights();
		Loaded += (_, _) => ApplyHighlights();
	}

	private void ApplyHighlights()
	{
		if (HighlightedDates is null)
			return;

		var dayButtons = this.GetVisualDescendants().OfType<CalendarDayButton>();

		foreach (var btn in dayButtons)
		{
			if (btn.DataContext is DateTime date)
			{
				if (HighlightedDates.Contains(date.Date))
				{
					btn.Background = Brushes.DeepPink;
					btn.FontWeight = Avalonia.Media.FontWeight.Bold;
				}
				else
				{
					btn.ClearValue(BackgroundProperty);
					btn.ClearValue(FontWeightProperty);
				}
			}
		}
	}

	public SessionCalendarControl() {
		DisplayDateEnd = DateTime.Today;
	}
}
