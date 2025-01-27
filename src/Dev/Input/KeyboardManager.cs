using Rythmify.Core;
using SharpHook;

namespace Rythmify.UI;

public class ManagedGlobalHook {
	private static ManagedGlobalHook _instance;

	public static ManagedGlobalHook Instance {
		get {
			_instance ??= new ManagedGlobalHook();
			return _instance;
		}
	}

	public readonly TaskPoolGlobalHook Hook = new();

	ManagedGlobalHook() {
		TaskEnBien<bool> task = new();
		task.Start(() => {
			 Hook.Run();
			 return true;
		});
	}
}
