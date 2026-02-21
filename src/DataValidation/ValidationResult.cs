namespace Mania2mp4.DataValidation;

public class ValidationResult {
	public object? Value;
	public string? Error;

	public ValidationResult(object? value, string? error) {
		Value = value;
		Error = error;
	}
}
