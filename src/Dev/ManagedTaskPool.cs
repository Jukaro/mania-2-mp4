using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ManagedTaskPool : ObservableObject {
	private readonly SemaphoreSlim _semaphore;
	private readonly ConcurrentDictionary<int, Task> _runningTasks = new();
	private readonly object _lock = new();

	private static int _taskId = 0;

	[ObservableProperty]
	private int _remainingTaskCount = 0;

	public ManagedTaskPool(int maxDegreeOfParallelism) {
		_semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
	}

	public void AddTaskToPool(Func<Task> task)
	{
		int id = _taskId;

		var queuedTask = Task.Run(async () => {
			await _semaphore.WaitAsync();
			try {
				await task();
			} finally {
				_semaphore.Release();
			}
		}).ContinueWith(t => {
			lock (_lock) {
				Task removedTask;
				_runningTasks.TryRemove(id, out removedTask);
				RemainingTaskCount = _runningTasks.Count;
				if (_runningTasks.Count == 0) {
					_taskId = 0;
				}
			}
		});

		lock (_lock) {
			_runningTasks.TryAdd(id, queuedTask);
			_taskId++;
		}
	}
}
