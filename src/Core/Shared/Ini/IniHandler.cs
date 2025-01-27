using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
sealed class IniHandlerAttribute : Attribute {
	public string Handler { get; }

	public IniHandlerAttribute(string handler) {
		Handler = handler;
	}
}
