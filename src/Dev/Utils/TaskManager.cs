using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rythmify.Core;

public interface ITask {
	bool IsCompleted();
	void SetCompleted(bool value);
	void RunCallback();
}

public class TaskEnBien<ReturnType> : ITask {
	private volatile bool _isCompleted;
	public ReturnType Result { get; private set; }
	public Action<ReturnType> Callback;

	public TaskEnBien() {
		TaskManager.Register(this);
	}

	public void Start(Func<ReturnType> taskFunction) {
		Task.Run(() => {
			Result = taskFunction();
			_isCompleted = true;
		});
	}

	public void RunCallback() {
		Callback(Result);
	}

	public bool IsCompleted() => _isCompleted;
	public void SetCompleted(bool value) => _isCompleted = value;
}

public static class TaskManager {
	private static readonly List<ITask> _tasks = new();

	public static void Register(ITask task) {
		_tasks.Add(task);
	}

	public static void Update() {
		for (int i = 0; i < _tasks.Count; i++) {
			if (_tasks[i].IsCompleted()) {
				_tasks[i].RunCallback();
				_tasks.RemoveAt(i);
				i--;
			}
		}
	}
}
