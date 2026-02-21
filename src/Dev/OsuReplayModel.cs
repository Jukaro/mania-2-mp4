using CommunityToolkit.Mvvm.ComponentModel;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;

namespace Mania2mp4.Models;

public partial class OsuReplayModel : ObservableObject {
	[ObservableProperty]
	private SkinData? _skin = null;

	[ObservableProperty]
	private BeatmapWithScores? _beatmap = null;

	[ObservableProperty]
	private ReplayData? _score = null;
}
