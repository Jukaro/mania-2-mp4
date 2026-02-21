using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

public interface IFilter : INotifyPropertyChanged {
	bool Apply(object value);
}

public partial class AllowedValuesFilter<T, U> : ObservableObject, IFilter {
	public string Name;
	public Func<T, U> ValueGetter;

	[ObservableProperty]
	private List<U> _allowedValues = new();

	public AllowedValuesFilter(string name) {
		Name = name;
	}

	public bool Apply(object rawObject) {
		if (rawObject is not T obj) throw new ArgumentException($"Invalid argument type: {rawObject.GetType()}");
		if (AllowedValues.Count == 0) return true;
		U value = ValueGetter(obj);
		if (AllowedValues.Contains(value)) return true;
		return false;
	}
}
