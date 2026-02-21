using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Mania2mp4.DataValidation;

namespace Mania2mp4.Controls;

[TemplatePart("PART_Header", typeof(TextBlock))]
[TemplatePart("PART_CustomTextBox", typeof(CustomTextBox), IsRequired = true)]
[TemplatePart("PART_Slider", typeof(Slider), IsRequired = true)]
public class SliderWithInput : TemplatedControl {
	private CustomTextBox? _input;
	private Slider? _slider;
	private TextBlock? _header;

	public static readonly StyledProperty<double> MinimumProperty;
	public static readonly StyledProperty<double> MaximumProperty;
	public static readonly StyledProperty<double> SmallChangeProperty;
	public static readonly StyledProperty<double> LargeChangeProperty;
	public static readonly StyledProperty<double> TickFrequencyProperty;
	public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty;

	public static readonly StyledProperty<Func<string, ValidationResult>> ValidationFunctionProperty;
	public static readonly StyledProperty<string> HeaderProperty;

	private static readonly DirectProperty<SliderWithInput, double> _sliderValueProperty;

	public static readonly DirectProperty<SliderWithInput, double?> OutputProperty;

	private double? _output;
	private double _sliderValue;

	public double Minimum {
		get { return GetValue(MinimumProperty); }
		set { SetValue(MinimumProperty, value); }
	}

	public double Maximum {
		get { return GetValue(MaximumProperty); }
		set { SetValue(MaximumProperty, value); }
	}

	public double SmallChange {
		get { return GetValue(SmallChangeProperty); }
		set { SetValue(SmallChangeProperty, value); }
	}

	public double LargeChange {
		get { return GetValue(LargeChangeProperty); }
		set { SetValue(LargeChangeProperty, value); }
	}

	public double TickFrequency {
		get { return GetValue(TickFrequencyProperty); }
		set { SetValue(TickFrequencyProperty, value); }
	}

	public bool IsSnapToTickEnabled {
		get { return GetValue(IsSnapToTickEnabledProperty); }
		set { SetValue(IsSnapToTickEnabledProperty, value); }
	}

	public double? Output {
		get { return _output; }
		set { SetAndRaise(OutputProperty, ref _output, value); }
	}

	public Func<string, ValidationResult> ValidationFunction {
		get { return GetValue(ValidationFunctionProperty); }
		set { SetValue(ValidationFunctionProperty, value); }
	}

	public string Header {
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public double SliderValue {
		get { return _sliderValue; }
		set { SetAndRaise(_sliderValueProperty, ref _sliderValue, value);  }
	}

	static SliderWithInput() {
		MinimumProperty = AvaloniaProperty.Register<SliderWithInput, double>(nameof(Minimum));
		MaximumProperty = AvaloniaProperty.Register<SliderWithInput, double>(nameof(Maximum));
		SmallChangeProperty = AvaloniaProperty.Register<SliderWithInput, double>(nameof(SmallChange));
		LargeChangeProperty = AvaloniaProperty.Register<SliderWithInput, double>(nameof(LargeChange));
		TickFrequencyProperty = AvaloniaProperty.Register<SliderWithInput, double>(nameof(TickFrequency));
		IsSnapToTickEnabledProperty = AvaloniaProperty.Register<SliderWithInput, bool>(nameof(IsSnapToTickEnabled));

		ValidationFunctionProperty = AvaloniaProperty.Register<SliderWithInput, Func<string, ValidationResult>>(nameof(ValidationFunction));
		HeaderProperty = AvaloniaProperty.Register<FormField, string>(nameof(Header));

		OutputProperty = AvaloniaProperty.RegisterDirect<SliderWithInput, double?>(
			nameof(Output),
			o => o.Output,
			(o, v) => o.Output = v,
			defaultBindingMode: BindingMode.TwoWay
		);

		_sliderValueProperty = AvaloniaProperty.RegisterDirect<SliderWithInput, double>(
			nameof(SliderValue),
			o => o.SliderValue,
			(o, v) => o.SliderValue = v,
			defaultBindingMode: BindingMode.TwoWay
		);
	}

	public SliderWithInput() {
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
		if (e.Property == OutputProperty) {
			if (Output != null && (double)Output != SliderValue) {
				// Logger.LogDebug($"output prop changed (slider: {_sliderValue}, output: {_output})");
				SliderValue = (double)Output;
			} else if (Output == null && SliderValue != 0) {
				SliderValue = 0;
				Output = null;
			}
		} else if (e.Property == _sliderValueProperty) {
			// Logger.LogDebug($"sliderValue prop changed (slider: {_sliderValue}, output: {_output}, HasErrors: {DataValidationErrors.GetHasErrors(_input)})");
			if (Output == null && !DataValidationErrors.GetHasErrors(_input) || (Output != null && (double)Output != SliderValue))
				Output = SliderValue;
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		_input = e.NameScope.Find<CustomTextBox>("PART_CustomTextBox");
		_slider = e.NameScope.Find<Slider>("PART_Slider");
		_header = e.NameScope.Find<TextBlock>("PART_Header");

		if (_header != null) {
			_header.Text = Header;
		}

		_slider.Bind(Slider.ValueProperty, new Binding {
			Path = nameof(SliderValue),
			Source = this,
			Mode = BindingMode.TwoWay
		});
		_input.Bind(CustomTextBox.OutputProperty, new Binding {
			Path = nameof(Output),
			Source = this,
			Mode = BindingMode.TwoWay
		});
		_input.ValidationFunction = ValidateDouble;

		_slider.Bind(Slider.MinimumProperty, new Binding(nameof(Minimum)) { Source = this });
		_slider.Bind(Slider.MaximumProperty, new Binding(nameof(Maximum)) { Source = this });
		_slider.Bind(Slider.SmallChangeProperty, new Binding(nameof(SmallChange)) { Source = this });
		_slider.Bind(Slider.LargeChangeProperty, new Binding(nameof(LargeChange)) { Source = this });
		_slider.Bind(Slider.TickFrequencyProperty, new Binding(nameof(TickFrequency)) { Source = this });
		_slider.Bind(Slider.IsSnapToTickEnabledProperty, new Binding(nameof(IsSnapToTickEnabled)) { Source = this });

	}

	public ValidationResult ValidateDouble(string strValue) {
			double res;

			if (!double.TryParse(strValue, out res))
				return new (null, "Not a double");
			else if (res < Minimum)
				return new (null, $"Value must be greater than {Minimum}");
			else if (res > Maximum)
				return new (null, $"Value must be smaller than {Maximum}");
			return new (res, null);
	}
}
