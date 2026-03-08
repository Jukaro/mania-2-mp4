using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Databases;
using Rythmify.UI;

namespace Mania2mp4.ViewModels;

public partial class DatabasesViewModel : ViewModelBase {
	private DatabasesService _databasesModel;

	[ObservableProperty]
	private string _selectedOsuFolder = !string.IsNullOrEmpty(Paths.OsuDirectoryPath) ? Paths.OsuDirectoryPath : "";

	[ObservableProperty]
	private string _selectedOsuSongsFolder = !string.IsNullOrEmpty(Paths.OsuSongsDirectoryPath) ? Paths.OsuSongsDirectoryPath : "";

	[ObservableProperty]
	private string _selectedReplaysFolder = "";

	public DatabasesViewModel(DatabasesService databasesModel) {
		_databasesModel = databasesModel;
	}

	[RelayCommand]
	public async Task ChooseOsuFolder(Window parent) {
		SelectedOsuFolder = await ChooseFolder(parent);
		Paths.OsuDirectoryPath = SelectedOsuFolder;
		_databasesModel.TryInit();
	}

	[RelayCommand]
	public async Task ChooseOsuSongsFolder(Window parent) {
		SelectedOsuSongsFolder = await ChooseFolder(parent);
		Paths.OsuSongsDirectoryPath = SelectedOsuSongsFolder;
		_databasesModel.TryInit();
	}

	[RelayCommand]
	public async Task ChooseReplaysFolder(Window parent) {
		SelectedReplaysFolder = await ChooseFolder(parent);
	}

	private async Task<string> ChooseFolder(Window parent) {
		var selectedFolders = await parent.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {
			Title = "Choose Folder",
			AllowMultiple = false
		});

		if (selectedFolders == null || selectedFolders.Count == 0) return null;

		return selectedFolders[0].Path.LocalPath;
	}

	public async Task RewriteScoreDB() {
		if (_databasesModel.ScoreDB == null || SelectedReplaysFolder == "")
			return;

		_ = Task.Run(async () => WIPFunctions.UpdateScoreDB(_databasesModel, SelectedReplaysFolder))
			.ContinueWith((task) => {
				ScoreDBWriter.Write(_databasesModel.ScoreDB, Path.Combine(Paths.OsuDirectoryPath, "scores2.db"));
			});
	}

	public void RefreshDatabases() {
		_databasesModel.TryInit();
	}
}
