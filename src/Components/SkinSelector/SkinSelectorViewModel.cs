using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mania2mp4.Models;
using Mania2mp4.ViewModels;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.UI;

namespace Mania2mp4.ViewModels;

public partial class SkinSelectorViewModel : ViewModelBase {
	private OsuReplayModel _osuReplayModel;

	private string _skinFolderPath;

	[ObservableProperty]
	List<string> _skinPathList;

	private string _chosenSkin;

	public string ChosenSkin {
		get => _chosenSkin;
		set {
			_chosenSkin = value;
			Logger.LogDebug($"chosen skin: {Path.Combine(_skinFolderPath, _chosenSkin)}");
			Task.Run(() => {
				_osuReplayModel.Skin = SkinParser.Parse(Path.Combine(_skinFolderPath, _chosenSkin));
			});
		}
	}

	public SkinSelectorViewModel(OsuReplayModel osuReplayModel) {
		_osuReplayModel = osuReplayModel;

		Task.Run(InitSkinList);
	}

	private async Task InitSkinList() {
		while (Paths.OsuDirectoryPath == null || Paths.OsuSongsDirectoryPath == null);

		_skinFolderPath = Path.Combine(Paths.OsuDirectoryPath, "Skins");
		string[] skinPaths = Directory.GetDirectories(_skinFolderPath);

		// Logger.LogDebug($"folder: {_skinFolderPath}");

		// foreach (string skinPath in skinPaths)
		// 	Logger.LogDebug(skinPath);

		SkinPathList = skinPaths.Select(p => Path.GetFileName(p)).ToList();
	}
}
