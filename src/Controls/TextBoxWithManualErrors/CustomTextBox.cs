using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Mania2mp4.DataValidation;

namespace Mania2mp4.Controls;

public class CustomTextBox : TextBox {
	protected override Type StyleKeyOverride => typeof(TextBox);

	private string? _text;

	public static readonly DirectProperty<CustomTextBox, object?> OutputProperty =
		AvaloniaProperty.RegisterDirect<CustomTextBox, object?>(
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
			Text = value != null ? value.ToString() : null;
		}
	}

	public static readonly StyledProperty<Func<string, ValidationResult>> ValidationFunctionProperty =
		AvaloniaProperty.Register<CustomTextBox, Func<string, ValidationResult>>(nameof(ValidationFunction));

	public Func<string, ValidationResult> ValidationFunction {
		get => GetValue(ValidationFunctionProperty);
		set => SetValue(ValidationFunctionProperty, value);
	}

	private string? _errorMessage;

	public CustomTextBox() {
		this.GetObservable(TextProperty).Subscribe(OnTextChanged);
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

		// Logger.LogDebug($"errorMessage: {errorMessage}");

		_errorMessage = errorMessage;
		Exception? error = errorMessage == null ? null : new ToStringeableException(errorMessage);
		DataValidationErrors.SetError(this, error);
	}
}
