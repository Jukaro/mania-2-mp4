using System;

namespace Mania2mp4.Controls;

public class ToStringeableException : Exception {
	private string _str;

	public ToStringeableException(string message) {
		_str = message;
	}

	public override string ToString() {
		return _str;
	}
}
