using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Mania2mp4.DataValidation;
using Rythmify.Core;

namespace Mania2mp4.Controls;

[TemplatePart("PART_FieldInput", typeof(TextBox), IsRequired = true)]
[TemplatePart("PART_FieldHeader", typeof(TextBlock))]
public class FormField : TemplatedControl {
	public const string PartFieldInput = "PART_FieldInput";
	public const string PartFieldHeader = "PART_FieldHeader";

	private TextBox? _fieldInput;
	private TextBlock? _fieldHeader;

	private string? _text;

	public static readonly DirectProperty<FormField, object?> OutputProperty =
		AvaloniaProperty.RegisterDirect<FormField, object?>(
			nameof(Output),
			o => o.Output,
			(o, v) => o.Output = v,
			defaultBindingMode: BindingMode.TwoWay
		);

	private object? _output;

	public object? Output {
		get { return _output; }
		set {
			SetAndRaise(OutputProperty, ref _output, value);
			if (_fieldInput != null)
				_fieldInput.Text = value != null ? value.ToString() : null;
		}
	}

	public static readonly StyledProperty<string> HeaderProperty =
		AvaloniaProperty.Register<FormField, string>(nameof(Header));

	public string Header {
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public static readonly StyledProperty<Func<string, ValidationResult>> ValidationFunctionProperty =
		AvaloniaProperty.Register<FormField, Func<string, ValidationResult>>(nameof(ValidationFunction));

	public Func<string, ValidationResult> ValidationFunction {
		get => GetValue(ValidationFunctionProperty);
		set => SetValue(ValidationFunctionProperty, value);
	}

	private string? _errorMessage;

	public FormField() {

	}

	private void OnTextChanged(string? newText) {
		_text = newText;
		UpdateErrors();
	}

	private async void UpdateErrors()
	{
		object result = null;

		if (string.IsNullOrEmpty(_text)) {
			Update(null, null);
			return;
		}

		if (ValidationFunction != null) {
			ValidationResult validationResult = ValidationFunction(_text);

			if (validationResult.Value == null) {
				Update(null, validationResult.Error);
				return;
			}

			result = validationResult.Value;
		} else {
			result = _text;
		}

		// Logger.LogDebug($"result: {result}");

		Update(result, null);
	}

	private void Update(object? output, string? errorMessage) {
		SetAndRaise(OutputProperty, ref _output, output);

		if (_errorMessage == errorMessage)
			return ;

		Logger.LogDebug($"errorMessage: {errorMessage}");

		_errorMessage = errorMessage;
		Exception? error = errorMessage == null ? null : new ToStringeableException(errorMessage);
		DataValidationErrors.SetError(this, error);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		_fieldInput = e.NameScope.Find<TextBox>(PartFieldInput);
		_fieldHeader = e.NameScope.Find<TextBlock>(PartFieldHeader);

		if (_fieldHeader != null)
			_fieldHeader.Text = Header;
		_fieldInput.GetObservable(TextBox.TextProperty).Subscribe(OnTextChanged);
	}
}
